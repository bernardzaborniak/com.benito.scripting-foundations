using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Benito.ScriptingFoundations.Utilities.Editor;

namespace Benito.ScriptingFoundations.Saving.Editor
{
    public class SavingSettingsProvider
    {
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            return new SettingsProvider("Benitos Foundations/Saving Settings", SettingsScope.Project)
            {
                label = "Saving Settings",
                guiHandler = DrawGUI,
                keywords = new HashSet<string>(new[] { "Saving, Save, Load" })
            };
        }

        static void DrawGUI(string searchContext)
        {
            SavingSettings settings = SavingSettings.GetOrCreateSettings();

            if (settings != null)
            {
                SerializedObject serializedSettings = new SerializedObject(settings);
                EditorUtilities.DrawDefaultInspectorForSerializedObject(serializedSettings);

                //Check if the path is not null
                if (settings.GetSavesFolderPath() == string.Empty)
                {
                    EditorGUILayout.HelpBox("Please assign a correct Saves Folder Path", MessageType.Error);
                }
            }
            else
            {
                EditorGUILayout.HelpBox($"Please Assign or Create and Assign a valid {typeof(SavingSettings)} Asset. \n The default location is \"Resources/Settings/Saving Settings\" ", MessageType.Error);
            }
        }
    }
}
