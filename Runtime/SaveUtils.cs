using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SaveSystem
{

    public static class SaveUtils
    {
        private const string Pass = "9183h9ghsdpu19g8qwfg193rfapidfg1-97gdiufg193fgas=fg13ut";
        
        public struct LoadReport
        {
            public bool Success;
            public string FilePath;
            public bool FileExisted;
            public string Version;
            public string DeviceId;
            public byte[] Checksum;
            public string ChecksumStr;
            public byte[] Data;
            public string DataStr;
            public bool DifferentVersion => Version != Application.version;
            public bool DifferentDevice => DeviceId != SystemInfo.deviceUniqueIdentifier;
            public bool DifferentChecksum => IsChecksumDifferent();

            private bool IsChecksumDifferent()
            {
                if (Data == null)
                    return true;
                var newChecksum = GenerateMd5(Data);
                int len = newChecksum.Length;
                for (int i = 0; i < len; i++)
                {
                    byte c = Checksum[i];
                    byte nc = newChecksum[i];
                    if (nc != c)
                        return false;
                }
                return true;
            }

            public override string ToString()
            {
                return $"LoadReport\n" +
                       $"\tSuccess: {Success}\n" +
                       $"\tFilePath: {FilePath}\n" +
                       $"\tVersion: {Version}\n" +
                       $"\tDeviceId: {DeviceId}\n" +
                       $"\tChecksum: {Checksum}\n" +
                       $"\tJson: {Data}\n" +
                       $"\tFileExisted: {FileExisted}\n" +
                       $"\tDifferentVersion: {DifferentVersion}\n" +
                       $"\tDifferentDevice: {DifferentDevice}\n" +
                       $"\tDifferentChecksum: {DifferentChecksum}";
            }
        }
        
        private static readonly string EXT = "dat";
        
        private static string GetPersistentPath(IPersistent obj)
        {
            #if UNITY_EDITOR
                var filePath = Path.Combine(Application.persistentDataPath, $"editor-{obj.PersistentFileName}.{EXT}");
            #else
                var filePath = Path.Combine(Application.persistentDataPath, $"{obj.PersistentFileName}.{EXT}");
            #endif
            return filePath;
        }

        public static IList<IPersistent> GetAllPersistentObjects()
        {
            var label = SaveSystemSettings.Instance.PersistentsLabel;
            var result = Addressables
                .LoadAssetsAsync<IPersistent>(label, null)
                .WaitForCompletion();
            return result;
        }
        
        public static async Task<IList<IPersistent>> GetAllPersistentObjectsAsync(CancellationToken ct)
        {
            var label = SaveSystemSettings.Instance.PersistentsLabel;
            var operation = Addressables.LoadAssetsAsync<IPersistent>(label, null);
            while (!operation.IsDone && !ct.IsCancellationRequested)
            {
                await Task.Yield();
            }

            return operation.Result;
        }
        
        public static void SaveObject(IPersistent obj)
        {
            AssetGuidsDatabase.UseAndDispose(database =>
            {
                if (obj is IPersistentCallbackReceiver receiver)
                {
                    receiver.OnBeforeSave(database);
                }
                
                var filePath = GetPersistentPath(obj);
                var file = File.Open(filePath, FileMode.Create);
            
                var serializer = SaveSystemSettings.Instance.Serializer;

                byte[] data;
                if (obj is IPersistentCustomSerializable customSerializable)
                {
                    data = customSerializable.WriteData(database);
                }
                else
                {
                    data = serializer.Serialize(obj, database);
                }
                
                var version = Application.version;
                var deviceId = SystemInfo.deviceUniqueIdentifier;
                var encryptedData = SaveSystemSettings.Instance.EncryptData
                    ? DesEncryption.Encrypt(data, Pass)
                    : data;
                var checksum = GenerateMd5(data);
            
                using var writer = new BinaryWriter(file);
                writer.Write(version);
                writer.Write(deviceId);
                writer.Write(checksum.Length);
                writer.Write(checksum);
                writer.Write(encryptedData.Length);
                writer.Write(encryptedData);
                Debug.Log($"- SaveUtils.SaveObject: saving to {filePath}");
            });
            
            if (obj is IPersistentCallbackReceiver receiver)
            {
                receiver.OnAfterSave();
            }
        }

        public static LoadReport LoadObject(IPersistent obj)
        {
            if (obj is IPersistentCallbackReceiver receiver)
            {
                receiver.OnBeforeLoad();
            }
            
            var report = new LoadReport();
            var filePath = GetPersistentPath(obj);
            report.FilePath = filePath;
            Debug.Log($"- SaveUtils.LoadObject: loading from {filePath}");

            if(!File.Exists(filePath))
            {
                report.Success = false;
                report.FileExisted = false;
                return report;
            }

            // read file and fill report
            var file = File.OpenRead(filePath);
            using var reader = new BinaryReader(file);
            var version = reader.ReadString();
            var deviceId = reader.ReadString();
            int checksumLength = reader.ReadInt32();
            var checkSum = reader.ReadBytes(checksumLength);
            int dataLength = reader.ReadInt32();
            var data = reader.ReadBytes(dataLength);

            byte[] decryptedData;
            if (SaveSystemSettings.Instance.EncryptData)
                DesEncryption.TryDecrypt(data, Pass, out decryptedData);
            else
                decryptedData = data;

            report.Success = true;
            report.FileExisted = true;
            report.Version = version;
            report.DeviceId = deviceId;
            report.Checksum = checkSum;
            report.Data = decryptedData;
            report.ChecksumStr = Md5HashToString(checkSum);
            report.DataStr = Encoding.UTF8.GetString(decryptedData);

            // deserialize data
            var serializer = SaveSystemSettings.Instance.Serializer;
            AssetGuidsDatabase.UseAndDispose(database =>
            {
                if (obj is IPersistentCustomSerializable customSerializable)
                {
                    customSerializable.ReadData(decryptedData, database);
                }
                else
                {
                    serializer.Deserialize(decryptedData, obj, database);
                }
                
                if (report.DifferentVersion)
                {
//                    obj.ReadPreviousVersion(version, dacryptedData);
                }
            
                if (obj is IPersistentCallbackReceiver receiver)
                {
                    receiver.OnAfterLoad(database);
                }
            });
            
            return report;
        }

        public static void Save(this IPersistent obj)
        {
            SaveObject(obj);
        }
        
        public static LoadReport Load(this IPersistent obj)
        {
            return LoadObject(obj);
        }
        
        private static byte[] GenerateMd5(byte[] data)
        {
            var md5 = MD5.Create();
            byte[] hash = md5.ComputeHash(data);
            return hash;
        }
        
        private static string GenerateMd5(string text)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(text);
            byte[] hash = GenerateMd5(inputBytes);
            return Md5HashToString(hash);
        }

        private static string Md5HashToString(byte[] hash)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }
}