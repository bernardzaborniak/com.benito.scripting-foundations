using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Benito.ScriptingFoundations.Utilities.Editor;

namespace Benito.ScriptingFoundations.Modding.Editor
{
    static class ModdingSettingsProvider 
    {
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            return new SettingsProvider("Benitos Foundations/Modding Settings", SettingsScope.Project)
            {
                label = "Modding Settings",
                guiHandler = DrawGUI,
                keywords = new HashSet<string>(new[] { "Modding, Custom Level, Mod" })
            };
        }

        static void DrawGUI(string searchContext)
        {
            ModdingSettings settings = ModdingSettings.GetOrCreateSettings();

            if (settings != null)
            {
                SerializedObject serializedSettings = new SerializedObject(settings);
                EditorUtilities.DrawDefaultInspectorForSerializedObject(serializedSettings);

                //Check if the path is not null
                if (settings.GetModFolderPath() == string.Empty)
                {
                     EditorGUILayout.HelpBox("Please assign a correct Mod Folder Path", MessageType.Error);
                }
            }
            else
            {
                EditorGUILayout.HelpBox($"Please Assign or Create and Assign a valid {typeof(ModdingSettings)} Asset. \n The default location is \"Resources/Settings/Modding Settings\" ", MessageType.Error);
            }
        }
    }
}

