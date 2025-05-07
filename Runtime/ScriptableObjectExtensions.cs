using System;
using System.Threading.Tasks;
using UnityEngine;

namespace SaveSystem
{
    public static class ScriptableObjectExtensions
    {
        public static async Task Save(this ScriptableObject obj)
        {
            if (obj is IPersistentAdapter adapter)
            {
                await adapter.Save();
                return;
            }
            
            await SaveUtils.SaveObject(obj);
        }
        
        public static async Task<LoadReport> Load(this ScriptableObject obj)
        {
            if (obj is IPersistentAdapter adapter)
            {
                return await adapter.Load();
            }
            
            return await SaveUtils.LoadObject(obj);
        }

        public static async Task<LoadReport> LoadOrCreate(this ScriptableObject obj)
        {
            if (obj is IPersistentAdapter adapter)
            {
                return await adapter.LoadOrCreate();
            }
            
            var report = await SaveUtils.LoadObject(obj);
            if (!report.Success)
            {
                obj.ResetToDefault();
                SaveLoadBroadcaster.Instance.NotifyLoad(obj);  // notify load since it was not notified in LoadObject function
                await obj.Save();
            }

            return report;
        }

        public static async Task<bool> IsSaved(this ScriptableObject obj)
        {
            if (obj is IPersistentAdapter adapter)
            {
                return await adapter.IsSaved();
            }
            
            var saveSystemSettings = SaveSystemSettings.Instance;
            var storage = saveSystemSettings.Storage;
            var guidResolver = saveSystemSettings.GuidsResolver;

            var profile = SaveUtils.GetEditorAwareProfile();
            var guid = guidResolver.GetGuid(obj);
            var exists = await storage.ExistsData(profile, guid);
            return exists;
        }

        public static async Task DeleteData(this ScriptableObject obj)
        {
            await SaveUtils.RemoveObjectData(obj);
        }
        
        public static void ResetToDefault(this ScriptableObject obj)
        {
            SaveUtils.ResetObject(obj);
        }

        public static void RegisterOnSaveListener(this ScriptableObject obj, Action listener)
        {
            SaveLoadBroadcaster.Instance.RegisterOnSaveListenerForObject(obj, listener);
        }
        
        public static void UnregisterOnSaveListener(this ScriptableObject obj, Action listener)
        {
            SaveLoadBroadcaster.Instance.UnregisterOnSaveListenerForObject(obj, listener);
        }
        
        public static void RegisterOnLoadListener(this ScriptableObject obj, Action listener)
        {
            SaveLoadBroadcaster.Instance.RegisterOnLoadListenerForObject(obj, listener);
        }
        
        public static void UnregisterOnLoadListener(this ScriptableObject obj, Action listener)
        {
            SaveLoadBroadcaster.Instance.UnregisterOnLoadListenerForObject(obj, listener);
        }
    }
}