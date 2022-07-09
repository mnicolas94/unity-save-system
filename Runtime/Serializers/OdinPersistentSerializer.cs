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
        
        public byte[] Serialize(IPersistent obj, AssetGuidsDatabase guidsDatabase)
        {
            var context = new SerializationContext
            {
                StringReferenceResolver = new GuidsReferenceResolver(guidsDatabase)
            };
            byte[] bytes = SerializationUtility.SerializeValue(obj, _format, context);
            return bytes;
        }

        public void Deserialize(byte[] data, IPersistent obj, AssetGuidsDatabase guidsDatabase)
        {
            var context = new DeserializationContext()
            {
                StringReferenceResolver = new GuidsReferenceResolver(guidsDatabase)
            };
            var loaded = SerializationUtility.DeserializeValue<IPersistent>(data, _format, context);
            var fields = loaded.GetType().GetRuntimeFields().ToArray();
            foreach (var field in fields)
            {
                var value = field.GetValue(loaded);
                field.SetValue(obj, value);
            }
        }
    }

    public class GuidsReferenceResolver : IExternalStringReferenceResolver
    {
        private AssetGuidsDatabase _database;
        
        public GuidsReferenceResolver(AssetGuidsDatabase database)
        {
            _database = database;
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
            if (value is Object obj && !(value is IPersistent))
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