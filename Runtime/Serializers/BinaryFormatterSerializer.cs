using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using OdinSerializer.OdinSerializer.Utilities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SaveSystem.Serializers
{
    public class BinaryFormatterSerializer : ISerializer
    {
        public byte[] Serialize(ScriptableObject obj, AssetGuidsDatabase guidsDatabase)
        {
            var formatter = new BinaryFormatter();
            var memoryStream = new MemoryStream();
            var ss = new SurrogateSelector();
            var serializer = new SurrogateSel(guidsDatabase);
            ss.AddSurrogate(typeof(Object), new StreamingContext(StreamingContextStates.All), serializer);
            formatter.SurrogateSelector = ss;
            
            var fields = obj.GetType().GetRuntimeFields();
            fields.ForEach(field =>
            {
                var value = field.GetValue(obj);
                formatter.Serialize(memoryStream, value);
            });
            var bytes = memoryStream.ToArray();
            
            return bytes;
        }

        public void Deserialize(byte[] data, ScriptableObject obj, AssetGuidsDatabase guidsDatabase)
        {
            var formatter = new BinaryFormatter();
            var ss = new SurrogateSelector();
            var serializer = new SurrogateSel(guidsDatabase);
            ss.AddSurrogate(typeof(Object), new StreamingContext(StreamingContextStates.All), serializer);
            formatter.SurrogateSelector = ss;
            var memoryStream = new MemoryStream();
            memoryStream.Write(data, 0, data.Length);
            var deserializeObject = (Object) formatter.Deserialize(memoryStream);
            
            var fields = deserializeObject.GetType().GetRuntimeFields();
            fields.ForEach(field =>
            {
                var value = field.GetValue(deserializeObject);
                field.SetValue(obj, value);
            });
        }
    }

    public class SurrogateSel : ISerializationSurrogate
    {
        private AssetGuidsDatabase _database;
        
        public SurrogateSel(AssetGuidsDatabase database)
        {
            _database = database;
        }
        
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            if (obj is Object unityObj)
            {
                var guid = _database.GetGuid(unityObj);
                info.AddValue("guid", guid);
            }
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            var guid = info.GetString("guid");
            var unityObj = _database.GetObject(guid);
            return unityObj;
        }
    }
}