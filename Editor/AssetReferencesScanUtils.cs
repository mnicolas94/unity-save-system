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

            // get objects
            var objectsGuids = filesGuids.SelectMany(guid =>
            {
                string objPath = AssetDatabase.GUIDToAssetPath(guid);
                var assetType = AssetDatabase.GetMainAssetTypeAtPath(objPath);
                var obj = AssetDatabase.LoadAssetAtPath<Object>(objPath);
                var objectsGuids = new List<(Object, string)>
                {
                    (obj, guid)
                };
                
                var isSceneAsset = assetType == typeof(SceneAsset);
                if (isSceneAsset)
                {
                    return objectsGuids;
                }
                else
                {
                    var subAssetsArray = AssetDatabase.LoadAllAssetRepresentationsAtPath(objPath);
                    var subAssets = new List<Object>(subAssetsArray);
                    
                    // also save top-level components in prefabs
                    var isPrefab = objPath.EndsWith(".prefab");
                    if (isPrefab && obj is GameObject go)
                    {
                        var components = go.GetComponents<Component>();
                        subAssets.AddRange(components);
                    }
                    
                    // save sub-assets with their local file id
                    foreach (var subAsset in subAssets)
                    {
                        AssetDatabase.TryGetGUIDAndLocalFileIdentifier(obj, out _, out long localFileId);
                        var databaseId = string.Concat(guid, "---", localFileId.ToString());
                        objectsGuids.Add((subAsset, databaseId));
                    }

                    return objectsGuids;
                }
            });
            return objectsGuids.ToList();
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
            return filesGuids.ToList();
        }
    }
}