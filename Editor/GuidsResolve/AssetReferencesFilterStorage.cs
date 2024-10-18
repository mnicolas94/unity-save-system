using System;
using System.Collections.Generic;
using System.IO;
using SaveSystem.Editor.GuidsResolve.Filters;
using UnityEditor;
using UnityEngine;
using Utils.Editor;
using Object = UnityEngine.Object;

namespace SaveSystem.Editor.GuidsResolve
{
    [CreateAssetMenu(fileName = "Filters", menuName = "Facticus/Save System/Filters")]
    public class AssetReferencesFilterStorage : ScriptableObjectSingleton<AssetReferencesFilterStorage>
    {
        [SerializeReference, SubclassSelector] private List<IReferencesFilter> _filters;

        public List<(Object, string)> GetObjectsAndGuids()
        {
            var objectsGuids = new List<(Object, string)>();

            foreach (var filter in _filters)
            {
                filter.AddObjectsAndGuids(objectsGuids);
            }
            
            return objectsGuids;
        }
        
        public T GetOrAddFilter<T>() where T : IReferencesFilter
        {
            foreach (var filter in _filters)
            {
                if (filter is T filterOfType)
                {
                    return filterOfType;
                }
            }

            var newFilter = Activator.CreateInstance<T>();
            _filters.Add(newFilter);
            EditorUtility.SetDirty(this);
            return newFilter;
        }
            
        public static AssetReferencesFilterStorage GetOrCreate()
        {
            if (Instance == null)
            {
                // create directory
                var settings = SaveSystemSettings.Editor_GetOrCreate();
                var settingsPath = AssetDatabase.GetAssetPath(settings);
                var settingsDirectory = Directory.GetParent(settingsPath);
                var storageDirectory = Path.Combine(settingsDirectory.Name, "Editor");
                Directory.CreateDirectory(storageDirectory);
                AssetDatabase.Refresh();

                // create asset
                var storage = CreateInstance<AssetReferencesFilterStorage>();
                var path = Path.Combine(storageDirectory, "AssetReferencesFilterStorage.asset");
                AssetDatabase.CreateAsset(storage, path);
            }

            return Instance;
        }
    }
}