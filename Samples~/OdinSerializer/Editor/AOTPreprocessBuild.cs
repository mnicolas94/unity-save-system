using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SaveSystem.Serializers;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

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
                // add additional types
                if (SaveSystemSettings.Instance.Serializer is OdinPersistentSerializer odinSerializer)
                {
                    var additionalTypes = odinSerializer.AOTAdditionalTypes.Select(GetTypeByName);
                    types.AddRange(additionalTypes);
                }
                OdinSerializer.OdinSerializer.Editor.AOTSupportUtilities.GenerateDLL(
                    Application.dataPath,
                    DllName,
                    types
                );
            }
        }
        
        public static Type GetTypeByName(string name)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var tt = assembly.GetType(name);
                if (tt != null)
                {
                    return tt;
                }
            }

            return null;
        }
    }
}