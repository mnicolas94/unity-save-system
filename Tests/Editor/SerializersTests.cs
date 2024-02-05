using System;
using System.Collections.Generic;
using NUnit.Framework;
using SaveSystem.GuidsResolve;
using SaveSystem.Serializers;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace SaveSystem.Tests.Editor
{
    public class SerializersTests
    {
        private const string UnitySerializerKey = "UnitySerializer";

        private readonly Dictionary<string, ISerializer> _serializers = new Dictionary<string, ISerializer>
        {
            { UnitySerializerKey, new UnitySerializer() },
        };
        
        [TestCase(UnitySerializerKey)]
        public void WhenSerializeAnObject_ItsDeserializationHasTheProperTheValues_Test(string serializerKey)
        {
            // arrange
            var serializer = _serializers[serializerKey];
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
    }

    public class PersistentObject : ScriptableObject
    {
        public string S;
        public int I;
        public List<bool> Lb;
        public PersistentObject _reference;
    }

    public class RandomGuidResolver : IGuidResolver
    {
        public bool ExistsGuid(string guid)
        {
            return true;
        }

        public bool ExistsObject(Object obj)
        {
            return true;
        }

        public string GetGuid(Object obj)
        {
            return Random.value.ToString();
        }

        public Object GetObject(string guid)
        {
            return null;
        }

        public bool TryGetGuid(Object obj, out string guid)
        {
            guid = GetGuid(obj);
            return true;
        }

        public bool TryGetObject(string guid, out Object obj)
        {
            obj = GetObject(guid);
            return true;
        }

        public void PopulateDatabase(List<(Object, string)> references)
        {
            
        }
    }
}