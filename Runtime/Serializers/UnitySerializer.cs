using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Collections.LowLevel.Unsafe.NotBurstCompatible;
using Unity.Serialization.Binary;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SaveSystem.Serializers
{
    [Serializable]
    public class UnitySerializer : ISerializer
    {
        public byte[] Serialize(ScriptableObject obj, AssetGuidsDatabase guidsDatabase)
        {
            return Serialize<ScriptableObject>(obj, guidsDatabase);
        }
        
        public unsafe byte[] Serialize<T>(T obj, AssetGuidsDatabase guidsDatabase) where T : Object
        {
            using var stream = new UnsafeAppendBuffer(128, 8, Allocator.Temp);
            var unsafeAppendBuffer = stream;
            var parameters = GetBinarySerializationParameters(guidsDatabase);

            BinarySerialization.ToBinary(&unsafeAppendBuffer, obj, parameters);
            var data = unsafeAppendBuffer.ToBytesNBC();
            
            return data;
        }

        public void Deserialize(byte[] data, ScriptableObject obj, AssetGuidsDatabase guidsDatabase)
        {
            Deserialize<ScriptableObject>(data, obj, guidsDatabase);
        }
        
        public unsafe void Deserialize<T>(byte[] data, T obj, AssetGuidsDatabase guidsDatabase) where T : Object
        {
            using var nativeArray = new NativeArray<byte>(data, Allocator.Temp);

            var reader = new UnsafeAppendBuffer.Reader(nativeArray.GetUnsafePtr(), nativeArray.Length);

            var parameters = GetBinarySerializationParameters(guidsDatabase);
            var deserialized = BinarySerialization.FromBinary<T>(&reader, parameters);
            
            ReflectionUtils.CopyTo(deserialized, obj);
        }
        
        private static BinarySerializationParameters GetBinarySerializationParameters(
            AssetGuidsDatabase guidsDatabase)
        {
            var parameters = new BinarySerializationParameters
            {
                UserDefinedAdapters = new List<IBinaryAdapter> { new GuidBinaryAdapter(guidsDatabase) },
                DisableRootAdapters = true
            };
            return parameters;
        }
    }

    public class GuidBinaryAdapter : IContravariantBinaryAdapter<Object>
    {
        private AssetGuidsDatabase _database;

        public GuidBinaryAdapter(AssetGuidsDatabase database)
        {
            _database = database;
        }

        public void Serialize(IBinarySerializationContext context, Object value)
        {
            var guid = "";
            if (_database != null)
            {
                guid = _database.TryGetGuid(value, out bool exists);
            }
            
            context.SerializeValue(guid);
        }

        public object Deserialize(IBinaryDeserializationContext context)
        {
            if (_database != null)
            {
                var guid = context.DeserializeValue<string>();
                var value = _database.TryGetObject(guid, out var exists);
                
                return value;
            }
            
            return null;
        }
    }
}