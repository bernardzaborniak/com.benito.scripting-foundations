using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace Benito.ScriptingFoundations.Validator.Editor
{
    public class ValidatorWindow : EditorWindow
    {
        float buttonHeight = 28f;
        HashSet<GameObject> gosToValidate = new HashSet<GameObject>();
        Vector2 scroll;

        [MenuItem("Tools/Benito/Validator")]
        public static void ShowWindow()
        {
            var window = GetWindow<ValidatorWindow>("Validator");
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Search in Scene", GUILayout.Width(position.width/3), GUILayout.Height(buttonHeight)))
            {
                SearchForObjectsInScene();
            }

            if (GUILayout.Button("Search in Assets", GUILayout.Width(position.width / 3), GUILayout.Height(buttonHeight)))
            {
                SearchForObjectsInAssets();
            }

            if (GUILayout.Button("Search in Packages", GUILayout.Width(position.width / 3), GUILayout.Height(buttonHeight)))
            {
                SearchForObjectsInPackages();
            }
            EditorGUILayout.EndHorizontal();

            DisplaySearchResult();
        }

        void DisplaySearchResult()
        {
            foreach (GameObject goToValidate in gosToValidate)
            {
                EditorGUILayout.LabelField(goToValidate.name);
            }
        }

        void SearchForObjectsInScene()
        {
            GameObject[] allGoInScene = FindObjectsOfType<GameObject>(true);

            SearchForObjectsWithValidateAttribute(allGoInScene);
        }

        void SearchForObjectsInAssets()
        {

        }

        void SearchForObjectsInPackages()
        {

        }

        void SearchForObjectsWithValidateAttribute(GameObject[] gosToCheck)
        {
            gosToValidate.Clear();

            for (int i = 0; i < gosToCheck.Length; i++)
            {
                if (EditorUtility.DisplayCancelableProgressBar($"Looking for Objects to validate ...", gosToCheck[i].name, (i / (float)gosToCheck.Length)))
                    break;

                Component[] components = gosToCheck[i].GetComponents<Component>();

                for (int j = 0; j < components.Length; j++)
                {
                    if (components[j] == null)
                        continue;


                    FieldInfo[] fieldInfos = components[j].GetType().GetFields(BindingFlags.Instance |BindingFlags.Static |BindingFlags.NonPublic |BindingFlags.Public);
                    
                    foreach (FieldInfo info in fieldInfos)
                    {
                        var customAttribute = info.GetCustomAttribute<ValidateAttribute>();
                        if (customAttribute != null)
                        {
                            gosToValidate.Add(gosToCheck[i]);
                        }
                    }
                }
            }

            EditorUtility.ClearProgressBar();
        }
    }
}
