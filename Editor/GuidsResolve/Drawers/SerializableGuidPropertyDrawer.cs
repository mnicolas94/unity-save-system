using SaveSystem.GuidsResolve;
using UnityEditor;
using UnityEngine;

namespace SaveSystem.Editor.GuidsResolve.Drawers
{
    [CustomPropertyDrawer(typeof(SerializableGlobalObjectId))]
    public class SerializableGuidPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property,
            GUIContent label)
        {
            var guid = property.boxedValue as SerializableGlobalObjectId;
            EditorGUI.BeginChangeCheck();
            var newObject = EditorGUI.ObjectField(position, label, guid.ObjectReference, typeof(Object), true);
            if (EditorGUI.EndChangeCheck())
            {
                guid.ObjectReference = newObject;
                property.boxedValue = guid;
            }

            property.serializedObject.ApplyModifiedProperties();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }
    }
}