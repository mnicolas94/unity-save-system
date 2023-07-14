using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SaveSystem
{
    [Serializable]
    public class SaveLoadBroadcaster
    {
        public static SaveLoadBroadcaster Instance => SaveSystemSettings.Instance.Broadcaster;
        
        [SerializeField] private UnityEvent<ScriptableObject> _onSave;
        [SerializeField] private UnityEvent<ScriptableObject> _onLoad;

        private Dictionary<ScriptableObject, Action> _onSaveSpecific = new Dictionary<ScriptableObject, Action>();
        private Dictionary<ScriptableObject, Action> _onLoadSpecific = new Dictionary<ScriptableObject, Action>();
        
        public UnityEvent<ScriptableObject> OnSave => _onSave;

        public UnityEvent<ScriptableObject> OnLoad => _onLoad;

        public void RegisterOnSaveListenerForObject(ScriptableObject persistent, Action listener)
        {
            if (!_onSaveSpecific.ContainsKey(persistent))
            {
                _onSaveSpecific.Add(persistent, default);
            }

            _onSaveSpecific[persistent] += listener;
        }
        
        public void UnregisterOnSaveListenerForObject(ScriptableObject persistent, Action listener)
        {
            if (_onSaveSpecific.ContainsKey(persistent))
            {
                _onSaveSpecific[persistent] -= listener;
            }
        }
        
        public void RegisterOnLoadListenerForObject(ScriptableObject persistent, Action listener)
        {
            if (!_onLoadSpecific.ContainsKey(persistent))
            {
                _onLoadSpecific.Add(persistent, default);
            }

            _onLoadSpecific[persistent] += listener;
        }
        
        public void UnregisterOnLoadListenerForObject(ScriptableObject persistent, Action listener)
        {
            if (_onLoadSpecific.ContainsKey(persistent))
            {
                _onLoadSpecific[persistent] -= listener;
            }
        }

        internal void NotifySave(ScriptableObject persistent)
        {
            _onSave.Invoke(persistent);
            if (_onSaveSpecific.TryGetValue(persistent, out var listeners))
            {
                listeners?.Invoke();
            }
        }
        
        internal void NotifyLoad(ScriptableObject persistent)
        {
            _onLoad.Invoke(persistent);
            if (_onLoadSpecific.TryGetValue(persistent, out var listeners))
            {
                listeners?.Invoke();
            }
        }
    }
}