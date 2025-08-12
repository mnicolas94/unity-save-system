using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace SaveSystem
{
    public static class ScriptableObjectExtensions
    {
        private const float DefaultSaveCooldown = 1f;
        private static readonly Dictionary<ScriptableObject, float> _lastSavedTime = new ();
        private static readonly List<ScriptableObject> _onCooldown = new ();
        
        /// <summary>
        /// Saves the object avoiding saving too often to reduce overhead. If the request to save is too often, the save will be performed after
        /// a cooldown. It ignores any save request during the cooldown period.
        /// </summary>
        /// <param name="obj"></param>
        public static async Task SaveNotOften(this ScriptableObject obj)
        {
            var cooldown = DefaultSaveCooldown;
            var lastSaveTime = _lastSavedTime.GetValueOrDefault(obj, -cooldown * 2);
            var elapsed = Time.time - lastSaveTime;
            
            if (elapsed < cooldown)  // trying to save too early
            {
                if (!_onCooldown.Contains(obj))
                {
                    SaveAfterCooldown(obj, cooldown);
                }
                return;
            }

            await obj.Save();
        }
        
        private static async void SaveAfterCooldown(ScriptableObject obj, float cooldown)
        {
            _onCooldown.Add(obj);
                    
            await Awaitable.WaitForSecondsAsync(cooldown);
            await obj.Save();

            _onCooldown.Remove(obj);
        }
        
        public static async Task Save(this ScriptableObject obj)
        {
            _lastSavedTime[obj] = Time.time;
            
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
            if (obj is IPersistentAdapter adapter)
            {
                await adapter.DeleteData();
                return;
            }
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