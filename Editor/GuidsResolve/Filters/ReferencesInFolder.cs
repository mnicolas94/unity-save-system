using System;
using System.Collections.Generic;
using System.Linq;
using SaveSystem.GuidsResolve;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SaveSystem.Editor.GuidsResolve.Filters
{
    [Serializable]
    public class ReferencesInFolder : IReferencesFilter
    {
        [SerializeField] private List<string> _searchDatabaseAssetsInFolder;
        [SerializeField] private List<string> _ignoreDatabaseAssetsInFolder;
        [SerializeField] private List<DefaultAsset> _searchDatabaseAssetsInFolderAsset;
        [SerializeField] private List<DefaultAsset> _ignoreDatabaseAssetsInFolderAsset;
        [SerializeField] private List<SerializableGlobalObjectId> _addAssets;
        [SerializeField] private List<SerializableGlobalObjectId> _ignoreAssets;
        
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
                objectsGuids.Add((obj, guid));
                AssetReferencesScanUtils.AddSubAssets(obj, guid, objectsGuids);
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