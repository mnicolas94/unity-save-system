using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using SaveSystem.Encryption;
using SaveSystem.GuidsResolve.Legacy;
using SaveSystem.Serializers;
using SaveSystem.Storages;
using SaveSystem.Utilities;
using UnityEngine;

namespace SaveSystem
{
    public static class SaveUtils
    {
        public static string GetEditorAwareProfile()
        {
#if UNITY_EDITOR
            return "editor";
#endif
            return SaveSystemSettings.SaveProfile;
        }

        [Obsolete("Use ResetObject instead.")]
        public static void ResetPersistentObject(ScriptableObject obj)
        {
            ResetObject(obj);
        }
        
        public static void ResetObject(ScriptableObject obj)
        {
            if (obj is IPersistentResetable resetable)
            {
                resetable.ResetToDefault();
            }
            else
            {
                var defaultObject = ScriptableObject.CreateInstance(obj.GetType());
                SerializationUtilities.CopyTo(defaultObject, obj);
            }
        }
        
        public static async Task SaveObject(ScriptableObject obj)
        {
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
            var checksum = Md5.GenerateMd5(data);
            var encryptedData = saveSystemSettings.EncryptData
                ? DesEncryption.Encrypt(data)
                : data;

            // write in storage
            var storage = saveSystemSettings.Storage;
            var guid = guidResolver.GetGuid(obj);
            var profile = GetEditorAwareProfile();
            
            Stream stream;
            if (storage is IStorageStream storageStream)
            {
                stream = storageStream.GetStreamToWrite(profile, guid);
            }
            else
            {
                stream = new MemoryStream();
            }
            
            await using var writer = new BinaryWriter(stream);
            writer.Write(version);
            writer.Write(deviceId);
            writer.Write(checksum.Length);
            writer.Write(checksum);
            writer.Write(encryptedData.Length);
            writer.Write(encryptedData);

            if (storage is not IStorageStream)
            {
                var memoryStream = (MemoryStream)stream;
                await storage.Write(profile, guid, memoryStream.ToArray());
            }

            await stream.DisposeAsync();
            
            // DebugLog($"- SaveUtils.SaveObject: saving to {filePath}");
            
            if (obj is IPersistentCallbackReceiver receiverAfter)
            {
                receiverAfter.OnAfterSave();
            }
            SaveLoadBroadcaster.Instance.NotifySave(obj);
        }

        public static async Task<LoadReport> LoadObject(ScriptableObject obj)
        {
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

                // read data from storage
                var profile = GetEditorAwareProfile();
                var guid = guidResolver.GetGuid(obj);
                bool success;
                Stream stream = null;
                if (storage is IStorageStream storageStream)
                {
                    success = storageStream.TryGetStreamToRead(profile, guid, out stream);
                    if (!success && LegacyGuidsDeserialization.TryGetOldGuidFromNewOne(guid, out var oldGuid))
                    {
                        // try to use legacy guids
                        success = storageStream.TryGetStreamToRead(profile, oldGuid, out stream);
                    }
                }
                else
                {
                    var (readSuccess, storageData) = await storage.Read(profile, guid);
                    if (!readSuccess && LegacyGuidsDeserialization.TryGetOldGuidFromNewOne(guid, out var oldGuid))
                    {
                        // try to use legacy guids
                        (readSuccess, storageData) = await storage.Read(profile, oldGuid);
                    }
                    success = readSuccess;
                    if (success)
                    {
                        stream = new MemoryStream(storageData);
                    }
                }
                
                // return if storage couldn't read data
                if(!success)
                {
                    report.Success = false;
                    report.FailureReason = "Storage couldn't load data";
                    report.FileExisted = false;
                    return report;
                }

                // read data from storage
                using var reader = new BinaryReader(stream);
                var version = reader.ReadString();
                var deviceId = reader.ReadString();
                int checksumLength = reader.ReadInt32();
                var checkSum = reader.ReadBytes(checksumLength);
                int dataLength = reader.ReadInt32();
                var data = reader.ReadBytes(dataLength);

                await stream.DisposeAsync();

                // decrypt data if encrypted
                byte[] decryptedData;
                if (saveSystemSettings.EncryptData)
                    DesEncryption.TryDecrypt(data, out decryptedData);
                else
                    decryptedData = data;

                // fill report
                report.Success = true;
                report.FileExisted = true;
                report.Version = version;
                report.DeviceId = deviceId;
                report.Checksum = checkSum;
                report.Data = decryptedData;
                report.ChecksumStr = Md5.Md5HashToString(checkSum);
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
                Logging.DebugLogError($"Error during load: {e.Message}");
            }
            
            return report;
        }

        public static async Task RemoveObjectData(ScriptableObject obj)
        {
            var saveSystemSettings = SaveSystemSettings.Instance;
            var storage = saveSystemSettings.Storage;
            var guidResolver = saveSystemSettings.GuidsResolver;
            var profile = GetEditorAwareProfile();

            if (guidResolver.TryGetGuid(obj, out var id))
            {
                await storage.Delete(profile, id);

                // try to remove legacy data
                if (LegacyGuidsDeserialization.TryGetOldGuidFromNewOne(id, out var oldId))
                {
                    await storage.Delete(profile, oldId);
                }
            }
        }
    }
}