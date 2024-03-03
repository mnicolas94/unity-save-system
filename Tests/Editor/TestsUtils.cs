using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SaveSystem.GuidsResolve;
using SaveSystem.Serializers;
using SaveSystem.Storages;

namespace SaveSystem.Tests.Editor
{
    public static class TestsUtils
    {
        public static readonly Dictionary<string, ISerializer> Serializers = new Dictionary<string, ISerializer>
        {
            { "UnitySerializerBinary", new UnitySerializer(SerializationMode.Binary) },
            { "UnitySerializerJson", new UnitySerializer(SerializationMode.Json) },
        };
        
        public static readonly Dictionary<string, IGuidResolver> GuidsResolvers = new Dictionary<string, IGuidResolver>
        {
            { "GuidsDatabase", new GuidsDatabase() },
        };
        
        public static readonly Dictionary<string, IStorage> Storages = new Dictionary<string, IStorage>
        {
            { "storage-files", new FilesStorage() },
        };

        public static List<string> SerializersKeys => Serializers.Keys.ToList();
        
        public static IEnumerator RunTaskAsCoroutine(Task task)
        {
            while (!task.IsCompleted)
            {
                yield return null;
            }
        }
    }
}