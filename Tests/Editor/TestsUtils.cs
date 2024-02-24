using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using SaveSystem.GuidsResolve;
using SaveSystem.Serializers;

namespace SaveSystem.Tests.Editor
{
    public static class TestsUtils
    {
        public const string UnitySerializerBinaryKey = "UnitySerializerBinary";
        public const string UnitySerializerJsonKey = "UnitySerializerJson";

        public static readonly Dictionary<string, ISerializer> Serializers = new Dictionary<string, ISerializer>
        {
            { UnitySerializerBinaryKey, new UnitySerializer(SerializationMode.Binary) },
            { UnitySerializerJsonKey, new UnitySerializer(SerializationMode.Json) },
        };
        
        public const string GuidsDatabaseKey = "GuidsDatabase";

        public static readonly Dictionary<string, IGuidResolver> GuidsResolvers = new Dictionary<string, IGuidResolver>
        {
            { UnitySerializerBinaryKey, new GuidsDatabase() },
        };
        
        public static IEnumerator RunTaskAsCoroutine(Task task)
        {
            while (!task.IsCompleted)
            {
                yield return null;
            }
        }
    }
}