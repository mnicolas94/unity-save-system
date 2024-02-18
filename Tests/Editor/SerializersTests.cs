using System;
using System.Collections.Generic;
using NUnit.Framework;
using SaveSystem.GuidsResolve;
using UnityEngine;
using Utils.Serializables;

namespace SaveSystem.Tests.Editor
{
    public class SerializersTests
    {
        [TestCase(TestsUtils.UnitySerializerBinaryKey)]
        [TestCase(TestsUtils.UnitySerializerJsonKey)]
        public void WhenSerializeAnObject_ItsDeserializationHasTheProperValues_Test(string serializerKey)
        {
            // arrange
            var serializer = TestsUtils.Serializers[serializerKey];
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
        
        [TestCase(TestsUtils.UnitySerializerBinaryKey)]
        [TestCase(TestsUtils.UnitySerializerJsonKey)]
        public void WhenSerializeAFieldThatDependsOnISerializationCallbackReceiverInterface_ItsDeserializationHasTheProperValues_Test(string serializerKey)
        {
            // arrange
            var serializer = TestsUtils.Serializers[serializerKey];
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
    }

    public class PersistentObject : ScriptableObject
    {
        public string S;
        public int I;
        public List<bool> Lb;
        public PersistentObject _reference;
        public SerializableDictionary<string, int> Dictionary;
    }
}