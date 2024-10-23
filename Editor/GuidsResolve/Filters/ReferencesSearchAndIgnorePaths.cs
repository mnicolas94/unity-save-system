using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SaveSystem.Editor.GuidsResolve.Filters
{
    [Serializable]
    public class ReferencesSearchAndIgnorePaths : IReferencesFilter
    {
        [SerializeField] private List<string> _searchDatabaseAssetsInFolder;
        [SerializeField] private List<string> _ignoreDatabaseAssetsInFolder;
        
        public void AddObjectsAndGuids(List<(Object, string)> objectsGuids)
        {
            var filesGuids = GetGuidsInPaths(
                _searchDatabaseAssetsInFolder,
                _ignoreDatabaseAssetsInFolder
            );

            // get also sub-assets
            foreach (var guid in filesGuids)
            {
                string objPath = AssetDatabase.GUIDToAssetPath(guid);
                var obj = AssetDatabase.LoadAssetAtPath<Object>(objPath);
                var globalId = GlobalObjectId.GetGlobalObjectIdSlow(obj).ToString();
                objectsGuids.Add((obj, globalId));
                AssetReferencesScanUtils.AddSubAssets(obj, objectsGuids);
            }
        }
        
        public List<string> GetGuidsInPaths(List<string> searchPaths, List<string> ignorePaths)
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