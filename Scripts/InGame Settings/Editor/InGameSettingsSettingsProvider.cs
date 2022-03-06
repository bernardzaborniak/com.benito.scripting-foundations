using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Benito.ScriptingFoundations.Utilities.Editor;

namespace Benito.ScriptingFoundations.InGameSettings.Editor
{
    public class InGameSettingsSettingsProvider 
    {
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            return new SettingsProvider("Benitos Foundations/Ingame Settings Settings", SettingsScope.Project)
            {
                label = "InGame Settings Settings",
                guiHandler = DrawGUI,
                keywords = new HashSet<string>(new[] { "InGame Settings, Settings, InGame" })
            };
        }

        static void DrawGUI(string searchContext)
        {
            InGameSettingsSettings settings = InGameSettingsSettings.GetOrCreateSettings();

            if (settings != null)
            {
                SerializedObject serializedSettings = new SerializedObject(settings);
                EditorUtilities.DrawDefaultInspectorForSerializedObject(serializedSettings);

                //Check if the path is not null
                if (settings.GetInGameSettingsFolderPath() == string.Empty)
                {
                    EditorGUILayout.HelpBox("Please assign a correct InGameSettings Folder Path", MessageType.Error);
                }
            }
            else
            {
                EditorGUILayout.HelpBox($"Please Assign or Create and Assign a valid {typeof(InGameSettingsSettings)} Asset. \n The default location is \"Resources/Settings/InGame Settings\" ", MessageType.Error);
            }
        }
    }
}


