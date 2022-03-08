using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Benito.ScriptingFoundations.Modding.Editor
{
    [CustomEditor(typeof(ModManager))]
    public class ModManagerEditor : UnityEditor.Editor
    {
        ModManager manager;

        private void OnEnable()
        {
            manager = (ModManager)target;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Looking for Mods at: ");
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField(ModdingSettings.GetOrCreateSettings().GetModFolderPath());
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();

            if(GUILayout.Button("Open Mods Folder"))
            {
                EditorUtility.RevealInFinder(ModdingSettings.GetOrCreateSettings().GetModFolderPath());
            }

            EditorGUILayout.LabelField("Available Mods", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            {
                if (manager.GetAvailableMods().Count == 0)
                {
                    EditorGUILayout.LabelField("No Mods found in Mods Folder");
                }
                else
                {
                    foreach (ModInfo info in manager.GetAvailableMods())
                    {
                        EditorGUILayout.LabelField(info.name);
                    }
                }
            }
            EditorGUI.indentLevel--;

            EditorGUILayout.LabelField("Loaded Mods", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            {
                if (manager.GetLoadedMods().Count == 0)
                {
                    EditorGUILayout.LabelField("No Mods loaded");
                }
                else
                {
                    foreach (ModInfo info in manager.GetLoadedMods())
                    {
                        EditorGUILayout.LabelField(info.name);
                    }
                }
            }
            EditorGUI.indentLevel--;
        }
    }

}

