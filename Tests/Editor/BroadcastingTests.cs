using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEditor;
using UnityEngine.TestTools;

namespace SaveSystem.Tests.Editor
{
    public class BroadcastingTests
    {
        private static PersistentTest _data;
        private const string Path = "Assets/SaveSystem/Tests/Editor/Data/PersistentTest.asset";

        [SetUp]
        public void SetUp()
        {
            _data = AssetDatabase.LoadAssetAtPath<PersistentTest>(Path);
        }
        
        [UnityTest]
        public IEnumerator WhenRegisterSaveListenerAndSave_ListenerIsCalled()
        {
            // arrange
            bool saved = false;
            void OnSave()
            {
                saved = true;
            }
            _data.RegisterOnSaveListener(OnSave);
            
            // act
            yield return TestsUtils.RunTaskAsCoroutine(_data.Save());
            
            // assert
            Assert.IsTrue(saved);
            _data.UnregisterOnSaveListener(OnSave);
        }
        
        [UnityTest]
        public IEnumerator WhenUnregisterSaveListenerAndSave_ListenerIsNotCalled()
        {
            // arrange
            bool saved = false;
            void OnSave()
            {
                saved = true;
            }
            _data.RegisterOnSaveListener(OnSave);
            _data.UnregisterOnSaveListener(OnSave);
            
            // act
            yield return TestsUtils.RunTaskAsCoroutine(_data.Save());
            
            // assert
            Assert.IsFalse(saved);
        }
        
        [UnityTest]
        public IEnumerator WhenRegisterLoadListenerAndLoad_ListenerIsCalled()
        {
            // arrange
            bool loaded = false;
            void OnLoad()
            {
                loaded = true;
            }
            _data.RegisterOnLoadListener(OnLoad);
            
            // act
            yield return TestsUtils.RunTaskAsCoroutine(_data.Save());  // make sure it is saved beforehand
            yield return TestsUtils.RunTaskAsCoroutine(_data.Load());
            
            // assert
            Assert.IsTrue(loaded);
            _data.UnregisterOnLoadListener(OnLoad);
        }
        
        [UnityTest]
        public IEnumerator WhenUnregisterLoadListenerAndLoad_ListenerIsNotCalled()
        {
            // arrange
            bool loaded = false;
            void OnLoad()
            {
                loaded = true;
            }
            _data.RegisterOnLoadListener(OnLoad);
            _data.UnregisterOnLoadListener(OnLoad);
            
            // act
            yield return TestsUtils.RunTaskAsCoroutine(_data.Save());  // make sure it is saved beforehand
            yield return TestsUtils.RunTaskAsCoroutine(_data.Load());
            
            // assert
            Assert.IsFalse(loaded);
        }
        
        [UnityTest]
        public IEnumerator WhenWaitingForObjectToBeSaved_ItEndsAfterObjectIsSaved()
        {
            // arrange
            var timeout = 5000;
            var cts = new CancellationTokenSource(timeout);
            var ct = cts.Token;
            
            // act
            yield return TestsUtils.RunTaskAsCoroutine(
                Task.WhenAll(
                    SaveLoadBroadcaster.Instance.WaitToBeSavedAsync(_data, ct),
                    _data.Save()
                )
            );
            
            // assert
            Assert.IsFalse(ct.IsCancellationRequested);
        }
        
        [UnityTest]
        public IEnumerator WhenWaitingForObjectToBeLoaded_ItEndsAfterObjectIsLoaded()
        {
            // arrange
            var timeout = 5000;
            var cts = new CancellationTokenSource(timeout);
            var ct = cts.Token;
            
            // act
            yield return TestsUtils.RunTaskAsCoroutine(
                Task.WhenAll(
                    SaveLoadBroadcaster.Instance.WaitToBeLoadedAsync(_data, ct),
                    _data.Load()
                )
            );
            
            // assert
            Assert.IsFalse(ct.IsCancellationRequested);
        }
    }
}