using UnityEngine;

namespace SaveSystem
{
    [CreateAssetMenu(fileName = "AssetSaver", menuName = "Facticus/Save system/AssetSaver", order = 0)]
    public class AssetSaver : ScriptableObject
    {
        public void Save(ScriptableObject obj)
        {
            obj.Save();
        }
    }
}