using UnityEngine;

namespace SaveSystem
{
    public static class Persistent
    {
        public static void Save(this ScriptableObject obj)
        {
            SaveUtils.SaveObject(obj);
        }
        
        public static SaveUtils.LoadReport Load(this ScriptableObject obj)
        {
            return SaveUtils.LoadObject(obj);
        }

        public static string GetPersistentFileName(this ScriptableObject obj)
        {
            return SaveUtils.GetPersistentFileName(obj);
        }
        
        public static void ResetToDefault(this ScriptableObject obj)
        {
            SaveUtils.ResetPersistentObject(obj);
        }
    }
}