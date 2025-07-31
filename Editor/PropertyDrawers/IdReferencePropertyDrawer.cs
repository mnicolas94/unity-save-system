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
            var boxedValue = property.boxedValue;

            var field = new ObjectField();
            field.objectType = boxedValue.GetType().GenericTypeArguments[0];
            field.label = property.displayName;
            field.AddToClassList("unity-base-field__aligned");  // make widths aligned with other fields in inspector
            
            var currentObject = (boxedValue as IdReference).Object;
            field.SetValueWithoutNotify(currentObject);
            field.RegisterValueChangedCallback((evt) =>
            {
                var newObject = evt.newValue;
                if (SaveSystemSettings.Instance.GuidsResolver.TryGetGuid(newObject, out var newId))
                {
                    idProperty.stringValue = newId;
                    idProperty.serializedObject.ApplyModifiedProperties();
                }
            });
            
            return field;
        }
    }
}