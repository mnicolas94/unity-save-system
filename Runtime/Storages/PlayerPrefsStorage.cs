using System;
using System.Threading.Tasks;
using UnityEngine;

namespace SaveSystem.Storages
{
    public class PlayerPrefsStorage : IStorage
    {
        private string GetPlayerPrefsKey(string profile, string key)
        {
            return $"facticus-savesystem-storage-{profile}-{key}";
        }
        
        public async Task<bool> ExistsData(string profile, string key)
        {
            var prefsKey = GetPlayerPrefsKey(profile, key);
            return PlayerPrefs.HasKey(prefsKey);
        }

        public async Task Write(string profile, string key, byte[] data)
        {
            var prefsKey = GetPlayerPrefsKey(profile, key);
            var base64 = Convert.ToBase64String(data);
            PlayerPrefs.SetString(prefsKey, base64);
        }

        public async Task<(bool, byte[])> Read(string profile, string key)
        {
            var prefsKey = GetPlayerPrefsKey(profile, key);
            var exists = PlayerPrefs.HasKey(prefsKey);
            if (exists)
            {
                var base64 = PlayerPrefs.GetString(prefsKey);
                var data = Convert.FromBase64String(base64);
                return (true, data);
            }

            return (false, null);
        }

        public async Task Delete(string profile, string key)
        {
            var prefsKey = GetPlayerPrefsKey(profile, key);
            PlayerPrefs.DeleteKey(prefsKey);
        }
    }
}