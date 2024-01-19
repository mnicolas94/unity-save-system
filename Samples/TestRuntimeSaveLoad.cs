using System.Threading.Tasks;
using SaveSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Samples
{
    public class TestRuntimeSaveLoad : MonoBehaviour
    {
        [SerializeField] private TestRuntimeSaveLoadData _data;

        [SerializeField] private Button _button;
        
        [SerializeField] private float _headerScale;
        [SerializeField] private TextMeshProUGUI _beforeSaveText;
        [SerializeField] private TextMeshProUGUI _afterSaveChangeText;
        [SerializeField] private TextMeshProUGUI _afterLoadText;
        
        private async void Start()
        {
            await SaveAndLoad();
            _button.onClick.AddListener(OnClick);
        }

        private async void OnClick()
        {
            _button.interactable = false;
            
            await SaveAndLoad();
            
            _button.interactable = true;
        }

        private async Task SaveAndLoad()
        {
            _data.Randomize();
            SetDataToText(_beforeSaveText);
            await _data.Save();

            _data.Randomize();
            SetDataToText(_afterSaveChangeText);

            await _data.Load();
            SetDataToText(_afterLoadText);
        }

        private void SetDataToText(TextMeshProUGUI text)
        {
            var json = JsonUtility.ToJson(_data, true);
            text.text = $"<size={_headerScale * 100}%>{text.name}:\n<size=100%>{json}";
        }
    }
}