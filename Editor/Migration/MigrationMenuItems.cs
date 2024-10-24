using System.Linq;
using SaveSystem.Editor.GuidsResolve;
using SaveSystem.Editor.GuidsResolve.Filters;
using UnityEditor;
using UnityEngine;

namespace SaveSystem.Editor.Migration
{
    public static class MigrationMenuItems
    {
        [MenuItem("Tools/Facticus/Save System/Migration/From <=0.5.x")]
        public static void MigrateFrom05x()
        {
            var settings = SaveSystemSettings.Editor_GetOrCreate();
            var filterStorage = AssetReferencesFilterStorage.GetOrCreate();

            Undo.RecordObject(filterStorage, "SaveSystem: Migrate from <=0.5.x");
            
            var filter = filterStorage.GetOrAddFilter<ReferencesSearchAndIgnore>();
            var addObjects = settings.SearchDatabaseAssetsInPaths.Select(AssetDatabase.LoadAssetAtPath<Object>);
            var ignoreObjects = settings.IgnoreDatabaseAssetsInPaths.Select(AssetDatabase.LoadAssetAtPath<Object>);

            // update assets to add
            foreach (var obj in addObjects)
            {
                if (obj == null) continue;
                filter.AddAsset(obj);
            }

            // update assets to ignore
            foreach (var obj in ignoreObjects)
            {
                if (obj == null) continue;
                filter.IgnoreAsset(obj);
            }
            
            EditorUtility.SetDirty(filterStorage);
            EditorGUIUtility.PingObject(filterStorage);
            
            Debug.Log("Migration was successful!");
        }
    }
}