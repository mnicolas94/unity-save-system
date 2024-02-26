using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace SaveSystem.Editor.StoredDataVisualizationWindow
{
    public class SaveVisualizer : EditorWindow
    {
        [SerializeField] private VisualTreeAsset _visualTreeAsset;
        
        private Label _errorMessagesLabel;
        private VisualElement _rootContainer;
        private Label _encryptedLabel;
        private Label _decryptedLabel;
        private Label _loadReportLabel;

        [MenuItem("Tools/Facticus/Save System/SaveVisualizer")]
        public static void ShowExample()
        {
            SaveVisualizer wnd = GetWindow<SaveVisualizer>();
            wnd.titleContent = new GUIContent("Saves Visualizer");
        }

        private void OnEnable()
        {
            Selection.selectionChanged += DisplaySelectedAsset;
        }

        private void OnDisable()
        {
            Selection.selectionChanged -= DisplaySelectedAsset;
        }

        public void CreateGUI()
        {
            var root = rootVisualElement;

            // load visual tree
            _visualTreeAsset = Resources.Load<VisualTreeAsset>("SaveVisualizer");
            var visualTree = _visualTreeAsset.Instantiate();
            root.Add(visualTree);

            _errorMessagesLabel = visualTree.Q<Label>("ErrorMessagesLabel");
            _rootContainer = visualTree.Q("RootContainer");
            _encryptedLabel = visualTree.Q<Label>("EncryptedDataLabel");
            _decryptedLabel = visualTree.Q<Label>("DecryptedDataLabel");
            _loadReportLabel = visualTree.Q<Label>("LoadReportLabel");
            
            DisplaySelectedAsset();
        }

        private async void DisplaySelectedAsset()
        {
            var selected = Selection.GetFiltered<ScriptableObject>(SelectionMode.Assets);

            bool displayError = false;
            if (selected.Length == 0)
            {
                // clear editor
                displayError = true;
                _errorMessagesLabel.text = "No asset selected";
            }
            else if (selected.Length > 1)
            {
                // clear editor
                // show message of not support for multiple assets
                displayError = true;
                _errorMessagesLabel.text = "Multiple assets not supported";
            }
            else
            {
                displayError = false;
                try
                {
                    var asset = selected[0];
                    var report = await asset.Load();

                    _decryptedLabel.text = report.DataStr;
                    _loadReportLabel.text = report.ToString();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
            
            _errorMessagesLabel.style.display = displayError ? DisplayStyle.Flex : DisplayStyle.None;
            _rootContainer.style.display = displayError ? DisplayStyle.None : DisplayStyle.Flex;
        }
    }
}
