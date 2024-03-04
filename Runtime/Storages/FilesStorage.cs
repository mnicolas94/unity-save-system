using System;
using System.IO;
using System.Threading.Tasks;
using UnityEditor.UIElements;
using UnityEngine;

namespace SaveSystem.Storages
{
    [AddTypeMenu("MultipleFiles")]
    [Serializable]
    public class FilesStorage : IStorage, IStorageStream
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
            if (string.IsNullOrEmpty(profile))
            {
                return Path.Combine(Application.persistentDataPath, $"{key}.{_fileExtension}");
            }
            else
            {
                return Path.Combine(Application.persistentDataPath, $"{profile}-{key}.{_fileExtension}");
            }
        }

        public bool TryGetStreamToRead(string profile, string key, out Stream stream)
        {
            var filePath = GetFilePath(profile, key);
            if (File.Exists(filePath))
            {
                stream = File.OpenRead(filePath);
                return true;
            }

            stream = null;
            return false;
        }

        public Stream GetStreamToWrite(string profile, string key)
        {
            var filePath = GetFilePath(profile, key);
            var file = File.Open(filePath, FileMode.Create);
            return file;
        }
    }
}