using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Content;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SaveSystem.Editor
{
    public static class SaveDataEditorUtils
    {
        [MenuItem("Tools/Facticus/SaveSystem/Reset data")]
        public static void ResetData()
        {
            var persistentObjects = SaveUtils.GetAllPersistentObjects();
            foreach (var persistent in persistentObjects)
            {
                SaveUtils.ResetPersistentObject(persistent);
            }
        }

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
            foreach (var obj in selected)
            {
                SaveUtils.ResetPersistentObject(obj);
            }
        }
        
        [MenuItem("Assets/Facticus/SaveSystem/Remove data", false, 0)]
        [MenuItem("CONTEXT/ScriptableObject/Remove data", false, 100)]
        public static void RemoveObjectData()
        {
            var selected = Selection.GetFiltered<ScriptableObject>(SelectionMode.Assets);
            foreach (var obj in selected)
            {
                var path = SaveUtils.GetPersistentPath(obj);
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }
        
        [MenuItem("Assets/Facticus/SaveSystem/Remove and reset data", false, 0)]
        [MenuItem("CONTEXT/ScriptableObject/Remove and reset data", false, 100)]
        public static void RemoveAndResetObjectData()
        {
            ResetObjectData();
            RemoveObjectData();
        }

        [MenuItem("Assets/Facticus/SaveSystem/Save", true)]
        [MenuItem("Assets/Facticus/SaveSystem/Load", true)]
        [MenuItem("Assets/Facticus/SaveSystem/Remove data", true)]
        [MenuItem("Assets/Facticus/SaveSystem/Reset data", true)]
        public static bool ValidateMenus()
        {
            var selected = Selection.GetFiltered<ScriptableObject>(SelectionMode.Assets);
            return selected.Length > 0;
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