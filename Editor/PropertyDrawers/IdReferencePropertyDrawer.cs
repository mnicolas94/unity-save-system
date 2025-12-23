using SaveSystem.Utilities;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SaveSystem.Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(IdReference<>), true)]
    public class IdReferencePropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var idProperty = property.FindPropertyRelative(IdReference.IdFieldName);
            var idReference = (IdReference) property.boxedValue;

            var field = new ObjectField();
            field.objectType = idReference.GetReferencedType();
            field.label = property.displayName;
            field.AddToClassList("unity-base-field__aligned");  // make widths aligned with other fields in inspector
            
            var currentObject = idReference.Object;
            field.SetValueWithoutNotify(currentObject);
            field.RegisterValueChangedCallback((evt) =>
            {
                var newObject = evt.newValue;
                if (SaveSystemSettings.Instance.GuidsResolver.TryGetGuid(newObject, out var newId))
                {
                    idProperty.stringValue = newId;
                    idProperty.serializedObject.ApplyModifiedProperties();
                }
                else
                {
                    field.SetValueWithoutNotify(evt.previousValue);
                    Debug.LogWarning($"No guid found for {newObject}. You might solve it by populating the guids database");
                }
            });
            
            return field;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var idProperty = property.FindPropertyRelative(IdReference.IdFieldName);
            var idReference = (IdReference) property.boxedValue;

            EditorGUI.BeginChangeCheck();
            var newObject = EditorGUI.ObjectField(
                position,
                property.displayName,
                idReference.Object,
                idReference.GetReferencedType(),
                false);
            if (EditorGUI.EndChangeCheck())
            {
                if (SaveSystemSettings.Instance.GuidsResolver.TryGetGuid(newObject, out var newId))
                {
                    idProperty.stringValue = newId;
                    idProperty.serializedObject.ApplyModifiedProperties();
                }
                else if (newObject == null)
                {
                    idProperty.stringValue = "";
                    idProperty.serializedObject.ApplyModifiedProperties();
                }
                else
                {
                    Debug.LogWarning($"No guid found for {newObject}. You might solve it by populating the guids database");
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}