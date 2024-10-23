﻿using System;
using System.Collections.Generic;
using SaveSystem.GuidsResolve;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SaveSystem.Editor.GuidsResolve.Filters
{
    [Serializable]
    public class ReferencesSearchAndIgnore : IReferencesFilter
    {
        [SerializeField] private List<SerializableGlobalObjectId> _addAssets;
        [SerializeField] private List<SerializableGlobalObjectId> _ignoreAssets;
        
        public void AddObjectsAndGuids(List<(Object, string)> objectsGuids)
        {
            var objsGuids = GetGuidsInPaths(
                _addAssets,
                _ignoreAssets
            );

            // get also sub-assets
            foreach (var (obj, guid) in objsGuids)
            {
                objectsGuids.Add((obj, guid));
                AssetReferencesScanUtils.AddSubAssets(obj, objectsGuids);
            }
        }
        
        public List<(Object, string)> GetGuidsInPaths(List<SerializableGlobalObjectId> searchPaths,
            List<SerializableGlobalObjectId> ignorePaths)
        {
            // get assets
            var filesGuids = GetFilesGuids(searchPaths);

            // ignore assets
            var ignoreGuids = GetFilesGuids(ignorePaths);
            foreach (var ignoreGuid in ignoreGuids)
            {
                filesGuids.Remove(ignoreGuid);
            }
            return filesGuids;
        }
        
        public static List<(Object, string)> GetFilesGuids(List<SerializableGlobalObjectId> references)
        {
            var result = new List<(Object, string)>();
            
            // convert to GlobalObjectId
            var referencesCount = references.Count;
            var identifiers = new GlobalObjectId[referencesCount];
            var objects = new Object[referencesCount];
            for (int i = 0; i < referencesCount; i++)
            {
                var reference = references[i];
                identifiers[i] = reference.ToGlobalObjectId();
            }
            // use batch method to optimize conversion
            GlobalObjectId.GlobalObjectIdentifiersToObjectsSlow(identifiers, objects);
            
            // detect folders
            var folders = new List<string>();
            for (int i = 0; i < objects.Length; i++)
            {
                var obj = objects[i];
                if (obj == null) continue;
                
                if (obj is DefaultAsset)  // is a folder
                {
                    var path = AssetDatabase.GetAssetPath(obj);
                    folders.Add(path);
                }
                else  // is an asset
                {
                    var id = identifiers[i].ToString();
                    result.Add((obj, id));
                }
            }

            // add assets inside folders
            var guids = folders.Count > 0
                ? AssetDatabase.FindAssets("", folders.ToArray())
                : Array.Empty<string>();
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                
                // ignore sub-folders
                bool isFolder = AssetDatabase.IsValidFolder(path);
                if (isFolder) continue;

                var obj = AssetDatabase.LoadAssetAtPath<Object>(path);
                var id = GlobalObjectId.GetGlobalObjectIdSlow(obj).ToString();
                result.Add((obj, id));
            }
            
            return result;
        }
    }
}