﻿using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace SaveSystem.Editor.GuidsResolve
{
    public class AssetReferencesDatabasePopulator : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            Debug.Log("Populating assets GUIDs database before build");
            PopulateDatabase();
        }

        [MenuItem("Tools/Facticus/Save System/Populate guids database")]
        public static void PopulateDatabase()
        {
            var objectsGuids = AssetReferencesScanUtils.GetDataObjectsAndGuids();
            var objectsGuidsTuple = objectsGuids.ConvertAll(objGuid =>
            {
                var (obj, guid) = objGuid;
                return (obj, guid);
            });
            var saveSystemSettings = SaveSystemSettings.Instance;
            saveSystemSettings.GuidsResolver.PopulateDatabase(objectsGuidsTuple);
            EditorUtility.SetDirty(saveSystemSettings);
            AssetDatabase.SaveAssetIfDirty(saveSystemSettings);
            try
            {
                EditorWindow.focusedWindow.ShowNotification(new GUIContent("Database populated successfully"));
            }
            catch
            {
                // ignore
            }
        }
    }
    
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