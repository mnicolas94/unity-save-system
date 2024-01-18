using System;
using System.Collections.Generic;
using NUnit.Framework;
using SaveSystem.Serializers;
using UnityEngine;

namespace SaveSystem.Tests.Editor
{
    public class SerializersTests
    {
        private const string UnitySerializerKey = "UnitySerializer";
        private const string OdinSerializerKey = "OdinSerializer";

        private readonly Dictionary<string, ISerializer> _serializers = new Dictionary<string, ISerializer>
        {
            { UnitySerializerKey, new UnitySerializer() },
            { OdinSerializerKey, new OdinPersistentSerializer() },
        };
        
        [TestCase(UnitySerializerKey)]
        [TestCase(OdinSerializerKey)]
        public void WhenSerializeAnObject_ItsDeserializationHasTheProperTheValues_Test(string serializerKey)
        {
            // arrange
            var serializer = _serializers[serializerKey];
            var expected = ScriptableObject.CreateInstance<PersistentObject>();
            expected.I = 42;
            expected.S = "Hello serializer";
            expected.Lb = new List<bool> { true, false, true };
                
            // act
            var bytes = serializer.Serialize(expected, null);
            var deserialized = ScriptableObject.CreateInstance<PersistentObject>();
            serializer.Deserialize(bytes, deserialized, null);

            // assert
            Assert.AreEqual(expected.I, deserialized.I);
            Assert.AreEqual(expected.S, deserialized.S);
            Assert.AreEqual(expected.Lb, deserialized.Lb);
        }
    }

    public class PersistentObject : ScriptableObject
    {
        public string S;
        public int I;
        public List<bool> Lb;
        public PersistentObject _reference;
    }
}