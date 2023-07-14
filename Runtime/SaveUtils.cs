using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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
        
        public static string GetPersistentPath(ScriptableObject obj)
        {
            #if UNITY_EDITOR
                var filePath = Path.Combine(Application.persistentDataPath, $"editor-{GetPersistentFileName(obj)}.{EXT}");
            #else
                var filePath = Path.Combine(Application.persistentDataPath, $"{GetPersistentFileName(obj)}.{EXT}");
            #endif
            return filePath;
        }

        public static string GetPersistentFileName(ScriptableObject obj)
        {
            var guid = AssetGuidsDatabase.Instance.GetGuid(obj);
            return guid;
        }

        public static void ResetPersistentObject(ScriptableObject obj)
        {
            if (obj is IPersistentResetable resetable)
            {
                resetable.ResetToDefault();
            }
            else
            {
                var defaultObject = ScriptableObject.CreateInstance(obj.GetType());
                ReflectionUtils.CopyTo(defaultObject, obj);
            }
        }
        
        public static async Task SaveObject(ScriptableObject obj)
        {
            await Task.Yield();  // this is to allow this function to run asynchronously
            
            var database = AssetGuidsDatabase.Instance;
            
            if (obj is IPersistentCallbackReceiver receiverBefore)
            {
                receiverBefore.OnBeforeSave(database);
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
            DebugLog($"- SaveUtils.SaveObject: saving to {filePath}");
            
            if (obj is IPersistentCallbackReceiver receiverAfter)
            {
                receiverAfter.OnAfterSave();
            }
            SaveLoadBroadcaster.Instance.NotifySave(obj);
        }

        public static async Task<LoadReport> LoadObject(ScriptableObject obj)
        {
            await Task.Yield();  // this is to allow this function to run asynchronously
            
            if (obj is IPersistentCallbackReceiver receiverBefore)
            {
                receiverBefore.OnBeforeLoad();
            }
            
            var report = new LoadReport();
            var filePath = GetPersistentPath(obj);
            report.FilePath = filePath;
            DebugLog($"- SaveUtils.LoadObject: loading from {filePath}");

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
            var database = AssetGuidsDatabase.Instance;
            
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
//                    obj.ReadPreviousVersion(version, decryptedData);
            }
        
            if (obj is IPersistentCallbackReceiver receiverAfter)
            {
                receiverAfter.OnAfterLoad(database);
            }

            // notify object was loaded
            SaveLoadBroadcaster.Instance.NotifyLoad(obj);
            
            return report;
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
        
        private static void DebugLog(string message)
        {
            if (SaveSystemSettings.Instance.DebugLogging)
            {
                Debug.Log(message);
            }
        }
    }
}