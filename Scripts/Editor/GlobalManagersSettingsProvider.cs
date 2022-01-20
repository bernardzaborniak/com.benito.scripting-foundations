using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

/// <summary>
/// Responsible for displaying the Global Managers Settings in the project Settings Window.
/// </summary>

namespace Benitos.ScriptingFoundations.Managers.Editor
{
    // Register a SettingsProvider using IMGUI for the drawing framework:
    // https://docs.unity3d.com/2019.1/Documentation/ScriptReference/SettingsProvider.html
    static class GlobalManagersSettingsProvider
    {
        [SettingsProvider]
        public static SettingsProvider CreateBootstrapperSettingsProvider()
        {
            // First parameter is the path in the Settings window.
            // Second parameter is the scope of this setting: it only appears in the Project Settings window.
            var provider = new SettingsProvider("Benitos Foundations/Global Managers Settings", SettingsScope.Project)
            {
                // By default the last token of the path is used as display name if no label is provided.
                label = "Global Managers Settings",
                // Create the SettingsProvider and initialize its drawing (IMGUI) function in place:
                guiHandler = (searchContext) =>
                {
                    GlobalManagersSettings settings = GlobalManagersSettings.GetOrCreateSettings();

                    //settings = (GlobalManagersSettings)EditorGUILayout.ObjectField("Global Managers Settings", settings, typeof(GlobalManagersSettings), false);

                    if (settings != null)
                    {
                        SerializedObject serializedSettings = new SerializedObject(settings);
                        EditorGUILayout.PropertyField(serializedSettings.FindProperty("globalManagersPrefab"));
                        serializedSettings.ApplyModifiedProperties();

                        //Check if the assinged Prefab is correct
                        if (settings.globalManagersPrefab.GetComponentInChildren<GlobalSingletonManager>() == null)
                            EditorGUILayout.HelpBox("Make sure the assigned Prefab has a GlobalSingletonManager(parent or child) on it", MessageType.Error);

                    }
                    else
                    {
                        EditorGUILayout.HelpBox("Please Assign or Create and Assign a valid Global Managers Settings Asset. \n The default location is \"Resources/Settings/Global Managers Settings\" ", MessageType.Error);
                    }
                },

                // Populate the search keywords to enable smart search filtering and label highlighting:
                keywords = new HashSet<string>(new[] { "Managers, Global, Preload" })
            };

            return provider;
        }
    }
}
