using UnityEngine;

namespace SaveSystem
{
    [CreateAssetMenu(fileName = "AssetSaver", menuName = "Facticus/Save System/AssetSaver", order = 0)]
    public class AssetSaver : ScriptableObject
    {
        public async void Save(ScriptableObject obj)
        {
            await obj.Save();
        }
    }
}