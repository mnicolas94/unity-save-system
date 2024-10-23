using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SaveSystem.Editor.GuidsResolve
{
    public static class AssetReferencesScanUtils
    {
        public static List<(Object, string)> GetDataObjectsAndGuids()
        {
            var storage = AssetReferencesFilterStorage.GetOrCreate();
            var objectsGuids = storage.GetObjectsAndGuids();
            return objectsGuids;
        }
        
        public static void AddSubAssets(Object obj, List<(Object, string)> objectsGuids)
        {
            var objPath = AssetDatabase.GetAssetPath(obj);
            var assetType = AssetDatabase.GetMainAssetTypeAtPath(objPath);

            var isSceneAsset = assetType == typeof(SceneAsset); // ignore scenes' sub-assets
            if (!isSceneAsset)
            {
                // add sub-assets representations (the ones seen as files in the Project window)
                var subAssetsArray = AssetDatabase.LoadAllAssetRepresentationsAtPath(objPath);
                var subAssetsIds = new GlobalObjectId[subAssetsArray.Length];
                GlobalObjectId.GetGlobalObjectIdsSlow(subAssetsArray, subAssetsIds);
                for (int i = 0; i < subAssetsArray.Length; i++)
                {
                    var subAsset = subAssetsArray[i];
                    var id = subAssetsIds[i].ToString();
                    objectsGuids.Add((subAsset, id));
                }
                
                // add top-level components if it's a prefab
                if (obj is GameObject go && PrefabUtility.IsPartOfPrefabAsset(go))
                {
                    var components = go.GetComponents<Component>();
                    var componentsIds = new GlobalObjectId[components.Length];
                    GlobalObjectId.GetGlobalObjectIdsSlow(components, componentsIds);
                    for (int i = 0; i < components.Length; i++)
                    {
                        var component = components[i];
                        var id = componentsIds[i].ToString();
                        objectsGuids.Add((component, id));
                    }
                }
            }
        }

        
        [Obsolete()]
        private static string GetSubAssetGuid(string guid, Object subAsset)
        {
            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(subAsset, out _, out long localFileId);
            var databaseId = string.Concat(guid, "---", localFileId.ToString());
            return databaseId;
        }
    }
}