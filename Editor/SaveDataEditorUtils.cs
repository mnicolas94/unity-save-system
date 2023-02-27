using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
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
            var objectsGuids = filesGuids.ConvertAll(guid =>
            {
                string objPath = AssetDatabase.GUIDToAssetPath(guid);
                var obj = AssetDatabase.LoadAssetAtPath<Object>(objPath);
                return (obj, guid);
            });
            return objectsGuids;
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