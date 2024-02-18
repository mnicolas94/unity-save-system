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
    public static class UnityBinarySerializer
    {
        public static unsafe byte[] Serialize<T>(T obj, IGuidResolver guidsResolver) where T : Object
        {
            using var stream = new UnsafeAppendBuffer(128, 8, Allocator.Temp);
            var unsafeAppendBuffer = stream;
            var parameters = GetBinarySerializationParameters(guidsResolver);

            BinarySerialization.ToBinary(&unsafeAppendBuffer, obj, parameters);
            var data = unsafeAppendBuffer.ToBytesNBC();
            
            return data;
        }
        
        public static unsafe void Deserialize<T>(byte[] data, T obj, IGuidResolver guidsResolver) where T : Object
        {
            using var nativeArray = new NativeArray<byte>(data, Allocator.Temp);

            var reader = new UnsafeAppendBuffer.Reader(nativeArray.GetUnsafePtr(), nativeArray.Length);

            var parameters = GetBinarySerializationParameters(guidsResolver);
            var deserialized = BinarySerialization.FromBinary<T>(&reader, parameters);
            
            ReflectionUtils.CopyTo(deserialized, obj);
        }
        
        private static BinarySerializationParameters GetBinarySerializationParameters(IGuidResolver guidsResolver)
        {
            var parameters = new BinarySerializationParameters
            {
                UserDefinedAdapters = new List<IBinaryAdapter>
                {
                    new SerializationCallbacksTriggerBinaryAdapter(),
                    new GuidBinaryAdapter(guidsResolver)
                },
                DisableRootAdapters = true
            };
            return parameters;
        }
    }

    public class SerializationCallbacksTriggerBinaryAdapter : IContravariantBinaryAdapter<object>
    {
        public void Serialize(IBinarySerializationContext context, object value)
        {
            if (value is ISerializationCallbackReceiver receiver)
            {
                receiver.OnBeforeSerialize();
            }

            context.ContinueVisitation();
        }

        public object Deserialize(IBinaryDeserializationContext context)
        {
            var result = context.ContinueVisitation();
            if (result is ISerializationCallbackReceiver receiver)
            {
                receiver.OnAfterDeserialize();
            }

            return result;
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