using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace SaveSystem
{
    public static class ScriptableObjectExtensions
    {
        private const float DefaultSaveCooldown = 1f;
        private static readonly Dictionary<ScriptableObject, float> LastSavedTimes = new ();
        private static readonly Dictionary<ScriptableObject, float> CustomCooldowns = new ();
        private static readonly List<ScriptableObject> OnCooldown = new ();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init()
        {
            LastSavedTimes.Clear();
            CustomCooldowns.Clear();
            OnCooldown.Clear();
        }

        /// <summary>
        /// Saves the object avoiding saving too often to reduce overhead. If the request to save is too often, the save will be performed after
        /// a cooldown. It ignores any save request during the cooldown period.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="cooldown">The allowed time between saves</param>
        public static async Task SaveNotOften(this ScriptableObject obj, float cooldown)
        {
            CustomCooldowns[obj] = cooldown;
            await SaveNotOften(obj);
        }
        
        /// <summary>
        /// Saves the object avoiding saving too often to reduce overhead. If the request to save is too often, the save will be performed after
        /// a cooldown. It ignores any save request during the cooldown period.
        /// </summary>
        /// <param name="obj"></param>
        public static async Task SaveNotOften(this ScriptableObject obj)
        {
            var cooldown = CustomCooldowns.GetValueOrDefault(obj, DefaultSaveCooldown);
            var lastSaveTime = LastSavedTimes.GetValueOrDefault(obj, -cooldown * 2);
            var elapsed = Time.time - lastSaveTime;
            
            if (elapsed < cooldown)  // trying to save too early
            {
                if (!OnCooldown.Contains(obj))
                {
                    SaveAfterCooldown(obj, cooldown);
                }
                return;
            }

            await obj.Save();
        }
        
        private static async void SaveAfterCooldown(ScriptableObject obj, float cooldown)
        {
            OnCooldown.Add(obj);
                    
            await Awaitable.WaitForSecondsAsync(cooldown);
            await obj.Save();

            OnCooldown.Remove(obj);
        }
        
        public static async Task Save(this ScriptableObject obj)
        {
            LastSavedTimes[obj] = Time.time;
            
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