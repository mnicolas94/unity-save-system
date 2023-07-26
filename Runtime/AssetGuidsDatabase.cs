using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using Utils;
using Utils.Serializables;
using Object = UnityEngine.Object;

namespace SaveSystem
{
    [Serializable]
    public class AssetToGuidDictionary : SerializableDictionary<Object, string>{}
    [Serializable]
    public class GuidToAssetDictionary : SerializableDictionary<string, Object>{}
    
    [CreateAssetMenu(fileName = "AssetGuidsDatabase", menuName = "Facticus/Save system/AssetGuidsDatabase", order = 0)]
    public class AssetGuidsDatabase : ScriptableObjectSingleton<AssetGuidsDatabase>
    {
        [SerializeField] private List<Object> _assets = new List<Object>();
        [SerializeField] private List<string> _guids = new List<string>();
        private Dictionary<Object, string> _assetToGuid;
        private Dictionary<string, Object> _guidToAsset;

        public ReadOnlyCollection<Object> Assets => _assets.AsReadOnly();
        public ReadOnlyCollection<string> Guids => _guids.AsReadOnly();

        protected override void OnEnableCallback()
        {
            InitializeLookupDictionaries();
        }

        private void InitializeLookupDictionaries()
        {
            _assetToGuid = new AssetToGuidDictionary();
            _guidToAsset = new GuidToAssetDictionary();
            for (int i = 0; i < _assets.Count; i++)
            {
                var asset = _assets[i];
                var guid = _guids[i];
                if (asset == null)
                    continue;
                
                _assetToGuid[asset] = guid;
                _guidToAsset[guid] = asset;
            }
        }

        public bool ExistsGuid(string guid)
        {
            return _guidToAsset.ContainsKey(guid);
        }
        
        public bool ExistsObject(Object obj)
        {
            return _assetToGuid.ContainsKey(obj);
        }
        
        public string GetGuid(Object obj)
        {
            if (_assetToGuid.ContainsKey(obj))
            {
                return _assetToGuid[obj];
            }

            throw new ArgumentException($"Object not present in database: {obj}. _assetToGuid count {_assetToGuid.Count}");
        }
        
        public Object GetObject(string guid)
        {
            if (_guidToAsset.ContainsKey(guid))
            {
                return _guidToAsset[guid];
            }

            throw new ArgumentException($"Guid not present in database: {guid}");
        }
        
        public string TryGetGuid(Object obj, out bool exists)
        {
            if (_assetToGuid.ContainsKey(obj))
            {
                exists = true;
                return _assetToGuid[obj];
            }

            exists = false;
            return null;
        }
        
        public Object TryGetObject(string guid, out bool exists)
        {
            if (_guidToAsset.ContainsKey(guid))
            {
                exists = true;
                return _guidToAsset[guid];
            }

            exists = false;
            return null;
        }
        
        #if UNITY_EDITOR

        public static void PopulateDatabase(List<(Object, string)> references)
        {
            var database = Instance;
            database._assets.Clear();
            database._guids.Clear();
            foreach (var (obj, guid) in references)
            {
                database._assets.Add(obj);
                database._guids.Add(guid);
            }

            database.InitializeLookupDictionaries();
        }
        
        #endif
    }
}