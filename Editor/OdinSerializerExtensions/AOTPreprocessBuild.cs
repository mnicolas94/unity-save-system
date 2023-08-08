using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace SaveSystem.Editor.OdinSerializerExtensions
{
    public class AOTPreprocessBuild : IPreprocessBuildWithReport
    {
        private const string DllName = "com.facticus.savesystem.aot";
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            var scriptingBackend = PlayerSettings.GetScriptingBackend(report.summary.platformGroup);
            if (scriptingBackend == ScriptingImplementation.IL2CPP)
            {
                ScanAndGenerateDll();
            }
            else
            {
                // remove AOT dll to avoid old assemblies to be referenced
                var pathsToDelete = new List<string>
                {
                    Path.Combine(Application.dataPath, $"{DllName}.dll"),
                    Path.Combine(Application.dataPath, "link.xml"),
                };
                foreach (var path in pathsToDelete)
                {
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                }
            }
        }

        public static void ScanAndGenerateDll()
        {
            if (AOTSupportUtilities.ScanProjectForSerializedTypes(out var types))
            {
                OdinSerializer.OdinSerializer.Editor.AOTSupportUtilities.GenerateDLL(
                    Application.dataPath,
                    DllName,
                    types
                );
            }
        }
    }
}