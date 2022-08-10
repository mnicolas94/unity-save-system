﻿using System;
using System.Text;
using UnityEngine;

namespace SaveSystem.Serializers
{
    [Serializable]
    public class JsonSerializer : ISerializer
    {
        public byte[] Serialize(ScriptableObject obj, AssetGuidsDatabase guidsDatabase)
        {
            string json = JsonUtility.ToJson(obj);
            return Encoding.UTF8.GetBytes(json);
        }

        public void Deserialize(byte[] data, ScriptableObject obj, AssetGuidsDatabase guidsDatabase)
        {
            string dataStr = Encoding.UTF8.GetString(data);
            JsonUtility.FromJsonOverwrite(dataStr, obj);
        }
    }
}