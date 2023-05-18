using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace SaveSystem
{
    public static class Persistent
    {
        public static async Task Save(this ScriptableObject obj)
        {
            await SaveUtils.SaveObject(obj);
        }
        
        public static async Task<SaveUtils.LoadReport> Load(this ScriptableObject obj)
        {
            return await SaveUtils.LoadObject(obj);
        }

        public static async Task<SaveUtils.LoadReport> LoadOrCreate(this ScriptableObject obj)
        {
            var report = await SaveUtils.LoadObject(obj);
            if (!report.Success)
            {
                obj.ResetToDefault();
                await obj.Save();
            }

            return report;
        }

        public static string GetPersistentFileName(this ScriptableObject obj)
        {
            return SaveUtils.GetPersistentFileName(obj);
        }

        public static bool IsSaved(this ScriptableObject obj)
        {
            var path = SaveUtils.GetPersistentPath(obj);
            return File.Exists(path);
        }
        
        public static void ResetToDefault(this ScriptableObject obj)
        {
            SaveUtils.ResetPersistentObject(obj);
        }
    }
}