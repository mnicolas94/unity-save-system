using System.Collections;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEditor;
using UnityEngine.TestTools;

namespace SaveSystem.Tests.Editor
{
    public class DoNotPersistTests
    {
        private static PersistentTest _data;
        private const string Path = "Assets/SaveSystem/Tests/Editor/PersistentTest.asset";

        [SetUp]
        public void SetUp()
        {
            _data = AssetDatabase.LoadAssetAtPath<PersistentTest>(Path);
        }

        [UnityTest]
        public IEnumerator WhenSavingAndLoadingAnObject_FieldsPersist()
        {
            // arrange
            _data.I = 45;
            
            // act
            yield return RunTaskAsCoroutine(_data.Save());
            _data.I = 46;
            yield return RunTaskAsCoroutine(_data.Load());
            
            // assert
            var expected = 45;
            Assert.AreEqual(expected, _data.I);
        }

        [UnityTest]
        public IEnumerator WhenSavingAndLoadingAnObjectWithADoNotPersistField_TheDoesNotPersist()
        {
            // arrange
            _data.S = "45";
            
            // act
            yield return RunTaskAsCoroutine(_data.Save());
            _data.S = "46";
            yield return RunTaskAsCoroutine(_data.Load());
            
            // assert
            var expected = "46";
            Assert.AreEqual(expected, _data.S);
        }

        private IEnumerator RunTaskAsCoroutine(Task task)
        {
            while (!task.IsCompleted)
            {
                yield return null;
            }
        }
    }
}