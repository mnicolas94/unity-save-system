using System.Diagnostics;
using System.IO;
using SaveSystem.Storages;
using UnityEditor;
using UnityEngine;

namespace SaveSystem.Editor
{
    public static class SaveSystemMenuItems
    {
        [MenuItem("Assets/Facticus/SaveSystem/Save", false, 0)]
        [MenuItem("CONTEXT/ScriptableObject/Save", false, 100)]
        public static void SaveObjectData()
        {
            var selected = Selection.GetFiltered<ScriptableObject>(SelectionMode.Assets);
            foreach (var obj in selected)
            {
                obj.Save();
            }
        }
        
        [MenuItem("Assets/Facticus/SaveSystem/Load", false, 0)]
        [MenuItem("CONTEXT/ScriptableObject/Load", false, 100)]
        public static void LoadObjectData()
        {
            var selected = Selection.GetFiltered<ScriptableObject>(SelectionMode.Assets);
            foreach (var obj in selected)
            {
                obj.Load();
            }
        }
        
        [MenuItem("Assets/Facticus/SaveSystem/Reset data", false, 0)]
        [MenuItem("CONTEXT/ScriptableObject/Reset data", false, 100)]
        public static void ResetObjectData()
        {
            var selected = Selection.GetFiltered<ScriptableObject>(SelectionMode.Assets);
            Undo.RecordObjects(selected, "Reset data");
            foreach (var obj in selected)
            {
                obj.ResetToDefault();
            }
        }
        
        [MenuItem("Assets/Facticus/SaveSystem/Remove data", false, 0)]
        [MenuItem("CONTEXT/ScriptableObject/Remove data", false, 100)]
        public static async void RemoveObjectData()
        {
            var saveSystemSettings = SaveSystemSettings.Instance;
            var storage = saveSystemSettings.Storage;
            var guidResolver = saveSystemSettings.GuidsResolver;
            var profile = SaveUtils.GetEditorAwareProfile();
            
            var selected = Selection.GetFiltered<ScriptableObject>(SelectionMode.Assets);
            foreach (var obj in selected)
            {
                var guid = guidResolver.GetGuid(obj);
                await storage.Delete(profile, guid);
            }
        }
        
        [MenuItem("Assets/Facticus/SaveSystem/Remove and reset data", false, 0)]
        [MenuItem("CONTEXT/ScriptableObject/Remove and reset data", false, 100)]
        public static void RemoveAndResetObjectData()
        {
            ResetObjectData();
            RemoveObjectData();
        }
        
        [MenuItem("Assets/Facticus/SaveSystem/Open file location", false, 0)]
        [MenuItem("CONTEXT/ScriptableObject/Open file location", false, 100)]
        public static void OpenFileLocation()
        {
            var saveSystemSettings = SaveSystemSettings.Instance;
            var storage = saveSystemSettings.Storage;
            var guidResolver = saveSystemSettings.GuidsResolver;
            var profile = SaveUtils.GetEditorAwareProfile();
            
            var selected = Selection.GetFiltered<ScriptableObject>(SelectionMode.Assets)[0];
            var guid = guidResolver.GetGuid(selected);
            if (storage is FilesStorage filesStorage)
            {
                var path = filesStorage.GetFilePath(profile, guid);
                path = Path.GetFullPath(path);
                if (File.Exists(path))
                {
                    Process.Start("explorer.exe", "/select, " + path);
                }
            }
        }

        [MenuItem("Assets/Facticus/SaveSystem/Save", true)]
        [MenuItem("Assets/Facticus/SaveSystem/Load", true)]
        [MenuItem("Assets/Facticus/SaveSystem/Remove data", true)]
        [MenuItem("Assets/Facticus/SaveSystem/Reset data", true)]
        [MenuItem("Assets/Facticus/SaveSystem/Remove and reset data", true)]
        public static bool ValidateMenus()
        {
            var selected = Selection.GetFiltered<ScriptableObject>(SelectionMode.Assets);
            return selected.Length > 0;
        }
        
        [MenuItem("Assets/Facticus/SaveSystem/Open file location", true)]
        public static bool ValidateMenusOnlyOneSelected()
        {
            var selected = Selection.GetFiltered<ScriptableObject>(SelectionMode.Assets);
            return selected.Length == 1;
        }
    }
}