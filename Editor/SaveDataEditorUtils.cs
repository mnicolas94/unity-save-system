using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using SaveSystem.Storages;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SaveSystem.Editor
{
    public static class SaveDataEditorUtils
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
                SaveUtils.ResetPersistentObject(obj);
            }
        }
        
        [MenuItem("Assets/Facticus/SaveSystem/Remove data", false, 0)]
        [MenuItem("CONTEXT/ScriptableObject/Remove data", false, 100)]
        public static async void RemoveObjectData()
        {
            var saveSystemSettings = SaveSystemSettings.Instance;
            var storage = saveSystemSettings.Storage;
            var guidResolver = saveSystemSettings.GuidsResolver;
            var profile = SaveUtils.GetProfile();
            
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
            var profile = SaveUtils.GetProfile();
            
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
        
        public static List<(Object obj, string guid)> GetDataObjectsAndGuids()
        {
            // get assets
            var searchPaths = SaveSystemSettings.Instance.SearchDatabaseAssetsInPaths;
            var filesGuids = GetFilesGuids(searchPaths);

            // ignore assets
            var ignorePaths = SaveSystemSettings.Instance.IgnoreDatabaseAssetsInPaths;
            var ignoreGuids = GetFilesGuids(ignorePaths);
            filesGuids.RemoveAll(guid => ignoreGuids.Contains(guid));

            // get objects
            var objectsGuids = filesGuids.SelectMany(guid =>
            {
                string objPath = AssetDatabase.GUIDToAssetPath(guid);
                var isSceneAsset = AssetDatabase.GetMainAssetTypeAtPath(objPath) == typeof(SceneAsset);
                if (isSceneAsset)
                {
                    var obj = AssetDatabase.LoadAssetAtPath<Object>(objPath);
                    return new[] { (obj, guid) };
                }
                else
                {
                    var objects = AssetDatabase.LoadAllAssetsAtPath(objPath);
                    var objectsGuids = objects.Select(obj =>
                    {
                        var isMain = AssetDatabase.IsMainAsset(obj);
                        AssetDatabase.TryGetGUIDAndLocalFileIdentifier(obj, out var g, out long localFileId);
                        var databaseId = isMain ? guid : $"{guid}---{localFileId}";
                        return (obj, databaseId);
                    });

                    return objectsGuids;
                }
            });
            return objectsGuids.ToList();
        }
        
        private static List<string> GetFilesGuids(List<string> paths)
        {
            var folders = paths.Where(AssetDatabase.IsValidFolder).ToArray();
            var files = paths.Where(path => !AssetDatabase.IsValidFolder(path));

            var guids = folders.Length > 0
                ? AssetDatabase.FindAssets("", folders)
                : Array.Empty<string>();
            var filesGuids = guids.Where(guid =>
            {
                string objPath = AssetDatabase.GUIDToAssetPath(guid);
                bool isFolder = AssetDatabase.IsValidFolder(objPath);
                return !isFolder;
            });
            var additionalFilesGuids = files.Select(AssetDatabase.AssetPathToGUID);
            filesGuids = filesGuids.Concat(additionalFilesGuids);
            return filesGuids.ToList();
        }
    }
}