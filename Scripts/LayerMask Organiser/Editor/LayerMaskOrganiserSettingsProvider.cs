using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Benito.ScriptingFoundations.Utilities.Editor;

namespace Benito.ScriptingFoundations.LayerMaskOrganiser.Editor
{
    static class LayerMaskOrganiserSettingsProvider
    {
        static Vector2 scroll = Vector2.zero;
        static int descriptionHeight = 60;

        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            return new SettingsProvider("Benitos Foundations/Layer Mask Organiser", SettingsScope.Project)
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

                serializedSettings.Update();

                List<(int,string)> allNonNullLayers = new List<(int,string)>();
                for (int i = 0; i < 31; i++)
                {
                    string layerName = UnityEngine.LayerMask.LayerToName(i);
                    if (layerName.Length > 0)
                        allNonNullLayers.Add((i,layerName));
                }

                descriptionHeight = EditorGUILayout.IntSlider("descriptionHeight", descriptionHeight, 35, 120);

                scroll = GUILayout.BeginScrollView(scroll);
                {
                    foreach ((int id, string name) currentLayer in allNonNullLayers)
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.BeginVertical();
                            {
                                EditorGUILayout.LabelField($"{currentLayer.id} - {currentLayer.name}", GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.25f));
                                settings.layerInfosList[currentLayer.id].type = (LayerMaskOrganiser.LayerType)EditorGUILayout.EnumPopup(settings.layerInfosList[currentLayer.id].type, GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.25f));
                            }
                            EditorGUILayout.EndVertical();

                            settings.layerInfosList[currentLayer.id].description = EditorGUILayout.TextField(settings.layerInfosList[currentLayer.id].description, GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.4f), GUILayout.Height(descriptionHeight));
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
                GUILayout.EndScrollView();

                EditorUtility.SetDirty(settings);
                serializedSettings.ApplyModifiedProperties();
            }
            else
            {
                EditorGUILayout.HelpBox($"Please Assign or Create and Assign a valid {typeof(LayerMaskOrganiser)} Asset. \n The default location is \"Resources/Settings/Global Managers Settings\" ", MessageType.Error);
            }
        }
    }
}

