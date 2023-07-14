using System.Collections.Generic;
using SaveSystem.Serializers;
using UnityEngine;
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
        [SerializeField] private bool _populateDatabaseBeforeEnterPlayMode;

        [Header("Listeners")]
        [SerializeField] private SaveLoadBroadcaster _broadcaster = new SaveLoadBroadcaster();
        
        [Header("Debug")]
        [SerializeField] private bool _debugLogging;
        
        public ISerializer Serializer => _serializer;

        public bool EncryptData => _encryptData;

        public List<string> SearchDatabaseAssetsInPaths => _searchDatabaseAssetsInPaths;

        public List<string> IgnoreDatabaseAssetsInPaths => _ignoreDatabaseAssetsInPaths;

        public bool PopulateDatabaseBeforeEnterPlayMode => _populateDatabaseBeforeEnterPlayMode;

        public SaveLoadBroadcaster Broadcaster => _broadcaster;

        public bool DebugLogging => _debugLogging;
    }
}