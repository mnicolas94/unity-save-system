using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

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
                activateHandler = (searchContext, rootElement) =>
                {
                    var inspector = new InspectorElement(settings);
                    var scroll = new ScrollView(ScrollViewMode.Vertical);
                    scroll.Add(inspector);
                    rootElement.Add(scroll);
                },
                keywords = keywords,
            };
            
            return provider;
        }
    }
}