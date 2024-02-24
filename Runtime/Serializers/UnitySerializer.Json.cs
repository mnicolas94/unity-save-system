using System.Collections.Generic;
using System.Text;
using SaveSystem.GuidsResolve;
using Unity.Serialization.Json;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SaveSystem.Serializers
{
    public static class UnityJsonSerializer
    {
        public static byte[] Serialize<T>(T obj, IGuidResolver guidsResolver) where T : Object
        {
            var parameters = GetJsonSerializationParameters(guidsResolver);

            var json = JsonSerialization.ToJson(obj, parameters);
            var data = Encoding.UTF8.GetBytes(json);
            
            return data;
        }
        
        public static void Deserialize<T>(byte[] data, T obj, IGuidResolver guidsResolver) where T : Object
        {
            var parameters = GetJsonSerializationParameters(guidsResolver);
            var json = Encoding.UTF8.GetString(data);
            var deserialized = JsonSerialization.FromJson<T>(json, parameters);
            
            ReflectionUtils.CopyTo(deserialized, obj);
        }
        
        private static JsonSerializationParameters GetJsonSerializationParameters(IGuidResolver guidsResolver)
        {
            var parameters = new JsonSerializationParameters
            {
                UserDefinedAdapters = new List<IJsonAdapter>
                {
                    new SerializationCallbacksTriggerJsonAdapter(),
                    new GuidJsonAdapter(guidsResolver)
                },
                DisableRootAdapters = true
            };
            return parameters;
        }
    }

    public class SerializationCallbacksTriggerJsonAdapter : IContravariantJsonAdapter<object>
    {
        public void Serialize(IJsonSerializationContext context, object value)
        {
            if (value is ISerializationCallbackReceiver receiver)
            {
                receiver.OnBeforeSerialize();
            }
            context.ContinueVisitation();
        }

        public object Deserialize(IJsonDeserializationContext context)
        {
            var result = context.ContinueVisitation();
            if (result is ISerializationCallbackReceiver receiver)
            {
                receiver.OnAfterDeserialize();
            }

            return result;
        }
    }
    

    public class GuidJsonAdapter : IContravariantJsonAdapter<Object>
    {
        private IGuidResolver _resolver;

        public GuidJsonAdapter(IGuidResolver resolver)
        {
            _resolver = resolver;
        }

        public void Serialize(IJsonSerializationContext context, Object value)
        {
            var guid = "";
            if (_resolver != null && value != null)
            {
                guid = _resolver.GetGuid(value);
            }
            
            context.SerializeValue(guid);
        }

        public object Deserialize(IJsonDeserializationContext context)
        {
            var guid = context.DeserializeValue<string>(context.SerializedValue);
            if (_resolver != null && !string.IsNullOrEmpty(guid))
            {
                var value = _resolver.GetObject(guid);
                
                return value;
            }
            
            return null;
        }
    }
}