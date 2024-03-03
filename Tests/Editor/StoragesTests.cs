using System.Text;
using NUnit.Framework;
using SaveSystem.Storages;
using UnityEngine;

namespace SaveSystem.Tests.Editor
{
    public class StoragesTests
    {
        [TestCaseSource(typeof(TestsUtils), nameof(TestsUtils.Storages))]
        public void WhenReadDataAfterWritingIt_TheDataIsTheSame_Test(IStorage storage)
        {
            // arrange
            var profile = "profile";
            var key = "key";
            var data = Encoding.UTF8.GetBytes("Data");
            
            // act
            TestsUtils.RunAsyncMethodSync(storage.Write(profile, key, data));
            var (success, result) = TestsUtils.RunAsyncMethodSync(storage.Read(profile, key));
            
            // assert
            Assert.IsTrue(success);
            Assert.AreEqual(data, result);
            
            // tear down
            TestsUtils.RunAsyncMethodSync(storage.Delete(profile, key));
        }
        
        [TestCaseSource(typeof(TestsUtils), nameof(TestsUtils.Storages))]
        public void WhenReadUnExistentData_TheReturnedBoolIsFalse_Test(IStorage storage)
        {
            // arrange
            var profile = "profile";
            var key = "key";
            // make sure it doesn't exist
            TestsUtils.RunAsyncMethodSync(storage.Delete(profile, key));
            
            // act
            var (success, result) = TestsUtils.RunAsyncMethodSync(storage.Read(profile, key));
            
            // assert
            Assert.IsFalse(success);
        }
        
        [TestCaseSource(typeof(TestsUtils), nameof(TestsUtils.Storages))]
        public void WhenReadDataAfterDeletingIt_TheReturnedBoolIsFalse_Test(IStorage storage)
        {
            // arrange
            var profile = "profile";
            var key = "key";
            var data = Encoding.UTF8.GetBytes("Data");
            
            // act
            TestsUtils.RunAsyncMethodSync(storage.Write(profile, key, data));
            TestsUtils.RunAsyncMethodSync(storage.Delete(profile, key));
            var (success, result) = TestsUtils.RunAsyncMethodSync(storage.Read(profile, key));
            
            // assert
            Assert.AreEqual(false, success);
        }
    }
}