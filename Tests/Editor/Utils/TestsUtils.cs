using System;
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
        public static readonly List<ISerializer> Serializers = new()
        {
            new UnitySerializer(SerializationMode.Binary),
            new UnitySerializer(SerializationMode.Json),
        };
        
        public static readonly List<IGuidResolver> GuidsResolvers = new()
        {
            new GuidsDatabase(),
        };
        
        public static readonly List<IStorage> Storages = new()
        {
            new FilesStorage(),
            new PlayerPrefsStorage(),
        };
        
        public static IEnumerator RunTaskAsCoroutine(Task task)
        {
            while (!task.IsCompleted)
            {
                yield return null;
            }
        }
        
        public static T RunAsyncMethodSync<T>(Task<T> task)
        {
            return Task.Run(async () => await task).GetAwaiter().GetResult();
        }
        
        public static void RunAsyncMethodSync(Task task)
        {
            Task.Run(async () => await task).GetAwaiter().GetResult();
        }
    }
}