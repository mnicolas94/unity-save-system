using System;
using System.Linq;
using System.Reflection;
using OdinSerializer.OdinSerializer;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SaveSystem.Serializers
{
    [Serializable]
    public class OdinPersistentSerializer : ISerializer
    {
        [SerializeField] private DataFormat _format;
        
        public byte[] Serialize(ScriptableObject obj, AssetGuidsDatabase guidsDatabase)
        {
            var context = new SerializationContext
            {
                StringReferenceResolver = new GuidsReferenceResolver(guidsDatabase, obj)
            };
            byte[] bytes = SerializationUtility.SerializeValue(obj, _format, context);
            return bytes;
        }

        public void Deserialize(byte[] data, ScriptableObject obj, AssetGuidsDatabase guidsDatabase)
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
        private AssetGuidsDatabase _database;
        private Object _parent;
        
        public GuidsReferenceResolver(AssetGuidsDatabase database, Object parent)
        {
            _database = database;
            _parent = parent;
        }

        public IExternalStringReferenceResolver NextResolver { get; set; }
        
        public bool TryResolveReference(string id, out object value)
        {
            value = null;
            if (id == "not_in_database") return true;

            value = _database.TryGetObject(id, out bool exists);
            return exists;
        }

        public bool CanReference(object value, out string id)
        {
            id = null;
            bool parentIsScriptableObject = _parent is ScriptableObject;
            bool isParent = ReferenceEquals(value, _parent);
            if (value is Object obj && (!isParent || !parentIsScriptableObject))
            {
                id = _database.TryGetGuid(obj, out bool exists);
                if (!exists)
                    id = "not_in_database";

                return true;
            }
            
            return false;
        }
    }
}