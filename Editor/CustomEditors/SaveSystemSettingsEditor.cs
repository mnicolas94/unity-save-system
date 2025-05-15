using System;
using System.Collections.Generic;
using SaveSystem.Editor.GuidsResolve;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Utils.Editor;

namespace SaveSystem.Editor.CustomEditors
{
    [CustomEditor(typeof(SaveSystemSettings))]
    public class SaveSystemSettingsEditor : UnityEditor.Editor
    {
        private readonly Dictionary<string, Func<SerializedProperty, VisualElement>> _propertyDrawers;

        public SaveSystemSettingsEditor()
        {
            _propertyDrawers = new()
            {
                {SaveSystemSettings.GuidsResolverFieldName, GuidsResolverFieldDrawer},
            };
        }

        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            
            var properties = PropertiesUtils.GetSerializedProperties(serializedObject, false, false);
            foreach (var serializedProperty in properties)
            {
                VisualElement propertyField;
                if (_propertyDrawers.TryGetValue(serializedProperty.name, out var propertyDrawerFactory))
                {
                    propertyField = propertyDrawerFactory(serializedProperty);
                }
                else
                {
                    propertyField = new PropertyField(serializedProperty);
                }
                root.Add(propertyField);
            }

            return root;
        }
        
        private VisualElement GuidsResolverFieldDrawer(SerializedProperty property)
        {
            var container = new VisualElement();
            var propertyField = new PropertyField(property);
            
            var filtersStorage = AssetReferencesFilterStorage.GetOrCreate();
            var filterStorageElement = new InspectorElement(filtersStorage);
            filterStorageElement.SetEnabled(false);
            var filtersFoldout = new Foldout();
            filtersFoldout.text = "References Filters";
            filtersFoldout.Add(filterStorageElement);
                
            container.Add(propertyField);
            container.Add(filtersFoldout);
            return container;
        }
    }
}