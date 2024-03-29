﻿using SaveSystem.GuidsResolve;
using UnityEditor;
using UnityEngine;
using Utils.Editor;
using Utils.Editor.EditorGUIUtils;

namespace SaveSystem.Editor
{
    public static class SaveSystemSettingsProvider
    {
        [SettingsProvider]
        public static SettingsProvider GetSettingsProvider()
        {
            bool existsSettings = SaveSystemSettings.Instance != null;
            SerializedObject so = existsSettings ? new SerializedObject(SaveSystemSettings.Instance) : null;
            var keywords = existsSettings ? SettingsProvider.GetSearchKeywordsFromSerializedObject(so) : new string[0];
            var provider = new SettingsProvider("Project/Facticus/Save system", SettingsScope.Project)
            {
                guiHandler = (searchContext) =>
                {
                    EditorGUILayout.Space(12);
                    
                    if (existsSettings)
                        GUIUtils.DrawSerializedObject(so);
                    else
                    {
                        var r = EditorGUILayout.GetControlRect();
                        if (GUI.Button(r, "Create settings"))
                        {
                            var settings = ScriptableObject.CreateInstance<SaveSystemSettings>();
                            AssetDatabase.CreateAsset(settings, "Assets/SaveSettings.asset");
                        }
                    }
                },
                keywords = keywords
            };
            
            return provider;
        }
    }
}