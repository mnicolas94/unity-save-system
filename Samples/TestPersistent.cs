using SaveSystem;
using UnityEngine;
using Utils;

namespace Samples
{
    [CreateAssetMenu(fileName = "TestPersistent", menuName = "TestPersistent", order = 0)]
    public class TestPersistent : ScriptableObjectSingleton<TestPersistent>
    {
        [SerializeField] private float _a;
        [SerializeField] private string _b;
        
        [ContextMenu("Save")]
        private void SaveThis()
        {
            this.Save();
        }
        
        [ContextMenu("Load")]
        private void LoadThis()
        {
            this.Load();
        }
    }
}