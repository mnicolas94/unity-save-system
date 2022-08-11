using System.Collections.Generic;
using SaveSystem.Serializers;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Utils;

namespace SaveSystem
{
    [CreateAssetMenu(fileName = "SaveSystemSettings", menuName = "SaveSystem/SaveSystemSettings")]
    public class SaveSystemSettings : ScriptableObjectSingleton<SaveSystemSettings>
    {
        [Header("Serialization")]
        [SerializeReference, SubclassSelector] private ISerializer _serializer;
        [SerializeField] private bool _encryptData;
        
        [Header("Asset Guid's database")]
        [SerializeField] private string[] _searchDatabaseAssetsInPaths;

        [Header("Persistents")]
        [SerializeField] private AssetLabelReference _persistentsLabel;
        [SerializeField] private List<ScriptableObject> _persistentObjects;
        
        public AssetLabelReference PersistentsLabel => _persistentsLabel;

        public ISerializer Serializer => _serializer;

        public bool EncryptData => _encryptData;

        public string[] SearchDatabaseAssetsInPaths => _searchDatabaseAssetsInPaths;

        public List<ScriptableObject> PersistentObjects => _persistentObjects;
    }
}