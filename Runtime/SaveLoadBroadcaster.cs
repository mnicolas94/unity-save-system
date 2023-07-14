using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

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

        private Dictionary<ScriptableObject, string> _savedInScenes = new Dictionary<ScriptableObject, string>();
        private Dictionary<ScriptableObject, string> _loadedInScenes = new Dictionary<ScriptableObject, string>();

        public UnityEvent<ScriptableObject> OnSave => _onSave;

        public UnityEvent<ScriptableObject> OnLoad => _onLoad;

        internal void NotifySave(ScriptableObject persistent)
        {
            // track saved state
            if (!_savedInScenes.ContainsKey(persistent))
            {
                _savedInScenes.Add(persistent, "");
            }
            _savedInScenes[persistent] = SceneManager.GetActiveScene().name;
            
            // notify to listeners
            _onSave.Invoke(persistent);
            if (_onSaveSpecific.TryGetValue(persistent, out var listeners))
            {
                listeners?.Invoke();
            }
        }
        
        internal void NotifyLoad(ScriptableObject persistent)
        {
            // track loaded state
            if (!_loadedInScenes.ContainsKey(persistent))
            {
                _loadedInScenes.Add(persistent, "");
            }
            _loadedInScenes[persistent] = SceneManager.GetActiveScene().name;
            
            // notify to listeners
            _onLoad.Invoke(persistent);
            if (_onLoadSpecific.TryGetValue(persistent, out var listeners))
            {
                listeners?.Invoke();
            }
        }

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
                if (_onSaveSpecific[persistent] == null)  // remove persistent from dictionary if don't have listeners anymore
                {
                    _onSaveSpecific.Remove(persistent);
                }
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
                if (_onLoadSpecific[persistent] == null)  // remove persistent from dictionary if don't have listeners anymore
                {
                    _onLoadSpecific.Remove(persistent);
                }
            }
        }

        public bool HasBeenSaved(ScriptableObject persistent)
        {
            return _savedInScenes.ContainsKey(persistent);
        }
        
        public bool HasBeenLoaded(ScriptableObject persistent)
        {
            return _loadedInScenes.ContainsKey(persistent);
        }
        
        public bool HasBeenSavedInCurrentScene(ScriptableObject persistent)
        {
            if (TryGetLastSceneSaved(persistent, out var scene))
            {
                return SceneManager.GetActiveScene().name == scene;
            }
            return false;
        }
        
        public bool HasBeenLoadedInCurrentScene(ScriptableObject persistent)
        {
            if (TryGetLastSceneLoaded(persistent, out var scene))
            {
                return SceneManager.GetActiveScene().name == scene;
            }
            return false;
        }
        
        public bool TryGetLastSceneSaved(ScriptableObject persistent, out string scene)
        {
            if (_savedInScenes.ContainsKey(persistent))
            {
                scene = _savedInScenes[persistent];
                return true;
            }
            scene = "";
            return false;
        }
        
        public bool TryGetLastSceneLoaded(ScriptableObject persistent, out string scene)
        {
            if (_loadedInScenes.ContainsKey(persistent))
            {
                scene = _loadedInScenes[persistent];
                return true;
            }
            scene = "";
            return false;
        }

        public async Task WaitToBeSavedAsync(ScriptableObject persistent, CancellationToken ct)
        {
            bool saved = false;
            void OnSave()
            {
                saved = true;
            }
            
            RegisterOnSaveListenerForObject(persistent, OnSave);
            try
            {
                while (!saved && !ct.IsCancellationRequested)
                {
                    await Task.Yield();
                }
            }
            finally
            {
                UnregisterOnSaveListenerForObject(persistent, OnSave);
            }
        }
        
        public async Task WaitToBeSavedOrAlreadySavedAsync(ScriptableObject persistent, CancellationToken ct)
        {
            if (!HasBeenSaved(persistent))
            {
                await WaitToBeSavedAsync(persistent, ct);
            }
        }
        
        public async Task WaitToBeLoadAsync(ScriptableObject persistent, CancellationToken ct)
        {
            bool loaded = false;
            void OnLoad()
            {
                loaded = true;
            }
            
            RegisterOnLoadListenerForObject(persistent, OnLoad);
            try
            {
                while (!loaded && !ct.IsCancellationRequested)
                {
                    await Task.Yield();
                }
            }
            finally
            {
                UnregisterOnLoadListenerForObject(persistent, OnLoad);
            }
        }
        
        public async Task WaitToBeLoadOrAlreadyLoadAsync(ScriptableObject persistent, CancellationToken ct)
        {
            if (!HasBeenLoaded(persistent))
            {
                await WaitToBeLoadAsync(persistent, ct);
            }
        }
    }
}