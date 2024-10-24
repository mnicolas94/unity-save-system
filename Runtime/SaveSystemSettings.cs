using System;
using System.Collections.Generic;
using SaveSystem.GuidsResolve;
using SaveSystem.Serializers;
using SaveSystem.Storages;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Utils;

namespace SaveSystem
{
    [CreateAssetMenu(fileName = "SaveSystemSettings", menuName = "Facticus/Save System/SaveSystemSettings")]
    public class SaveSystemSettings : ScriptableObjectSingleton<SaveSystemSettings>
    {
        [Header("Storage")]
        [SerializeReference, SubclassSelector] private IStorage _storage = new FilesStorage();
        public IStorage Storage => _storage;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField, HideInInspector,
         Tooltip("A profile allows to isolate contexts to save/load data. e.g. it can be used to " +
                 "implement saves slots. It is also used to isolate saves in the editor from saves " +
                 "in standalone builds. It wil be used to implement a backup profile feature in the future.")]
        private string _profile;
        public string Profile
        {
            get => _profile;
            set => _profile = value;
        }

        public static string SaveProfile
        {
            get => Instance.Profile;
            set => Instance.Profile = value;
        }

        [Header("Serialization")]
        [SerializeReference, SubclassSelector] private ISerializer _serializer;
        public ISerializer Serializer => _serializer;
        
        [SerializeField] private bool _encryptData;
        public bool EncryptData => _encryptData;

        [Header("Asset Guid's database")]
        [SerializeReference, SubclassSelector] private IGuidResolver _guidsResolver = new GuidsDatabase();
        public IGuidResolver GuidsResolver => _guidsResolver;

        [Obsolete("Deprecated since v0.6.0. Click Tools/Facticus/Save System/Migration/From <=0.5.x")]
        [Tooltip("Deprecated since v0.6.0. Click Tools/Facticus/Save System/Migration/From <=0.5.x")]
        [SerializeField] private List<string> _searchDatabaseAssetsInPaths;
        public List<string> SearchDatabaseAssetsInPaths => _searchDatabaseAssetsInPaths;
        
        [Obsolete("Deprecated since v0.6.0. Click Tools/Facticus/Save System/Migration/From <=0.5.x")]
        [Tooltip("Deprecated since v0.6.0. Click Tools/Facticus/Save System/Migration/From <=0.5.x")]
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

#if UNITY_EDITOR
        public static SaveSystemSettings Editor_GetOrCreate()
        {
            if (Instance == null)
            {
                var settings = CreateInstance<SaveSystemSettings>();
                var path = "Assets/SaveSettings.asset";
                AssetDatabase.CreateAsset(settings, path);
                Debug.Log($"Save System settings created at {path}", settings);
            }

            return Instance;
        }
#endif
    }
}