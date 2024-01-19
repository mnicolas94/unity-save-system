using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using SaveSystem.Serializers;
using Unity.PerformanceTesting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SaveSystem.Tests.Editor
{
    public class PerformanceTests
    {
        private const string UnitySerializerKey = "UnitySerializer";
        private const string OdinSerializerKey = "OdinSerializer";

        private readonly Dictionary<string, ISerializer> _serializers = new Dictionary<string, ISerializer>
        {
            { UnitySerializerKey, new UnitySerializer() },
            { OdinSerializerKey, new OdinPersistentSerializer() },
        };
        
        [Performance]
        [TestCase(OdinSerializerKey)]
        [TestCase(UnitySerializerKey)]
        public void Serialization_PerformanceTest(string serializerKey)
        {
            // arrange
            var serializer = _serializers[serializerKey];
            var obj = ScriptableObject.CreateInstance<SmallPersistentObject>();
            obj.Init();

            TestSerialization(serializer, obj, x => x.Randomize());
        }
        
        [Performance]
        [TestCase(OdinSerializerKey)]
        [TestCase(UnitySerializerKey)]
        public void SerializationLargeObject_PerformanceTest(string serializerKey)
        {
            // arrange
            var serializer = _serializers[serializerKey];
            var obj = ScriptableObject.CreateInstance<LargePersistentObject>();
            obj.Init();

            // act
            TestSerialization(serializer, obj, x => x.Randomize());
        }

        private void TestSerialization<T>(ISerializer serializer, T obj,
            Action<T> randomizeFunction) where T : ScriptableObject
        {
            void Serialize()
            {
                var bytes = serializer.Serialize(obj, null);
            }

            Measure.Method(Serialize)
                .WarmupCount(10)
                .MeasurementCount(10)
                .DynamicMeasurementCount()
                .IterationsPerMeasurement(5)
                .GC()
                .SetUp(() => randomizeFunction?.Invoke(obj))
                .CleanUp(() => { /*cleanup code*/ })
                .Run();
        }
    }

    internal class SmallPersistentObject : ScriptableObject
    {
        public int IntField;
        public string StringField;
        public float FloatField;
        public bool BoolField;

        [Serializable]
        public struct NestedStruct
        {
            public int A, B;
        }

        public NestedStruct Struct;

        public Vector3 Position;
        
        public SmallPersistentObject Reference;

        public void Init()
        {
            Randomize();
        }
        
        public void Randomize()
        {
            IntField = (int)(Random.value * 100000);
            StringField = Random.value.ToString();
            FloatField = Random.value;
            BoolField = Random.value > 0.5f;
            Struct.A = (int)(Random.value * 100000);
            Struct.B = (int)(Random.value * 100000);
            Position = Random.insideUnitSphere;
        }
    }
    
    internal class LargePersistentObject : ScriptableObject
    {
        public List<int> IntFields = new();
        public List<string> StringFields = new();
        public List<float> FloatFields = new();
        public List<bool> BoolFields = new();

        [Serializable]
        public struct NestedStruct
        {
            public int A, B;
        }

        public List<NestedStruct> Structs = new();

        public List<Vector3> Positions = new();
        
        public List<SmallPersistentObject> References = new();

        public void Init()
        {
            Randomize();
        }
        
        public void Randomize(int count = 100)
        {
            IntFields.Clear();
            StringFields.Clear();
            FloatFields.Clear();
            BoolFields.Clear();
            Structs.Clear();
            Structs.Clear();
            Positions.Clear();

            for (int i = 0; i < count; i++)
            {
                IntFields.Add((int)(Random.value * 100000));
                StringFields.Add(Random.value.ToString());
                FloatFields.Add(Random.value);
                BoolFields.Add(Random.value > 0.5f);
                Structs.Add(new NestedStruct
                {
                    A = (int)(Random.value * 100000),
                    B = (int)(Random.value * 100000)
                });
                Positions.Add(Random.insideUnitSphere);
            }
        }
    }
}