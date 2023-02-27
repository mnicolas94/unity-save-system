using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace SaveSystem.Editor.OdinSerializerExtensions
{
    public class AOTPreprocessBuild : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
#if ENABLE_IL2CPP
            Debug.Log("in IL2CPP");
#else
            Debug.Log("NOT in IL2CPP");
#endif
            
            Debug.Log("Populating assets GUIDs database before build");
            ScanAndGenerateDll();
        }

        [MenuItem("Tools/Facticus/SaveSystem/Scan types and generate DLL")]
        public static void ScanAndGenerateDll()
        {
            if (AOTSupportUtilities.ScanProjectForSerializedTypes(out var types))
            {
                OdinSerializer.OdinSerializer.Editor.AOTSupportUtilities.GenerateDLL(
                    Application.dataPath,
                    "com.facticus.savesystem.aot",
                    types
                );
            }
        }
    }
}