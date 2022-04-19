using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Benito.ScriptingFoundations.Utilities.Editor;

namespace Benito.ScriptingFoundations.BindingOverrides.Editor
{
    public class BindingOverridesSettingsSettingsProvider 
    {
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            return new SettingsProvider("Benitos Foundations/Binding Overrides Settings", SettingsScope.Project)
            {
                label = "Binding Overrides Settings",
                guiHandler = DrawGUI,
                keywords = new HashSet<string>(new[] { "Settings, Bindings, Overrides" })
            };
        }

        static void DrawGUI(string searchContext)
        {
            BindingOverridesSettings settings = BindingOverridesSettings.GetOrCreateSettings();

            if (settings != null)
            {
                SerializedObject serializedSettings = new SerializedObject(settings);
                EditorUtilities.DrawDefaultInspectorForSerializedObject(serializedSettings);

                //Check if the path is not null
                if (settings.GetOverridesFolderPath() == string.Empty)
                {
                    EditorGUILayout.HelpBox("Please assign a correct Settings Folder Path", MessageType.Error);
                }
            }
            else
            {
                EditorGUILayout.HelpBox($"Please Assign or Create and Assign a valid {typeof(BindingOverridesSettings)} Asset. \n The default location is \"Resources/Settings/\" ", MessageType.Error);
            }
        }
    }
}


