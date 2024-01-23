using SaveSystem.GuidsResolve;
using UnityEngine;

namespace SaveSystem.Serializers
{
    public interface ISerializer
    {
        byte[] Serialize(ScriptableObject obj, IGuidResolver guidsDatabase);
        void Deserialize(byte[] data, ScriptableObject obj, IGuidResolver guidsDatabase);
    }
}