using UnityEngine;

namespace SaveSystem.Serializers
{
    public interface ISerializer
    {
        byte[] Serialize(ScriptableObject obj, AssetGuidsDatabase guidsDatabase);
        void Deserialize(byte[] data, ScriptableObject obj, AssetGuidsDatabase guidsDatabase);
    }
}