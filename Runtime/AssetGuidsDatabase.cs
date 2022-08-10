using System;
using System.Collections.Generic;
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
    
    [CreateAssetMenu(fileName = "AssetGuidsDatabase", menuName = "SaveSystem/AssetGuidsDatabase", order = 0)]
    public class AssetGuidsDatabase : ScriptableObjectSingleton<AssetGuidsDatabase>
    {
        [SerializeField] private AssetToGuidDictionary _assetToGuid = new AssetToGuidDictionary();
        [SerializeField] private GuidToAssetDictionary _guidToAsset = new GuidToAssetDictionary();
        
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
            database._assetToGuid.Clear();
            database._guidToAsset.Clear();
            foreach (var (obj, guid) in references)
            {
                database._assetToGuid[obj] = guid;
                database._guidToAsset[guid] = obj;
            }
        }
        
        #endif
    }
}