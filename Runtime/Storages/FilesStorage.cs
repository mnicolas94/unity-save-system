﻿using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace SaveSystem.Storages
{
    [AddTypeMenu("MultipleFiles")]
    [Serializable]
    public class FilesStorage : IStorage
    {
        [SerializeField] private string _fileExtension = "dat";

        public async Task<bool> ExistsData(string profile, string key)
        {
            var filePath = GetFilePath(profile, key);
            return File.Exists(filePath);
        }

        public async Task Write(string profile, string key, byte[] data)
        {
            var filePath = GetFilePath(profile, key);
            var file = File.Open(filePath, FileMode.Create);
            
            await using var writer = new BinaryWriter(file);
            writer.Write(data);
        }

        public async Task<(bool, byte[])> Read(string profile, string key)
        {
            var filePath = GetFilePath(profile, key);
            try
            {
                var data = await File.ReadAllBytesAsync(filePath);
                // var data = File.ReadAllBytes(filePath);

                return (true, data);
            }
            catch
            {
                return (false, null);
            }
        }

        public async Task Delete(string profile, string key)
        {
            var filePath = GetFilePath(profile, key);
            File.Delete(filePath);
        }

        public string GetFilePath(string profile, string key)
        {
#if UNITY_EDITOR
            var filePath = Path.Combine(Application.persistentDataPath, $"editor-{key}.{_fileExtension}");
#else
            var filePath = Path.Combine(Application.persistentDataPath, $"{key}.{_fileExtension}");
#endif
            return filePath;
        }
    }
}