using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using SaveSystem.Utilities;
using UnityEngine;

namespace SaveSystem
{

    public static class SaveUtils
    {
        private const string Pass = "9183h9ghsdpu19g8qwfg193rfapidfg1-97gdiufg193fgas=fg13ut";
        
        public struct LoadReport
        {
            public bool Success;
            public string FailureReason;
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
                       $"\tFailureReason: {FailureReason}\n" +
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
        
        public static string GetProfile()
        {
            return "profile";  // TODO get proper profile
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
            
            var saveSystemSettings = SaveSystemSettings.Instance;
            var guidResolver = saveSystemSettings.GuidsResolver;
            
            if (obj is IPersistentCallbackReceiver receiverBefore)
            {
                receiverBefore.OnBeforeSave(guidResolver);
            }
            
            // serialize object
            var serializer = saveSystemSettings.Serializer;
            byte[] data;
            
            if (obj is IPersistentCustomSerializable customSerializable)
            {
                data = customSerializable.WriteData(guidResolver);
            }
            else
            {
                data = serializer.Serialize(obj, guidResolver);
            }
            
            // get save metadata and encrypt data
            var version = Application.version;
            var deviceId = SystemInfo.deviceUniqueIdentifier;
            var checksum = GenerateMd5(data);
            var encryptedData = saveSystemSettings.EncryptData
                ? DesEncryption.Encrypt(data, Pass)
                : data;

            // write in storage
            await using var memoryStream = new MemoryStream();
            await using var writer = new BinaryWriter(memoryStream);
            writer.Write(version);
            writer.Write(deviceId);
            writer.Write(checksum.Length);
            writer.Write(checksum);
            writer.Write(encryptedData.Length);
            writer.Write(encryptedData);
            
            var storage = saveSystemSettings.Storage;
            var guid = guidResolver.GetGuid(obj);
            var profile = GetProfile();
            await storage.Write(profile, guid, memoryStream.ToArray());
            
            // DebugLog($"- SaveUtils.SaveObject: saving to {filePath}");
            
            if (obj is IPersistentCallbackReceiver receiverAfter)
            {
                receiverAfter.OnAfterSave();
            }
            SaveLoadBroadcaster.Instance.NotifySave(obj);
        }

        public static async Task<LoadReport> LoadObject(ScriptableObject obj)
        {
            await Task.Yield();  // this is to allow this function to run asynchronously
            var report = new LoadReport();

            try
            {
                if (obj is IPersistentCallbackReceiver receiverBefore)
                {
                    receiverBefore.OnBeforeLoad();
                }
            
                // DebugLog($"- SaveUtils.LoadObject: loading from {filePath}");
                var saveSystemSettings = SaveSystemSettings.Instance;
                var storage = saveSystemSettings.Storage;
                var guidResolver = saveSystemSettings.GuidsResolver;

                var profile = GetProfile();
                var guid = guidResolver.GetGuid(obj);
                var (success, storageData) = await storage.Read(profile, guid);

                if(!success)
                {
                    report.Success = false;
                    report.FailureReason = "Storage couldn't load data";
                    report.FileExisted = false;
                    return report;
                }

                // read from storage and fill report
                await using var memoryStream = new MemoryStream(storageData);
                using var reader = new BinaryReader(memoryStream);
                var version = reader.ReadString();
                var deviceId = reader.ReadString();
                int checksumLength = reader.ReadInt32();
                var checkSum = reader.ReadBytes(checksumLength);
                int dataLength = reader.ReadInt32();
                var data = reader.ReadBytes(dataLength);

                byte[] decryptedData;
                if (saveSystemSettings.EncryptData)
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
                var serializer = saveSystemSettings.Serializer;
            
                if (obj is IPersistentCustomSerializable customSerializable)
                {
                    customSerializable.ReadData(decryptedData, guidResolver);
                }
                else
                {
                    serializer.Deserialize(decryptedData, obj, guidResolver);
                }
            
                if (report.DifferentVersion)
                {
                    // obj.ReadPreviousVersion(version, decryptedData);
                }
        
                if (obj is IPersistentCallbackReceiver receiverAfter)
                {
                    receiverAfter.OnAfterLoad(guidResolver);
                }

                // notify object was loaded
                SaveLoadBroadcaster.Instance.NotifyLoad(obj);
            }
            catch (Exception e)
            {
                report.Success = false;
                report.FailureReason = e.Message;
                DebugLogError($"Error during load: {e.Message}");
            }
            
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
        
        private static void DebugLogError(string message)
        {
            // always log errors
            Debug.LogError(message);
        }
    }
}