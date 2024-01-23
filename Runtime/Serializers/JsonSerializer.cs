using System;
using System.Text;
using SaveSystem.GuidsResolve;
using UnityEngine;

namespace SaveSystem.Serializers
{
    [Serializable]
    public class JsonSerializer : ISerializer
    {
        public byte[] Serialize(ScriptableObject obj, IGuidResolver guidsDatabase)
        {
            string json = JsonUtility.ToJson(obj);
            return Encoding.UTF8.GetBytes(json);
        }

        public void Deserialize(byte[] data, ScriptableObject obj, IGuidResolver guidsDatabase)
        {
            string dataStr = Encoding.UTF8.GetString(data);
            JsonUtility.FromJsonOverwrite(dataStr, obj);
        }
    }
}