using System.Collections;
using System.Threading.Tasks;
using NUnit.Framework;
using SaveSystem.Utilities;
using UnityEngine;
using UnityEngine.TestTools;

namespace SaveSystem.Tests.Editor
{
    public class SaveGroupTests
    {
        [UnityTest]
        public IEnumerator WhenSaveGroupIsSaved_NestedObjectsAreSaved()
        {
            // arrange
            var saveGroup = ScriptableObject.CreateInstance<SaveGroup>();
            var persistent = PersistentAdapter.Create();
            saveGroup.AddToGroup(persistent);

            // act
            yield return TestsUtils.RunTaskAsCoroutine(saveGroup.Save());
            
            // assert
            Assert.IsTrue(persistent.WasSaveCalled);
        }
        
        [UnityTest]
        public IEnumerator WhenSaveGroupIsLoadedSaved_NestedObjectsAreLoaded()
        {
            // arrange
            var saveGroup = ScriptableObject.CreateInstance<SaveGroup>();
            var persistent = PersistentAdapter.Create();
            saveGroup.AddToGroup(persistent);

            // act
            yield return TestsUtils.RunTaskAsCoroutine(saveGroup.Load());
            
            // assert
            Assert.IsTrue(persistent.WasLoadCalled);
        }
        
        [UnityTest]
        public IEnumerator WhenSaveGroupIsLoadedOrCreated_NestedObjectsAreLoadedOrCreated()
        {
            // arrange
            var saveGroup = ScriptableObject.CreateInstance<SaveGroup>();
            var persistent = PersistentAdapter.Create();
            saveGroup.AddToGroup(persistent);

            // act
            yield return TestsUtils.RunTaskAsCoroutine(saveGroup.LoadOrCreate());
            
            // assert
            Assert.IsTrue(persistent.WasLoadOrCreateCalled);
        }
        
        [UnityTest]
        public IEnumerator WhenSaveGroupIsDeleted_NestedObjectsDataAreDeleted()
        {
            // arrange
            var saveGroup = ScriptableObject.CreateInstance<SaveGroup>();
            var persistent = PersistentAdapter.Create();
            saveGroup.AddToGroup(persistent);

            // act
            yield return TestsUtils.RunTaskAsCoroutine(saveGroup.DeleteData());
            
            // assert
            Assert.IsTrue(persistent.WasDeleteDataCalled);
        }
        
        [UnityTest]
        public IEnumerator WhenSaveGroupIsSaved_NestedSaveGroupsAreSaved()
        {
            // arrange
            var saveGroup = ScriptableObject.CreateInstance<SaveGroup>();
            var nestedSaveGroupA = ScriptableObject.CreateInstance<SaveGroup>();
            var nestedSaveGroupB = ScriptableObject.CreateInstance<SaveGroup>();
            var nestedSaveGroupCInsideA = ScriptableObject.CreateInstance<SaveGroup>();
            var persistentA = PersistentAdapter.Create();
            var persistentB = PersistentAdapter.Create();
            var persistentC = PersistentAdapter.Create();
            
            saveGroup.AddToGroup(nestedSaveGroupA);
            saveGroup.AddToGroup(nestedSaveGroupB);
            nestedSaveGroupA.AddToGroup(nestedSaveGroupCInsideA);
            
            nestedSaveGroupA.AddToGroup(persistentA);
            nestedSaveGroupB.AddToGroup(persistentB);
            nestedSaveGroupCInsideA.AddToGroup(persistentC);

            // act
            yield return TestsUtils.RunTaskAsCoroutine(saveGroup.Save());
            
            // assert
            Assert.IsTrue(persistentA.WasSaveCalled);
            Assert.IsTrue(persistentB.WasSaveCalled);
            Assert.IsTrue(persistentC.WasSaveCalled);
        }
    }

    public class PersistentAdapter : ScriptableObject, IPersistentAdapter
    {
        public bool WasSaveCalled;
        public bool WasLoadCalled;
        public bool WasLoadOrCreateCalled;
        public bool WasIsSavedCalled;
        public bool WasDeleteDataCalled;

        public static PersistentAdapter Create()
        {
            return CreateInstance<PersistentAdapter>();
        }
        
        public async Task Save()
        {
            WasSaveCalled = true;
            await Task.Yield();
        }

        public async Task<LoadReport> Load()
        {
            WasLoadCalled = true;
            await Task.Yield();
            return default;
        }

        public async Task<LoadReport> LoadOrCreate()
        {
            WasLoadOrCreateCalled = true;
            await Task.Yield();
            return default;
        }

        public async Task<bool> IsSaved()
        {
            WasIsSavedCalled = true;
            await Task.Yield();
            return default;
        }

        public async Task DeleteData()
        {
            WasDeleteDataCalled = true;
            await Task.Yield();
        }
    }
}