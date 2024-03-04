using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using SaveSystem.Serializers;
using SaveSystem.Storages;
using SaveSystem.Tests.Editor;
using Unity.PerformanceTesting;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Profiling;
using Random = UnityEngine.Random;

namespace SaveSystem.Tests.Runtime
{
    public class PerformanceTests
    {
        [Performance]
        [TestCaseSource(typeof(TestsUtils), nameof(TestsUtils.Serializers))]
        public void Serialization_PerformanceTest(ISerializer serializer)
        {
            // arrange
            var obj = ScriptableObject.CreateInstance<SmallPersistentObject>();
            obj.Init();

            TestSerialization(serializer, obj, x => x.Randomize());
        }
        
        [Performance]
        [TestCaseSource(typeof(TestsUtils), nameof(TestsUtils.Serializers))]
        public void SerializationLargeObject_PerformanceTest(ISerializer serializer)
        {
            // arrange
            var obj = ScriptableObject.CreateInstance<LargePersistentObject>();
            obj.Init();

            // act
            TestSerialization(serializer, obj, x => x.Randomize());
        }

        private static List<(string, bool, Action<FilesStorage, string, string, byte[]>)> StorageFunctions = new()
        {
            (nameof(TestStorageWrite), false, TestStorageWrite),
            (nameof(TestStorageWriteWithStream), false, TestStorageWriteWithStream),
            (nameof(TestStorageRead), false, TestStorageRead),
            (nameof(TestStorageReadWithStream), false, TestStorageReadWithStream),
            ("Clear Data", true, null),
        };
        
        [Performance]
        [TestCaseSource(nameof(StorageFunctions))]
        public void IStorageVsIStorageStream_PerformanceTest(
            ValueTuple<string, bool, Action<FilesStorage, string, string, byte[]>> arguments)
        {
            var (name, clearData, function) = arguments;
            var storage = new FilesStorage();
            var profile = "profile";
            var dataKey = "key";
            
            if (clearData)
            {
                storage.Delete(profile, dataKey);
                return;
            }

            // create large data
            var obj = ScriptableObject.CreateInstance<LargePersistentObject>();
            obj.Init();
            var json = JsonUtility.ToJson(obj);
            var data = Encoding.UTF8.GetBytes(json);

            Profiler.BeginSample($"TestStorage - {name}");
            using (Measure.ProfilerMarkers("GC.Alloc"))
            {
                Measure.Method(() => function(storage, profile, dataKey, data))
                    .WarmupCount(20)
                    .MeasurementCount(50)
                    .DynamicMeasurementCount()
                    .IterationsPerMeasurement(5)
                    .Run();
            }
            Profiler.EndSample();
        }
        
        private static async void TestStorageWrite(FilesStorage storage, string profile, string dataKey, byte[] data)
        {
            using var memoryStream = new MemoryStream();
            using var writer = new BinaryWriter(memoryStream);
            writer.Write(data.Length);
            writer.Write(data);
                
            await storage.Write(profile, dataKey, memoryStream.ToArray());
        }
            
        private static async void TestStorageWriteWithStream(FilesStorage storage, string profile, string dataKey, byte[] data)
        {
            var stream = storage.GetStreamToWrite(profile, dataKey);
            await using var writer = new BinaryWriter(stream);
            writer.Write(data.Length);
            writer.Write(data);
        }
            
        private static async void TestStorageRead(FilesStorage storage, string profile, string dataKey, byte[] data)
        {
            var (success, dataResult ) = await storage.Read(profile, dataKey);
            using var memoryStream = new MemoryStream(dataResult);
            using var reader = new BinaryReader(memoryStream);
            var length = reader.ReadInt32();
            var bytes = reader.ReadBytes(length);
        }
            
        private static void TestStorageReadWithStream(FilesStorage storage, string profile, string dataKey, byte[] data)
        {
            var success = storage.TryGetStreamToRead(profile, dataKey, out var stream);
            using var reader = new BinaryReader(stream);
            var length = reader.ReadInt32();
            var bytes = reader.ReadBytes(length);
        }
        
        [Performance]
        [TestCaseSource(typeof(TestsUtils), nameof(TestsUtils.Storages))]
        public void Storages_PerformanceTest(IStorage storage)
        {
            var profile = "profile";
            var dataKey = "key";

            // create large data
            var obj = ScriptableObject.CreateInstance<LargePersistentObject>();
            obj.Init();
            var json = JsonUtility.ToJson(obj);
            var data = Encoding.UTF8.GetBytes(json);

            async void TestWrite()
            {
                await storage.Write(profile, dataKey, data);
            }
            
            async void TestRead()
            {
                var result = await storage.Read(profile, dataKey);
            }

            var name = $"Storage write - {storage}";
            Profiler.BeginSample(name);
            Measure.Method(TestWrite)
                .WarmupCount(10).MeasurementCount(10).DynamicMeasurementCount().IterationsPerMeasurement(5).Run();
            Profiler.EndSample();

            name = $"Storage read - {storage}";
            Profiler.BeginSample(name);
            Measure.Method(TestRead)
                .WarmupCount(10).MeasurementCount(10).DynamicMeasurementCount().IterationsPerMeasurement(5).Run();
            Profiler.EndSample();
            
            // tear down
            storage.Delete(profile, dataKey);
        }

        private void TestSerialization<T>(ISerializer serializer, T obj,
            Action<T> randomizeFunction) where T : ScriptableObject
        {
            void Serialize()
            {
                var bytes = serializer.Serialize(obj, null);
            }

            using (Measure.ProfilerMarkers("GC.Alloc"))
            {
                Measure.Method(Serialize)
                    .WarmupCount(10)
                    .MeasurementCount(10)
                    .DynamicMeasurementCount()
                    .IterationsPerMeasurement(5)
                    .SetUp(() => randomizeFunction?.Invoke(obj))
                    .CleanUp(() => { /*cleanup code*/ })
                    .Run();
            }
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