﻿using System.Collections.Generic;
using System.Linq;
using SaveSystem.Editor.GuidsResolve.Filters;
using UnityEditor;
using UnityEngine;

namespace SaveSystem.Editor.GuidsResolve
{
    public static class AddRemoveAssetsPathsMenus
    {
        [MenuItem("Assets/Facticus/SaveSystem/Search assets in paths", false, 1000)]
        public static void IncludeAsset()
        {
            AddAssets(false);
        }
        
        [MenuItem("Assets/Facticus/SaveSystem/Ignore assets in paths", false, 1000)]
        public static void IgnoreAsset()
        {
            AddAssets(true);
        }
        
        [MenuItem("Assets/Facticus/SaveSystem/Stop searching assets in paths", false, 1000)]
        public static void RemoveIncludeAsset()
        {
            RemovePaths(false);
        }
        
        [MenuItem("Assets/Facticus/SaveSystem/Stop ignoring assets in paths", false, 1000)]
        public static void RemoveIgnoreAsset()
        {
            RemovePaths(true);
        }
        
        [MenuItem("Assets/Facticus/SaveSystem/Search assets in paths", true, 1000)]
        public static bool IncludeAssetMenuValidation()
        {
            return IncludeOrExcludeAssetMenuValidation(false, true);
        }
        
        [MenuItem("Assets/Facticus/SaveSystem/Ignore assets in paths", true, 1000)]
        public static bool IgnoreAssetMenuValidation()
        {
            return IncludeOrExcludeAssetMenuValidation(true, true);
        }
        
        [MenuItem("Assets/Facticus/SaveSystem/Stop searching assets in paths", true, 1000)]
        public static bool RemoveIncludeAssetMenuValidation()
        {
            return IncludeOrExcludeAssetMenuValidation(false, false);
        }
        
        [MenuItem("Assets/Facticus/SaveSystem/Stop ignoring assets in paths", true, 1000)]
        public static bool RemoveIgnoreAssetMenuValidation()
        {
            return IncludeOrExcludeAssetMenuValidation(true, false);
        }
        
        private static void AddAssets(bool asIgnore)
        {
            var selectedAssets = Selection.GetFiltered<Object>(SelectionMode.Assets);
            
            var filterStorage = AssetReferencesFilterStorage.GetOrCreate();
            Undo.RecordObject(filterStorage, "SaveSystem: update references filters");

            var filter = filterStorage.GetOrAddFilter<ReferencesSearchAndIgnore>();
            foreach (var asset in selectedAssets)
            {
                if (asIgnore)
                {
                    filter.IgnoreAsset(asset);
                }
                else
                {
                    filter.AddAsset(asset);
                }
            }
            
            EditorUtility.SetDirty(filterStorage);
        }
        
        private static void RemovePaths(bool asIgnore)
        {
            var selectedAssets = Selection.GetFiltered<Object>(SelectionMode.Assets);
            
            var filterStorage = AssetReferencesFilterStorage.GetOrCreate();
            Undo.RecordObject(filterStorage, "SaveSystem: update references filters");

            var filter = filterStorage.GetOrAddFilter<ReferencesSearchAndIgnore>();
            foreach (var asset in selectedAssets)
            {
                if (asIgnore)
                {
                    filter.StopIgnoringAsset(asset);
                }
                else
                {
                    filter.RemoveAsset(asset);
                }
            }
            
            EditorUtility.SetDirty(filterStorage);
        }
        
        private static bool IncludeOrExcludeAssetMenuValidation(bool asIgnore, bool wantToAdd)
        {
            // TODO: implement
            return true;
        }
    }
}