using SaveSystem.GuidsResolve;
using UnityEditor;
using UnityEngine;
using Utils.Editor;
using Utils.Editor.EditorGUIUtils;

namespace SaveSystem.Editor
{
    public static class SaveSystemSettingsProvider
    {
        [SettingsProvider]
        public static SettingsProvider GetSettingsProvider()
        {
            var settings = SaveSystemSettings.Editor_GetOrCreate();
            SerializedObject so = new SerializedObject(settings);
            var keywords = SettingsProvider.GetSearchKeywordsFromSerializedObject(so);
            var provider = new SettingsProvider("Project/Facticus/Save system", SettingsScope.Project)
            {
                guiHandler = (searchContext) =>
                {
                    EditorGUILayout.Space(12);
                    GUIUtils.DrawSerializedObject(so);
                },
                keywords = keywords
            };
            
            return provider;
        }
    }
}