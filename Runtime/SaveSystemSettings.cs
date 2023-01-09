using System.Collections.Generic;
using SaveSystem.Serializers;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Utils;

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
    }
}