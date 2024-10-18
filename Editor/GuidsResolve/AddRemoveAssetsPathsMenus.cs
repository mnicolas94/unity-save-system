using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace SaveSystem.Editor.GuidsResolve
{
    public static class AddRemoveAssetsPathsMenus
    {
        [MenuItem("Assets/Facticus/SaveSystem/Search assets in paths", false, 1000)]
        public static void IncludeAsset()
        {
            AddPaths(SaveSystemSettings.Instance.SearchDatabaseAssetsInPaths);
            EditorUtility.SetDirty(SaveSystemSettings.Instance);
        }
        
        [MenuItem("Assets/Facticus/SaveSystem/Ignore assets in paths", false, 1000)]
        public static void IgnoreAsset()
        {
            AddPaths(SaveSystemSettings.Instance.IgnoreDatabaseAssetsInPaths);
            EditorUtility.SetDirty(SaveSystemSettings.Instance);
        }
        
        [MenuItem("Assets/Facticus/SaveSystem/Stop searching assets in paths", false, 1000)]
        public static void RemoveIncludeAsset()
        {
            RemovePaths(SaveSystemSettings.Instance.SearchDatabaseAssetsInPaths);
            EditorUtility.SetDirty(SaveSystemSettings.Instance);
        }
        
        [MenuItem("Assets/Facticus/SaveSystem/Stop ignoring assets in paths", false, 1000)]
        public static void RemoveIgnoreAsset()
        {
            RemovePaths(SaveSystemSettings.Instance.IgnoreDatabaseAssetsInPaths);
            EditorUtility.SetDirty(SaveSystemSettings.Instance);
        }
        
        [MenuItem("Assets/Facticus/SaveSystem/Search assets in paths", true, 1000)]
        public static bool IncludeAssetMenuValidation()
        {
            return IncludeOrExcludeAssetMenuValidation(SaveSystemSettings.Instance.SearchDatabaseAssetsInPaths, true);
        }
        
        [MenuItem("Assets/Facticus/SaveSystem/Ignore assets in paths", true, 1000)]
        public static bool IgnoreAssetMenuValidation()
        {
            return IncludeOrExcludeAssetMenuValidation(SaveSystemSettings.Instance.IgnoreDatabaseAssetsInPaths, true);
        }
        
        [MenuItem("Assets/Facticus/SaveSystem/Stop searching assets in paths", true, 1000)]
        public static bool RemoveIncludeAssetMenuValidation()
        {
            return IncludeOrExcludeAssetMenuValidation(SaveSystemSettings.Instance.SearchDatabaseAssetsInPaths, false);
        }
        
        [MenuItem("Assets/Facticus/SaveSystem/Stop ignoring assets in paths", true, 1000)]
        public static bool RemoveIgnoreAssetMenuValidation()
        {
            return IncludeOrExcludeAssetMenuValidation(SaveSystemSettings.Instance.IgnoreDatabaseAssetsInPaths, false);
        }
        
        private static void AddPaths(List<string> paths)
        {
            var guids = Selection.assetGUIDs;
            var selectedPaths = guids.Select(AssetDatabase.GUIDToAssetPath).ToList();

            Undo.RecordObject(SaveSystemSettings.Instance, "Add save system search/ignore paths");
            foreach (var path in selectedPaths)
            {
                bool contains = paths.Contains(path);
                if (!contains)
                {
                    paths.Add(path);
                }
            }
        }
        
        private static void RemovePaths(List<string> paths)
        {
            var guids = Selection.assetGUIDs;
            var selectedPaths = guids.Select(AssetDatabase.GUIDToAssetPath).ToList();

            Undo.RecordObject(SaveSystemSettings.Instance, "Remove save system search paths");
            foreach (var path in selectedPaths)
            {
                bool contains = paths.Contains(path);
                if (contains)
                {
                    paths.Remove(path);
                }
            }
        }
        
        private static bool IncludeOrExcludeAssetMenuValidation(List<string> paths, bool wantToAdd)
        {
            var guids = Selection.assetGUIDs;
            var selectedPaths = guids.Select(AssetDatabase.GUIDToAssetPath);
            var anyNotIncluded = selectedPaths.Any(path => paths.Contains(path) ^ wantToAdd);
            return anyNotIncluded;
            // return true;
        }
    }
}