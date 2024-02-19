using System.Collections.Generic;
using SaveSystem.GuidsResolve;
using SaveSystem.Serializers;
using UnityEngine;
using Utils;

namespace SaveSystem
{
    [CreateAssetMenu(fileName = "SaveSystemSettings", menuName = "Facticus/Save System/SaveSystemSettings")]
    public class SaveSystemSettings : ScriptableObjectSingleton<SaveSystemSettings>
    {
        [Header("Serialization")]
        [SerializeReference, SubclassSelector] private ISerializer _serializer;
        public ISerializer Serializer => _serializer;
        
        [SerializeField] private bool _encryptData;
        public bool EncryptData => _encryptData;

        [Header("Asset Guid's database")]
        [SerializeReference, SubclassSelector] private IGuidResolver _guidsResolver = new GuidsDatabase();
        public IGuidResolver GuidsResolver => _guidsResolver;

        [SerializeField] private List<string> _searchDatabaseAssetsInPaths;
        public List<string> SearchDatabaseAssetsInPaths => _searchDatabaseAssetsInPaths;
        
        [SerializeField] private List<string> _ignoreDatabaseAssetsInPaths;
        public List<string> IgnoreDatabaseAssetsInPaths => _ignoreDatabaseAssetsInPaths;
        
        [SerializeField] private bool _populateDatabaseBeforeEnterPlayMode;
        public bool PopulateDatabaseBeforeEnterPlayMode => _populateDatabaseBeforeEnterPlayMode;

        [Header("Listeners")]
        [SerializeField] private SaveLoadBroadcaster _broadcaster = new SaveLoadBroadcaster();
        public SaveLoadBroadcaster Broadcaster => _broadcaster;
        
        [Header("Debug")]
        [SerializeField] private bool _debugLogging;
        public bool DebugLogging => _debugLogging;
    }
}