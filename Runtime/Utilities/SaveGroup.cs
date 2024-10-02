using System.Collections.Generic;
using System.Threading.Tasks;
using SaveSystem.Attributes;
using UnityEngine;

namespace SaveSystem.Utilities
{
    [CreateAssetMenu(fileName = "SaveGroup", menuName = "Facticus/SaveSystem/SaveGroup", order = 0)]
    public class SaveGroup : ScriptableObject
    {
        [SerializeField, DoNotPersist] private List<ScriptableObject> _objects;

        public async Task Save()
        {
            foreach (var obj in _objects)
            {
                await obj.Save();
            }
        }
        
        public async Task<SaveUtils.LoadReport> Load()
        {
            SaveUtils.LoadReport report = default;
            foreach (var obj in _objects)
            {
                report = await obj.Load();
            }

            return report;
        }

        public async Task<SaveUtils.LoadReport> LoadOrCreate()
        {
            SaveUtils.LoadReport report = default;
            foreach (var obj in _objects)
            {
                report = await obj.LoadOrCreate();
            }

            return report;
        }
        
        public void ResetToDefault()
        {
            foreach (var obj in _objects)
            {
                obj.ResetToDefault();
            }
        }
        
        private void OnValidate()
        {
            if (_objects.Contains(this))
            {
                _objects.Remove(this);
                Debug.LogWarning("Can't add itself to SaveGroup", this);
            }
        }
    }
}