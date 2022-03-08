using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Benito.ScriptingFoundations.Utilities.Editor;


namespace Benito.ScriptingFoundations.Managers.Editor
{
    // Register a SettingsProvider using IMGUI for the drawing framework: https://docs.unity3d.com/2019.1/Documentation/ScriptReference/SettingsProvider.html
    static class GlobalManagersSettingsProvider
    {
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            return new SettingsProvider("Benitos Foundations/Global Managers Settings", SettingsScope.Project)
            {
                label = "Global Managers Settings",
                guiHandler = DrawGUI,
                keywords = new HashSet<string>(new[] { "Managers, Global, Preload" })
            };
        }

        static void DrawGUI(string searchContext)
        {
            GlobalManagersSettings settings = GlobalManagersSettings.GetOrCreateSettings();

            if (settings != null)
            {
                SerializedObject serializedSettings = new SerializedObject(settings);
                EditorUtilities.DrawDefaultInspectorForSerializedObject(serializedSettings);

                //Check if the assinged Prefab is correct
                if(settings.globalManagersPrefab != null)
                {
                    if (settings.globalManagersPrefab?.GetComponentInChildren<GlobalManagers>() == null)
                        EditorGUILayout.HelpBox("Make sure the assigned Prefab has a GlobalSingletonManager(parent or child) on it", MessageType.Error);
                }
                else
                {
                    EditorGUILayout.HelpBox($"Please Assign or Create and Assign a {typeof(GlobalManagers)} Asset. \n The default location is Benitos Scripting Foundations/Prefabs/ ", MessageType.Error);                   
                }
            }
            else
            {
                EditorGUILayout.HelpBox($"Please Assign or Create and Assign a valid {typeof(GlobalManagersSettings)} Asset. \n The default location is \"Resources/Settings/Global Managers Settings\" ", MessageType.Error);
            }
        }
    }
}
