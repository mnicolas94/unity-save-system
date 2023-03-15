using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace SaveSystem
{
    public class SaveDataLoader : MonoBehaviour
    {
        [SerializeField] private UnityEvent _onLoadingFinish;
        [SerializeField] private UnityEvent<float> _onLoadingProgress;
        [SerializeField] private List<ScriptableObject> _data;

        private CancellationTokenSource _cts;

        private void Start()
        {
            _cts = new CancellationTokenSource();
            LoadSaveDataAsync(_cts.Token);
        }

        private void OnDisable()
        {
            _cts.Cancel();
            _cts.Dispose();
        }

        private async void LoadSaveDataAsync(CancellationToken ct)
        {
            float loadPersistentsProgress = 0.5f;
            _onLoadingProgress.Invoke(loadPersistentsProgress);
            int count = _data.Count;
            for (int i = 0; i < count; i++)
            {
                if (ct.IsCancellationRequested)
                    break;
                
                var persistent = _data[i];
                await persistent.LoadOrCreate();
                
                float progress = loadPersistentsProgress + (float)(i + 1) / count * (1 - loadPersistentsProgress);
                _onLoadingProgress.Invoke(progress);
            }
            _onLoadingFinish.Invoke();
        }
    }
}