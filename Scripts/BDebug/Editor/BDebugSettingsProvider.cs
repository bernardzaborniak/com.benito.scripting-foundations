using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Benito.ScriptingFoundations.Utilities.Editor;

namespace Benito.ScriptingFoundations.BDebug.Editor
{
    public class BDebugSettingsProvider
    {
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            return new SettingsProvider("Benitos Foundations/BDebug Settings", SettingsScope.Project)
            {
                label = "BDebug Settings",
                guiHandler = DrawGUI,
                keywords = new HashSet<string>(new[] { "Debug, BDebug, Drawing" })
            };
        }

        static void DrawGUI(string searchContext)
        {
            BDebugSettings settings = BDebugSettings.GetOrCreateSettings();

            if (settings != null)
            {
                SerializedObject serializedSettings = new SerializedObject(settings);
                EditorUtilities.DrawDefaultInspectorForSerializedObject(serializedSettings);
            }
            else
            {
                EditorGUILayout.HelpBox($"Please Assign or Create and Assign a valid {typeof(BDebugSettings)} Asset. \n The default location is \"Resources/Settings/Modding Settings\" ", MessageType.Error);
            }
        }
    }
}
