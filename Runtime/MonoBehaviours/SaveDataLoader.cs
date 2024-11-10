using System;
using System.Collections.Generic;
using System.Threading;
using SaveSystem.Utilities;
using UnityEngine;
using UnityEngine.Events;

namespace SaveSystem
{
    public class SaveDataLoader : MonoBehaviour
    {
        [SerializeField] private UnityEvent _onLoadingFinish;
        [SerializeField] private UnityEvent<float> _onLoadingProgress;
        [SerializeField] private List<ScriptableObject> _data;
        [SerializeField] private bool _loadOnStart = true;
        
        private void Start()
        {
            if (_loadOnStart)
            {
                LoadSaveDataAsync(destroyCancellationToken);
            }
        }

        public async void LoadSaveDataAsync(CancellationToken ct)
        {
            float loadPersistentsProgress = 0.5f;
            _onLoadingProgress.Invoke(loadPersistentsProgress);
            int count = _data.Count;
            for (int i = 0; i < count; i++)
            {
                var persistent = _data[i];
                if (persistent is SaveGroup group)
                {
                    await group.LoadOrCreate();
                }
                else
                {
                    await persistent.LoadOrCreate();
                }
                
                if (ct.IsCancellationRequested)
                    break;
                
                float progress = loadPersistentsProgress + (float)(i + 1) / count * (1 - loadPersistentsProgress);
                _onLoadingProgress.Invoke(progress);
            }

            if (!ct.IsCancellationRequested)
            {
                _onLoadingFinish.Invoke();
            }
        }
    }
}