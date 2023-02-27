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
            var scriptingBackend = PlayerSettings.GetScriptingBackend(report.summary.platformGroup);
            if (scriptingBackend == ScriptingImplementation.IL2CPP)
            {
                ScanAndGenerateDll();
            }
        }

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