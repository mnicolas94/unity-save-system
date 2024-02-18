using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        
        public static IEnumerator RunTaskAsCoroutine(Task task)
        {
            while (!task.IsCompleted)
            {
                yield return null;
            }
        }
    }
}