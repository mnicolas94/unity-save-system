using System.Collections.Generic;
using SaveSystem.Utilities;
using UnityEngine;

namespace SaveSystem
{
    public class SaveAllOnDisable : MonoBehaviour
    {
        [SerializeField] private List<ScriptableObject> _persistentObjects;
        private async void OnDisable()
        {
            foreach (var persistentObject in _persistentObjects)
            {
                if (persistentObject is SaveGroup group)
                {
                    await group.Save();
                }
                await persistentObject.Save();
            }
        }
    }
}