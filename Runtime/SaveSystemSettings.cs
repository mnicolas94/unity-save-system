using SaveSystem.Serializers;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Utils;

namespace SaveSystem
{
    [CreateAssetMenu(fileName = "SaveSystemSettings", menuName = "SaveSystem/SaveSystemSettings")]
    public class SaveSystemSettings : ScriptableObjectSingleton<SaveSystemSettings>
    {
        [SerializeField] private AssetLabelReference _persistentsLabel;
        [SerializeReference, SubclassSelector] private ISerializer _serializer;
        [SerializeField] private bool _encryptData;
        [SerializeField] private string[] _searchDatabaseAssetsInPaths;
        
        public AssetLabelReference PersistentsLabel => _persistentsLabel;

        public ISerializer Serializer => _serializer;

        public bool EncryptData => _encryptData;

        public string[] SearchDatabaseAssetsInPaths => _searchDatabaseAssetsInPaths;
    }
}