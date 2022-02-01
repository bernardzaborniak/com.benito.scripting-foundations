using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Benito.ScriptingFoundations.Editor.Tools
{
    public class FindMissingScriptsInScene : EditorWindow
    {
        float buttonHeight = 28f;

        [MenuItem("Tools/Benito/Find Missing Scripts in Scene")]
        public static void ShowWindow()
        {
            var window = GetWindow<FindMissingScriptsInScene>("Find Missing Scripts in Scene");
            window.Show();
        }

        Vector2 scroll;
        private HashSet<GameObject> goWithMissingScripts = new HashSet<GameObject>();

        private void OnGUI()
        {
            EditorGUILayout.Space();

            if (GUILayout.Button("Search", GUILayout.Width(position.width), GUILayout.Height(buttonHeight)))
            {
                SearchForMissingComponents();
            }

            DisplayGoWithMissingComponents();
        }

        void SearchForMissingComponents()
        {
            GameObject[] allGoInScene = FindObjectsOfType<GameObject>(true);
            goWithMissingScripts = new HashSet<GameObject>();

            for (int i = 0; i < allGoInScene.Length; i++)
            {
                if (EditorUtility.DisplayCancelableProgressBar($"Looking for Missing Components...", allGoInScene[i].name, (i / (float)allGoInScene.Length)))
                    break;

                Component[] components = allGoInScene[i].GetComponents<Component>();

                for (int j = 0; j < components.Length; j++)
                {
                    if (components[j] == null)
                    {
                        goWithMissingScripts.Add(allGoInScene[i]);
                    }
                }
            }

            EditorUtility.ClearProgressBar();
        }

        void DisplayGoWithMissingComponents()
        {
            if (goWithMissingScripts.Count > 0)
            {
                if (GUILayout.Button("Remove all missing components", GUILayout.Width(position.width), GUILayout.Height(buttonHeight)))
                {
                    foreach (GameObject go in goWithMissingScripts)
                    {
                        GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
                    }

                    // Update View again;
                    SearchForMissingComponents();
                }

                scroll = GUILayout.BeginScrollView(scroll);
                {

                    foreach (GameObject go in goWithMissingScripts)
                    {
                        if (goWithMissingScripts == null)
                            continue;

                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Label(go.name, GUILayout.Width(position.width / 2));
                            if (GUILayout.Button("Select", GUILayout.Width(position.width / 4 - 10)))
                            {
                                Selection.activeObject = go;
                            }
                            if (GUILayout.Button("Remove missing components", GUILayout.Width(position.width / 4 - 10)))
                            {
                                GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
                            }
                        }
                        GUILayout.EndHorizontal();
                    }

                }
                GUILayout.EndScrollView();
            }
            else
            {
                EditorGUILayout.LabelField("No objects found");
            }
        }

    }
}
