using System;
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
            #if UNITY_EDITOR
            await Task.Yield();
            _onLoadingProgress.Invoke(0.9f);
            #else
            float loadPersistentsProgress = 0.5f;
            var persistentObjects = await SaveUtils.GetAllPersistentObjectsAsync(ct);
            _onLoadingProgress.Invoke(loadPersistentsProgress);
            int count = persistentObjects.Count;
            for (int i = 0; i < count; i++)
            {
                var persistent = persistentObjects[i];
                Debug.Log($"- SaveDataLoader.LoadSaveDataAsync: loading {persistent.PersistentFileName}");
                var loadReport = new SaveUtils.LoadReport{Success = false};
                try
                {
                    loadReport = persistent.Load();
                }
                catch (Exception e)
                {
                    Debug.LogError($"An exception was thrown during data loading: {persistent.PersistentFileName}");
                    Debug.LogError(e.ToString());
                }
                
                if (!loadReport.Success)
                {
                    persistent.ResetToDefault();
                    persistent.Save();
                }
                
                Debug.Log($"- SaveDataLoader.LoadSaveDataAsync: load report:\n{loadReport.ToString()}");

                if (ct.IsCancellationRequested)
                    break;
                
                await Task.Yield();
                float progress = loadPersistentsProgress + (float)(i + 1) / count * (1 - loadPersistentsProgress);
                _onLoadingProgress.Invoke(progress);
            }
            #endif
            _onLoadingFinish.Invoke();
        }
    }
}