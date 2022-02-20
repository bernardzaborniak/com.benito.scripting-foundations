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
        List<ValidationInfo> objectsToValidate = new List<ValidationInfo>();
        List<ValidationInfo> objectsWithInvalidValues = new List<ValidationInfo>();
        //List<GameObject> gosToValidateInScene = new List<GameObject>();
        //List<GameObject> gosWithInvalidValuesInScene = new List<GameObject>();

        //List<string> prefabsToValidate = new List<string>();
        //List<string> prefabsWithInvalidValues = new List<string>();
        Vector2 scroll;


        public class ValidationInfo
        {
            public enum Type
            {
                Prefab,
                GameObject
            }
            public readonly Type type;

            public readonly ValidateAttribute attribute;
            public readonly string prefabPath;
            public readonly GameObject gameObjectInScene;

            public bool isValid;

            public ValidationInfo(ValidateAttribute attribute, GameObject gameObjectInScene)
            {
                type = Type.GameObject;
                this.attribute = attribute;
                this.gameObjectInScene = gameObjectInScene;
            }

            public ValidationInfo(ValidateAttribute attribute, string prefabPath)
            {
                type = Type.Prefab;
                this.attribute = attribute;
                this.prefabPath = prefabPath;
            }
        }


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

            if (GUILayout.Button("Search in Scene", GUILayout.Width(position.width * 0.49f), GUILayout.Height(buttonHeight)))
            {
                SearchForObjectsInScene();
            }
            if (GUILayout.Button("Search in Prefabs in Assets and Packages", GUILayout.Width(position.width *0.49f), GUILayout.Height(buttonHeight)))
            {
                SearchForPrefabsInFolder();
            }


            EditorGUILayout.EndHorizontal();

            DisplaySearchResult();
        }

        void DisplaySearchResult()
        {
            scroll = GUILayout.BeginScrollView(scroll);
            {
                foreach (ValidationInfo info in objectsToValidate)
                {
                    DrawValidationFailedObjectInScene(info);
                }
            }
            GUILayout.EndScrollView();
        }

        void DrawValidationFailedObjectInScene(ValidationInfo info)
        {
            EditorGUILayout.BeginHorizontal();
            {
                if(info.type == ValidationInfo.Type.GameObject)
                {
                    EditorGUILayout.LabelField(info.gameObjectInScene.name);
                }
                else if(info.type == ValidationInfo.Type.Prefab)
                {
                    EditorGUILayout.LabelField(info.prefabPath);
                }
            }
            EditorGUILayout.EndHorizontal();
        }


        void SearchForObjectsInScene()
        {
            GameObject[] allGoInScene = FindObjectsOfType<GameObject>(true);
            objectsToValidate = CheckGameObjectsForValidateAttribute(allGoInScene);
        }

        void SearchForPrefabsInFolder()
        {
            string[] prefabGUIDs = AssetDatabase.FindAssets("t: prefab");
            objectsToValidate = CheckPrefabsForValidateAttribute(prefabGUIDs);
        }


        List<ValidationInfo> CheckPrefabsForValidateAttribute(string[] prefabsToCheckGUIDs)
        {
            List<ValidationInfo> prefabsWithValidateAttribute = new List<ValidationInfo>();

            for (int i = 0; i < prefabsToCheckGUIDs.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(prefabsToCheckGUIDs[i]);

                if (AssetDatabase.LoadMainAssetAtPath(path) is GameObject prefab)
                {
                    if (EditorUtility.DisplayCancelableProgressBar($"Looking for Objects to validate ...", prefab.name, (i / (float)prefabsToCheckGUIDs.Length)))
                        break;

                    ValidateAttribute attribute;
                    if (DoesGameObjectHasValidateAttribute(prefab, out attribute))
                    {
                        prefabsWithValidateAttribute.Add(new ValidationInfo(attribute, path));
                    }
                }
            }

            EditorUtility.ClearProgressBar();

            return prefabsWithValidateAttribute;
        }

        List<ValidationInfo> CheckGameObjectsForValidateAttribute(GameObject[] gosToCheck)
        {
            List<ValidationInfo> gosWithValidateAttribute = new List<ValidationInfo>();

            for (int i = 0; i < gosToCheck.Length; i++)
            {
                if (EditorUtility.DisplayCancelableProgressBar($"Looking for Objects to validate ...", gosToCheck[i].name, (i / (float)gosToCheck.Length)))
                    break;

                ValidateAttribute attribute;
                if (DoesGameObjectHasValidateAttribute(gosToCheck[i], out attribute))
                {
                    gosWithValidateAttribute.Add(new ValidationInfo(attribute, gosToCheck[i]));
                }
            }

            EditorUtility.ClearProgressBar();

            return gosWithValidateAttribute;
        }

        bool DoesGameObjectHasValidateAttribute(GameObject gameObject, out ValidateAttribute attribute)
        {
            Component[] components = gameObject.GetComponents<Component>();

            for (int j = 0; j < components.Length; j++)
            {
                if (components[j] == null)
                    continue;

                FieldInfo[] fieldInfos = components[j].GetType().GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

                foreach (FieldInfo info in fieldInfos)
                {
                    attribute = info.GetCustomAttribute<ValidateAttribute>();
                    if (attribute != null)
                    {
                        return true;
                    }
                }
            }

            attribute = null;
            return false;
        }

    }
}
