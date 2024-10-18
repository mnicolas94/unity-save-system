using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SaveSystem.Editor;
using SaveSystem.Editor.GuidsResolve;
using UnityEditor;
using UnityEngine;

namespace SaveSystem.Tests.Editor
{
    public class AssetReferencesScanTests
    {
        [Test]
        public void WhenScanPrefab_OnlyTopLevelComponentsAreStored()
        {
            // arrange
            var prefab = TestsReferences.Instance.prefab;
            var path = AssetDatabase.GetAssetPath(prefab);
            var guid = AssetDatabase.AssetPathToGUID(path);

            // act
            var objsGuids = new List<(Object, string)>();
            AssetReferencesScanUtils.AddSubAssets(prefab, guid, objsGuids);

            // assert
            var components = prefab.GetComponents<Component>();
            Assert.AreEqual(components.Length, objsGuids.Count);
            foreach (var (obj, _) in objsGuids)
            {
                Assert.Contains(obj, components);
            }
        }
        
        [Test]
        public void WhenScanAssetWithSubAssets_AllIdsAreUnique()
        {
            // arrange
            var prefab = TestsReferences.Instance.persistent;
            var path = AssetDatabase.GetAssetPath(prefab);
            var guid = AssetDatabase.AssetPathToGUID(path);

            // act
            var objsGuids = new List<(Object, string)>();
            AssetReferencesScanUtils.AddSubAssets(prefab, guid, objsGuids);

            // assert
            var onlyGuids = objsGuids.Select(o => o.Item2).ToList();
            Assert.AreEqual(onlyGuids.Distinct().Count(), onlyGuids.Count);
        }
    }
}