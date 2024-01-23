using System;
using System.Collections.Generic;
using OdinSerializer.OdinSerializer;
using SaveSystem.GuidsResolve;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SaveSystem.Serializers
{
    [Serializable]
    public class OdinPersistentSerializer : ISerializer
    {
        [SerializeField] private DataFormat _format;
        [SerializeField] private List<string> _aotAdditionalTypes;

        public List<string> AOTAdditionalTypes => _aotAdditionalTypes;

        public byte[] Serialize(ScriptableObject obj, IGuidResolver guidsResolver)
        {
            var context = new SerializationContext
            {
                StringReferenceResolver = new GuidsReferenceResolver(guidsResolver, obj)
            };
            byte[] bytes = SerializationUtility.SerializeValue(obj, _format, context);
            return bytes;
        }

        public void Deserialize(byte[] data, ScriptableObject obj, IGuidResolver guidsDatabase)
        {
            var context = new DeserializationContext
            {
                StringReferenceResolver = new GuidsReferenceResolver(guidsDatabase, obj)
            };
            var loaded = SerializationUtility.DeserializeValue<ScriptableObject>(data, _format, context);
            ReflectionUtils.CopyTo(loaded, obj);
        }
    }

    public class GuidsReferenceResolver : IExternalStringReferenceResolver
    {
        private IGuidResolver _database;
        private Object _parent;
        
        public GuidsReferenceResolver(IGuidResolver database, Object parent)
        {
            _database = database;
            _parent = parent;
        }

        public IExternalStringReferenceResolver NextResolver { get; set; }
        
        public bool TryResolveReference(string id, out object value)
        {
            value = null;
            if (id == "not_in_database") return true;

            var exists = _database.TryGetObject(id, out var obj);
            value = obj;
            return exists;
        }

        public bool CanReference(object value, out string id)
        {
            id = null;
            bool parentIsScriptableObject = _parent is ScriptableObject;
            bool isParent = ReferenceEquals(value, _parent);
            if (value is Object obj && (!isParent || !parentIsScriptableObject))
            {
                if (!_database.TryGetGuid(obj, out id))
                    id = "not_in_database";

                return true;
            }
            
            return false;
        }
    }
}