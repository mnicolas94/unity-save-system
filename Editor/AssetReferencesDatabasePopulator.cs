using System;
using System.Collections.Generic;
using System.Linq;
using SaveSystem.GuidsResolve;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace SaveSystem.Editor
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
            var objectsGuids = SaveDataEditorUtils.GetDataObjectsAndGuids();
            var saveSystemSettings = SaveSystemSettings.Instance;
            saveSystemSettings.GuidsResolver.PopulateDatabase(objectsGuids);
            EditorUtility.SetDirty(saveSystemSettings);
            AssetDatabase.SaveAssets();
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
}