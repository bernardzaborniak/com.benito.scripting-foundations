using System.Collections.Generic;
using UnityEditor;
using Benito.ScriptingFoundations.Utilities.Editor;

namespace Benito.ScriptingFoundations.LayerMaskOrganiser.Editor
{
    static class LayerMaskOrganiserSettingsProvider
    {
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            return new SettingsProvider("Benitos Foundations/LayerMask Organiser", SettingsScope.Project)
            {
                label = "LayerMask Organiser",
                guiHandler = DrawGUI,
                keywords = new HashSet<string>(new[] { "LayerMask, Layer, Organisation, Organiser" })
            };
        }

        static void DrawGUI(string searchContext)
        {
            LayerMaskOrganiser settings = LayerMaskOrganiser.GetOrCreateSettings();

            if (settings != null)
            {
                SerializedObject serializedSettings = new SerializedObject(settings);
                EditorUtilities.DrawDefaultInspectorForSerializedObject(serializedSettings);
            }
            else
            {
                EditorGUILayout.HelpBox($"Please Assign or Create and Assign a valid {typeof(LayerMaskOrganiser)} Asset. \n The default location is \"Resources/Settings/Global Managers Settings\" ", MessageType.Error);
            }
        }
    }
}

