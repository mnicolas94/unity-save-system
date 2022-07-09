using UnityEditor;

namespace SaveSystem.Editor
{
    public static class SaveDataEditorUtils
    {
        [MenuItem("Tools/Facticus/SaveSystem/Reset data")]
        public static void ResetData()
        {
            var persistentObjects = SaveUtils.GetAllPersistentObjects();
            foreach (var persistent in persistentObjects)
            {
                persistent.ResetToDefault();
            }
        }
    }
}