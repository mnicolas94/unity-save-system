using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace SaveSystem.Utilities
{
    [CreateAssetMenu(fileName = "SaveGroup", menuName = "Facticus/Save System/SaveGroup", order = 0)]
    public class SaveGroup : ScriptableObject, IPersistentResetable, IPersistentAdapter
    {
        [SerializeField] private List<ScriptableObject> _objects = new();

        public bool IsInGroup(ScriptableObject obj)
        {
            return _objects.Contains(obj);
        }
        
        public void AddToGroup(ScriptableObject obj)
        {
            if (!_objects.Contains(obj))
            {
                _objects.Add(obj);
            }
        }

        public void RemoveFromGroup(ScriptableObject obj)
        {
            _objects.Remove(obj);
        }

        public void Clear()
        {
            _objects.Clear();
        }

        public async Task Save()
        {
            foreach (var obj in _objects)
            {
                await obj.Save();
            }
        }
        
        public async Task<LoadReport> Load()
        {
            LoadReport report = default;
            foreach (var obj in _objects)
            {
                report = await obj.Load();
            }

            return report;
        }

        public async Task<LoadReport> LoadOrCreate()
        {
            LoadReport report = default;
            foreach (var obj in _objects)
            {
                report = await obj.LoadOrCreate();
            }

            return report;
        }

        public Task<bool> IsSaved()
        {
            return AreAllSaved();
        }

        public async Task<bool> IsAnySaved()
        {
            foreach (var obj in _objects)  // TODO perhaps parallelize this?
            {
                var isSaved = await obj.IsSaved();
                if (isSaved)
                {
                    return true;
                }
            }

            return false;
        }
        
        public async Task<bool> AreAllSaved()
        {
            foreach (var obj in _objects)  // TODO perhaps parallelize this?
            {
                var isSaved = await obj.IsSaved();
                if (!isSaved)
                {
                    return false;
                }
            }

            return true;
        }

        public async Task DeleteData()
        {
            foreach (var obj in _objects)  // TODO perhaps parallelize this?
            {
                await obj.DeleteData();
            }
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