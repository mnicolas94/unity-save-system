using UnityEditor;
using Utils.Editor;

namespace SaveSystem.Editor
{
    public static class SaveSystemSettingsProvider
    {
        [SettingsProvider]
        public static SettingsProvider GetSettingsProvider()
        {
            var so = new SerializedObject(SaveSystemSettings.Instance);
            var provider = new SettingsProvider("Project/Facticus/Save system", SettingsScope.Project)
            {
                guiHandler = (searchContext) =>
                {
                    EditorGUILayout.Space(12);
                    PropertiesUtils.DrawSerializedObject(so);
                },

                keywords = SettingsProvider.GetSearchKeywordsFromSerializedObject(so)
            };

            
            return provider;
        }
    }
}