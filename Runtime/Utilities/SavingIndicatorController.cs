using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace SaveSystem.Utilities
{
    public class SavingIndicatorController : MonoBehaviour
    {
        [Tooltip("Called when a file save starts, and there was no other file being saved at that moment. If more files" +
                 "get saved after that and the first file it's still saving, then the method is not called for those additional files.")]
        [SerializeField] private UnityEvent _saveStarted;
        
        [Tooltip("Called when a file save ends and there is no other files being saved at that moment.")]
        [SerializeField] private UnityEvent _saveEnded;

        private readonly List<Object> _beingSaved = new ();
        
        private void Awake()
        {
            SaveSystemSettings.Instance.Broadcaster.SaveStarted.AddListener(OnSaveStarted);
            SaveSystemSettings.Instance.Broadcaster.SaveEnded.AddListener(OnSaveEnded);
        }

        private void OnDestroy()
        {
            SaveSystemSettings.Instance.Broadcaster.SaveStarted.RemoveListener(OnSaveStarted);
            SaveSystemSettings.Instance.Broadcaster.SaveEnded.RemoveListener(OnSaveEnded);
        }

        private void OnSaveStarted(ScriptableObject objectSaved)
        {
            if (_beingSaved.Contains(objectSaved)) return;
            
            _beingSaved.Add(objectSaved);

            if (_beingSaved.Count == 1)
            {
                _saveStarted.Invoke();
            }
        }

        private void OnSaveEnded(ScriptableObject objectSaved)
        {
            _beingSaved.Remove(objectSaved);

            if (_beingSaved.Count == 0)
            {
                _saveEnded.Invoke();
            }
        }
    }
}