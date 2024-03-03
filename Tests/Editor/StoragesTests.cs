using System.Collections;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using SaveSystem.Storages;
using UnityEngine.TestTools;

namespace SaveSystem.Tests.Editor
{
    public class StoragesTests
    {
        public static List<TestCaseData> Storages => TestsUtils.Storages.ConvertAll(
            storage => new TestCaseData(storage).Returns(null));
        
        [UnityTest, TestCaseSource(nameof(Storages))]
        public IEnumerator WhenReadDataAfterWritingIt_TheDataIsTheSame_Test(IStorage storage)
        {
            // arrange
            var profile = "profile";
            var key = "key";
            var data = Encoding.UTF8.GetBytes("Data");
            
            // act
            yield return TestsUtils.RunTaskAsCoroutine(storage.Write(profile, key, data));
            var readTask = storage.Read(profile, key);
            yield return TestsUtils.RunTaskAsCoroutine(readTask);
            var (success, result) = readTask.Result;
            
            // assert
            Assert.IsTrue(success);
            Assert.AreEqual(data, result);
            
            // tear down
            yield return TestsUtils.RunTaskAsCoroutine(storage.Delete(profile, key));
        }
        
        [UnityTest, TestCaseSource(nameof(Storages))]
        public IEnumerator WhenReadUnExistentData_TheReturnedBoolIsFalse_Test(IStorage storage)
        {
            // arrange
            var profile = "profile";
            var key = "key";
            // make sure it doesn't exist
            yield return TestsUtils.RunTaskAsCoroutine(storage.Delete(profile, key));
            
            // act
            var readTask = storage.Read(profile, key);
            yield return TestsUtils.RunTaskAsCoroutine(readTask);
            var (success, result) = readTask.Result;
            
            // assert
            Assert.IsFalse(success);
        }
        
        [UnityTest, TestCaseSource(nameof(Storages))]
        public IEnumerator WhenReadDataAfterDeletingIt_TheReturnedBoolIsFalse_Test(IStorage storage)
        {
            // arrange
            var profile = "profile";
            var key = "key";
            var data = Encoding.UTF8.GetBytes("Data");
            
            // act
            yield return TestsUtils.RunTaskAsCoroutine(storage.Write(profile, key, data));
            yield return TestsUtils.RunTaskAsCoroutine(storage.Delete(profile, key));
            var readTask = storage.Read(profile, key);
            yield return TestsUtils.RunTaskAsCoroutine(readTask);
            var (success, result) = readTask.Result;
            
            // assert
            Assert.IsFalse(success);
        }
    }
}