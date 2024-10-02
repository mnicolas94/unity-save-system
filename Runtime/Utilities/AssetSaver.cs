using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace SaveSystem.Utilities
{
    [MovedFrom(sourceNamespace: "SaveSystem")]
    [CreateAssetMenu(fileName = "AssetSaver", menuName = "Facticus/Save System/AssetSaver", order = 0)]
    public class AssetSaver : ScriptableObject
    {
        public async void Save(ScriptableObject obj)
        {
            if (obj is SaveGroup group)
            {
                await group.Save();
            }
            await obj.Save();
        }
    }
}