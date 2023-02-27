using System;
using System.Collections.Generic;
using System.Linq;
using SaveSystem.Serializers;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Utils;
using Object = UnityEngine.Object;

namespace SaveSystem
{
    [CreateAssetMenu(fileName = "SaveSystemSettings", menuName = "Facticus/Save system/SaveSystemSettings")]
    public class SaveSystemSettings : ScriptableObjectSingleton<SaveSystemSettings>
    {
        [Header("Serialization")]
        [SerializeReference, SubclassSelector] private ISerializer _serializer;
        [SerializeField] private bool _encryptData;
        
        [Header("Asset Guid's database")]
        [SerializeField] private List<string> _searchDatabaseAssetsInPaths;
        [SerializeField] private List<string> _ignoreDatabaseAssetsInPaths;

        [Header("Persistents")]
        [SerializeField] private List<ScriptableObject> _persistentObjects;
        
        public ISerializer Serializer => _serializer;

        public bool EncryptData => _encryptData;

        public List<string> SearchDatabaseAssetsInPaths => _searchDatabaseAssetsInPaths;

        public List<string> IgnoreDatabaseAssetsInPaths => _ignoreDatabaseAssetsInPaths;

        public List<ScriptableObject> PersistentObjects => _persistentObjects;

        public List<(Object obj, string guid)> GetDataObjectsAndGuids()
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
        
        private List<string> GetFilesGuids(List<string> paths)
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