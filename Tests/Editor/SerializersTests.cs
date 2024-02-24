using System;
using System.Collections.Generic;
using NUnit.Framework;
using SaveSystem.GuidsResolve;
using SaveSystem.Serializers;
using UnityEngine;
using Utils.Serializables;
using Object = UnityEngine.Object;

namespace SaveSystem.Tests.Editor
{
    public class SerializersTests
    {
        [TestCaseSource(typeof(TestsUtils), nameof(TestsUtils.Serializers))]
        public void WhenSerializeAnObject_ItsDeserializationHasTheProperValues_Test(ISerializer serializer)
        {
            // arrange
            var expected = ScriptableObject.CreateInstance<PersistentObject>();
            expected.I = 42;
            expected.S = "Hello serializer";
            expected.Lb = new List<bool> { true, false, true };
            // var guidResolver = new RandomGuidResolver();
            var guidResolver = new GuidsDatabase();
            
            // act
            var bytes = serializer.Serialize(expected, guidResolver);
            var deserialized = ScriptableObject.CreateInstance<PersistentObject>();
            serializer.Deserialize(bytes, deserialized, guidResolver);

            // assert
            Assert.AreEqual(expected.I, deserialized.I);
            Assert.AreEqual(expected.S, deserialized.S);
            Assert.AreEqual(expected.Lb, deserialized.Lb);
        }
        
        [TestCaseSource(typeof(TestsUtils), nameof(TestsUtils.Serializers))]
        public void WhenSerializeAFieldThatDependsOnISerializationCallbackReceiverInterface_ItsDeserializationHasTheProperValues_Test(ISerializer serializer)
        {
            // arrange
            var expected = ScriptableObject.CreateInstance<PersistentObject>();
            expected.Dictionary = new SerializableDictionary<string, int>
            {
                {"a", 1},
                {"b", -1},
                {"c", 123},
            };
            var guidResolver = new GuidsDatabase();
            
            // act
            var bytes = serializer.Serialize(expected, guidResolver);
            var deserialized = ScriptableObject.CreateInstance<PersistentObject>();
            serializer.Deserialize(bytes, deserialized, guidResolver);

            // assert
            Assert.AreEqual(expected.Dictionary.Count, deserialized.Dictionary.Count);
            foreach (var (key, value) in expected.Dictionary)
            {
                Assert.IsTrue(deserialized.Dictionary.ContainsKey(key));
                Assert.AreEqual(value, deserialized.Dictionary[key]);
            }
        }
        
        [TestCaseSource(typeof(TestsUtils), nameof(TestsUtils.Serializers))]
        public void WhenSerializeAnObjectReferenceThatItsNotInGuidsDatabase_ThrowException_Test(ISerializer serializer)
        {
            // this behaviour is not wanted anymore
            
            // // arrange
            // var data = ScriptableObject.CreateInstance<PersistentObject>();
            // data.Reference = ScriptableObject.CreateInstance<PersistentObject>();
            // var guidResolver = new GuidsDatabase();
            //
            // // act and assert
            // Assert.Throws<ArgumentException>(() => serializer.Serialize(data, guidResolver));
        }
        
        [TestCaseSource(typeof(TestsUtils), nameof(TestsUtils.Serializers))]
        public void WhenDeserializeAnObjectReferenceThatItsNotInGuidsDatabase_ThrowException_Test(ISerializer serializer)
        {
            // this behaviour is not wanted anymore
            
            // // arrange
            // var data = ScriptableObject.CreateInstance<PersistentObject>();
            // data.Reference = ScriptableObject.CreateInstance<PersistentObject>();
            //
            // // populate database to get a valid serialization
            // var guidResolver = new GuidsDatabase();
            // var databaseData = new List<(Object, string)>
            // {
            //     (data.Reference, "arbitrary guid")  
            // };
            // guidResolver.PopulateDatabase(databaseData);
            // var bytes = serializer.Serialize(data, guidResolver);
            //
            // // get an empty guids database to simulate a deserialization without the proper guids
            // var emptyResolver = new GuidsDatabase();
            //
            // // act and assert
            // Assert.Throws<ArgumentException>(() => serializer.Deserialize(bytes, data, emptyResolver));
        }
    }

    public class PersistentObject : ScriptableObject
    {
        public string S;
        public int I;
        public List<bool> Lb;
        public PersistentObject Reference;
        public SerializableDictionary<string, int> Dictionary;
    }
}