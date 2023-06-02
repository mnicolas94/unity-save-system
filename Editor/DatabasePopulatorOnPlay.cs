using UnityEditor;
using UnityEngine;

namespace SaveSystem.Editor
{
    [InitializeOnLoad]
    public static class DatabasePopulatorOnPlay
    {
        // register an event handler when the class is initialized
        static DatabasePopulatorOnPlay()
        {
            EditorApplication.playModeStateChanged += LogPlayModeState;
        }

        private static void LogPlayModeState(PlayModeStateChange state)
        {
            var aboutToEnterPlayMode = state == PlayModeStateChange.ExitingEditMode;
            var populateOnPlayMode = SaveSystemSettings.Instance.PopulateDatabaseBeforeEnterPlayMode;
            if (aboutToEnterPlayMode && populateOnPlayMode)
            {
                AssetReferencesDatabasePopulator.PopulateDatabase();
            }
        }
    }
}