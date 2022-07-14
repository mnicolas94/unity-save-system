using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Debug = UnityEngine.Debug;

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

        [MenuItem("Tools/Facticus/SaveSystem/Populate guids database")]
        public static void PopulateDatabase()
        {
            var paths = SaveSystemSettings.Instance.SearchDatabaseAssetsInPaths;
            var objects = AssetDatabase.FindAssets("", paths);
            var objectsGuids = objects.Where(guid =>
            {
                string objPath = AssetDatabase.GUIDToAssetPath(guid);
                bool isFolder = AssetDatabase.IsValidFolder(objPath);
                return !isFolder;
            }).Select(guid =>
            {
                string objPath = AssetDatabase.GUIDToAssetPath(guid);
                var obj = AssetDatabase.LoadAssetAtPath<Object>(objPath);
                return (obj, guid);
            });
            AssetGuidsDatabase.PopulateDatabase(objectsGuids.ToList());
            EditorUtility.SetDirty(AssetGuidsDatabase.Instance);
            AssetDatabase.SaveAssets();
        }
    }
}