using System;
using SaveSystem.GuidsResolve;
using UnityEngine;

namespace SaveSystem.Serializers
{
    public enum SerializationMode
    {
        Binary,
        Json
    }
    
    [Serializable]
    public class UnitySerializer : ISerializer
    {
        [SerializeField] private SerializationMode _mode;

        public SerializationMode Mode
        {
            get => _mode;
            set => _mode = value;
        }

        public UnitySerializer() : this(SerializationMode.Binary)
        {
        }

        public UnitySerializer(SerializationMode mode)
        {
            _mode = mode;
        }

        public byte[] Serialize(ScriptableObject obj, IGuidResolver guidsResolver)
        {
            if (_mode == SerializationMode.Binary)
            {
                return UnityBinarySerializer.Serialize(obj, guidsResolver);
            }
            else
            {
                return UnityJsonSerializer.Serialize(obj, guidsResolver);
            }
        }

        public void Deserialize(byte[] data, ScriptableObject obj, IGuidResolver guidsResolver)
        {
            if (_mode == SerializationMode.Binary)
            {
                UnityBinarySerializer.Deserialize(data, obj, guidsResolver);
            }
            else
            {
                UnityJsonSerializer.Deserialize(data, obj, guidsResolver);
            }
        }

        public override string ToString()
        {
            return $"{nameof(UnitySerializer)}-{_mode}";
        }
    }
}