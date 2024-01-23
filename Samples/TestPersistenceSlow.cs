using System;
using SaveSystem;
using SaveSystem.GuidsResolve;
using UnityEngine;

namespace Samples
{
    [CreateAssetMenu(fileName = "TestPersistenceSlow", menuName = "TestPersistenceSlow", order = 0)]
    public class TestPersistenceSlow : ScriptableObject, IPersistentCallbackReceiver
    {
        [SerializeField] private float _a;
        [SerializeField] private string _b;
        
        [ContextMenu("Save")]
        private void SaveThis()
        {
            Debug.Log("Before Save call...");
            this.Save();
            Debug.Log("After Save call...");
        }
        
        [ContextMenu("Load")]
        private void LoadThis()
        {
            Debug.Log("Before Load call...");
            this.Load();
            Debug.Log("After Load call...");
        }
        
        public void OnBeforeSave(IGuidResolver guidsDatabase)
        {
            Debug.Log("Saving...");
            WaitSecondsSynchronous(2);
        }

        public void OnAfterSave()
        {
            Debug.Log("Saved");
        }

        public void OnBeforeLoad()
        {
            Debug.Log("Loading...");
            WaitSecondsSynchronous(2);
        }

        public void OnAfterLoad(IGuidResolver guidsDatabase)
        {
            Debug.Log("Loaded");
        }

        private void WaitSecondsSynchronous(float seconds)
        {
            var start = DateTime.Now;
            var elapsedSeconds = 0;
            while (elapsedSeconds < seconds)
            {
                var now = DateTime.Now;
                elapsedSeconds = (now - start).Seconds;
            }
        }
    }
}