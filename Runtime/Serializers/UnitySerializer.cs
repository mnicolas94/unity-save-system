using System;
using System.Collections.Generic;
using SaveSystem.GuidsResolve;
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
        public byte[] Serialize(ScriptableObject obj, IGuidResolver guidsResolver)
        {
            return Serialize<ScriptableObject>(obj, guidsResolver);
        }
        
        public unsafe byte[] Serialize<T>(T obj, IGuidResolver guidsResolver) where T : Object
        {
            using var stream = new UnsafeAppendBuffer(128, 8, Allocator.Temp);
            var unsafeAppendBuffer = stream;
            var parameters = GetBinarySerializationParameters(guidsResolver);

            BinarySerialization.ToBinary(&unsafeAppendBuffer, obj, parameters);
            var data = unsafeAppendBuffer.ToBytesNBC();
            
            return data;
        }

        public void Deserialize(byte[] data, ScriptableObject obj, IGuidResolver guidsResolver)
        {
            Deserialize<ScriptableObject>(data, obj, guidsResolver);
        }
        
        public unsafe void Deserialize<T>(byte[] data, T obj, IGuidResolver guidsResolver) where T : Object
        {
            using var nativeArray = new NativeArray<byte>(data, Allocator.Temp);

            var reader = new UnsafeAppendBuffer.Reader(nativeArray.GetUnsafePtr(), nativeArray.Length);

            var parameters = GetBinarySerializationParameters(guidsResolver);
            var deserialized = BinarySerialization.FromBinary<T>(&reader, parameters);
            
            ReflectionUtils.CopyTo(deserialized, obj);
        }
        
        private static BinarySerializationParameters GetBinarySerializationParameters(
            IGuidResolver guidsResolver)
        {
            var parameters = new BinarySerializationParameters
            {
                UserDefinedAdapters = new List<IBinaryAdapter> { new GuidBinaryAdapter(guidsResolver) },
                DisableRootAdapters = true
            };
            return parameters;
        }
    }

    public class GuidBinaryAdapter : IContravariantBinaryAdapter<Object>
    {
        private IGuidResolver _resolver;

        public GuidBinaryAdapter(IGuidResolver resolver)
        {
            _resolver = resolver;
        }

        public void Serialize(IBinarySerializationContext context, Object value)
        {
            var guid = "";
            if (_resolver != null && value != null)
            {
                _resolver.TryGetGuid(value, out guid);
            }
            
            context.SerializeValue(guid);
        }

        public object Deserialize(IBinaryDeserializationContext context)
        {
            var guid = context.DeserializeValue<string>();
            if (_resolver != null && !string.IsNullOrEmpty(guid))
            {
                _resolver.TryGetObject(guid, out var value);
                
                return value;
            }
            
            return null;
        }
    }
}