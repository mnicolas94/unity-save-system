using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SaveSystem.GuidsResolve.Legacy;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SaveSystem.GuidsResolve
{
    [Serializable]
    public class GuidsDatabase : IGuidResolver, ISerializationCallbackReceiver
    {
        [SerializeField] private List<Object> _assets = new ();
        [SerializeField] private List<string> _guids = new ();
        private Dictionary<Object, string> _assetToGuid = new ();
        private Dictionary<string, Object> _guidToAsset = new ();

        public ReadOnlyCollection<Object> Assets => _assets.AsReadOnly();
        public ReadOnlyCollection<string> Guids => _guids.AsReadOnly();

        // TODO optimize this so it does not happen on each deserialization
        private void InitializeLookupDictionaries()
        {
            _assetToGuid.Clear();
            _guidToAsset.Clear();
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
            if (TryGetGuid(obj, out var guid))
            {
                return guid;
            }

            throw new ArgumentException($"Object not present in database: {obj}. _assetToGuid count {_assetToGuid.Count}");
        }
        
        public Object GetObject(string guid)
        {
            if (TryGetObject(guid, out var obj))
            {
                return obj;
            }

            throw new ArgumentException($"Guid not present in database: {guid}");
        }
        
        public bool TryGetGuid(Object obj, out string guid)
        {
            if (ExistsObject(obj))
            {
                guid = _assetToGuid[obj];
                return true;
            }

            guid = null;
            return false;
        }
        
        public bool TryGetObject(string guid, out Object obj)
        {
            if (ExistsGuid(guid))
            {
                obj = _guidToAsset[guid];
                return true;
            }

            return LegacyGuidsDeserialization.TryGetObjectFromOldGuid(guid, _guidToAsset, out obj);
        }
        
        
        public void PopulateDatabase(List<(Object, string)> references)
        {
            _assets.Clear();
            _guids.Clear();
            foreach (var (obj, guid) in references)
            {
                _assets.Add(obj);
                _guids.Add(guid);
            }

            InitializeLookupDictionaries();
        }

        public void OnBeforeSerialize()
        {
            
        }

        public void OnAfterDeserialize()
        {
            // TODO optimize this so it does not happen on each deserialization
            InitializeLookupDictionaries();
        }
    }
}