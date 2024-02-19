﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using Utils;
using Object = UnityEngine.Object;

namespace SaveSystem.GuidsResolve
{
    // [CreateAssetMenu(fileName = "AssetGuidsDatabase", menuName = "Facticus/Save system/AssetGuidsDatabase", order = 0)]
    public class AssetGuidsDatabase : ScriptableObjectSingleton<AssetGuidsDatabase>, IGuidResolver
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
            _assetToGuid = new Dictionary<Object, string>();
            _guidToAsset = new Dictionary<string, Object>();
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
        
        public bool TryGetGuid(Object obj, out string guid)
        {
            if (_assetToGuid.ContainsKey(obj))
            {
                guid = _assetToGuid[obj];
                return true;
            }

            guid = null;
            return false;
        }
        
        public bool TryGetObject(string guid, out Object obj)
        {
            if (_guidToAsset.ContainsKey(guid))
            {
                obj = _guidToAsset[guid];
                return true;
            }

            obj = null;
            return false;
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
    }
}