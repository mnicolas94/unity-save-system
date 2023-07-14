using System.Collections.Generic;
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
                await persistentObject.Save();
            }
        }
    }
}