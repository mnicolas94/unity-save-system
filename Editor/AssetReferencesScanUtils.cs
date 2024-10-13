using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SaveSystem.Editor
{
    public static class AssetReferencesScanUtils
    {
        public static List<(Object obj, string guid)> GetDataObjectsAndGuids()
        {
            var filesGuids = GetGuidsInPaths(
                SaveSystemSettings.Instance.SearchDatabaseAssetsInPaths,
                SaveSystemSettings.Instance.IgnoreDatabaseAssetsInPaths
            );

            var objectsGuids = new List<(Object, string)>();

            // get also sub-assets
            foreach (var guid in filesGuids)
            {
                string objPath = AssetDatabase.GUIDToAssetPath(guid);
                var obj = AssetDatabase.LoadAssetAtPath<Object>(objPath);
                objectsGuids.Add((obj, guid));
                AddSubAssets(obj, guid, objectsGuids);
            }
            
            return objectsGuids;
        }

        public static void AddSubAssets(Object obj, string guid, List<(Object, string)> objectsGuids)
        {
            string objPath = AssetDatabase.GUIDToAssetPath(guid);
            var assetType = AssetDatabase.GetMainAssetTypeAtPath(objPath);

            var isSceneAsset = assetType == typeof(SceneAsset); // ignore scenes' sub-assets
            if (!isSceneAsset)
            {
                // add sub-assets representations (the ones seen as files in the Project window)
                var subAssetsArray = AssetDatabase.LoadAllAssetRepresentationsAtPath(objPath);
                foreach (var subAsset in subAssetsArray)
                {
                    var databaseId = GetSubAssetGuid(guid, subAsset);
                    objectsGuids.Add((subAsset, databaseId));
                }
                
                // add top-level components if it's a prefab
                if (obj is GameObject go && PrefabUtility.IsPartOfPrefabAsset(go))
                {
                    var components = go.GetComponents<Component>();
                    foreach (var component in components)
                    {
                        var databaseId = GetSubAssetGuid(guid, component);
                        objectsGuids.Add((component, databaseId));
                    }
                }
            }
        }

        private static string GetSubAssetGuid(string guid, Object subAsset)
        {
            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(subAsset, out _, out long localFileId);
            var databaseId = string.Concat(guid, "---", localFileId.ToString());
            return databaseId;
        }

        public static List<string> GetGuidsInPaths(List<string> searchPaths, List<string> ignorePaths)
        {
            // get assets
            var filesGuids = GetFilesGuids(searchPaths);

            // ignore assets
            var ignoreGuids = GetFilesGuids(ignorePaths);
            foreach (var ignoreGuid in ignoreGuids)
            {
                filesGuids.Remove(ignoreGuid);
            }
            return filesGuids;
        }

        public static List<string> GetFilesGuids(List<string> paths)
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
            filesGuids = filesGuids.Where(guid => !string.IsNullOrEmpty(guid));  // filter out invalid guids
            return filesGuids.ToList();
        }
    }
}