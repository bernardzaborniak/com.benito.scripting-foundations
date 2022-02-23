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

        List<ValidationInfoGameObjectCollection> allObjects = new List<ValidationInfoGameObjectCollection>();
        bool[] allGoObjectFoldout;
        bool[][] allComponentsFoldout;

        List<ValidationInfoGameObjectCollection> objectsWithInvalidValues = new List<ValidationInfoGameObjectCollection>();
        bool[] invalidGoObjectFoldout;
        bool[][] invalidComponentsFoldout;

        Vector2 scroll;

        #region Helper Classes

        class ValidationInfo
        {
            public enum Type
            {
                Prefab,
                GameObject
            }
            public readonly Type type;

            public readonly FieldInfo fieldInfo;
            public readonly ValidateAttribute attribute;
            public readonly string prefabPath;
            public readonly GameObject gameObject;

            public readonly bool isValid;

            public ValidationInfo(FieldInfo fieldInfo, ValidateAttribute attribute, bool isValid, GameObject gameObject, string prefabPath = "")
            {
                type = Type.GameObject;
                this.fieldInfo = fieldInfo;
                this.attribute = attribute;
                this.isValid = isValid;
                this.gameObject = gameObject;
                this.prefabPath = prefabPath;
            }
        }

        class ValidationInfoComponentCollection
        {
            public string name;
            public List<ValidationInfo> validationInfos;

            public ValidationInfoComponentCollection(string name, List<ValidationInfo> validationInfos)
            {
                this.name = name;
                this.validationInfos = validationInfos;
            }
        }

        class ValidationInfoGameObjectCollection
        {
            public string name;
            public List<ValidationInfoComponentCollection> componentCollections;

            public ValidationInfoGameObjectCollection(string name, List<ValidationInfoComponentCollection> componentCollections)
            {
                this.name = name;
                this.componentCollections = componentCollections;
            }
        }

        class ObjectToValidate
        {
            public readonly string prefabPath;
            public readonly GameObject gameObject;

            public ObjectToValidate(GameObject gameObject, string prefabPath = "")
            {
                this.gameObject = gameObject;
                this.prefabPath = prefabPath;
            }
        }

        #endregion


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
            {
                if (GUILayout.Button("Search in Scene", GUILayout.Width(position.width * 0.49f), GUILayout.Height(buttonHeight)))
                {
                    SearchForObjectsInScene();
                }
                if (GUILayout.Button("Search in Prefabs in Assets and Packages", GUILayout.Width(position.width * 0.49f), GUILayout.Height(buttonHeight)))
                {
                    SearchForPrefabsInFolder();
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Expand All", GUILayout.Width(position.width * 0.2f), GUILayout.Height(buttonHeight * 0.7f)))
                {
                    ExpandAll();
                }
                if (GUILayout.Button("Collapse All", GUILayout.Width(position.width * 0.2f), GUILayout.Height(buttonHeight * 0.7f)))
                {
                    CollapseAll();
                }
            }
            EditorGUILayout.EndHorizontal();

            DisplaySearchResult();
        }

        #region Drawing Results

        void DisplaySearchResult()
        {
            scroll = GUILayout.BeginScrollView(scroll);
            {
                var labelStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 16, fontStyle = FontStyle.Bold };

                EditorGUILayout.LabelField("Invalid Values", labelStyle);
                DrawValidationInfoCollection(objectsWithInvalidValues, invalidGoObjectFoldout, invalidComponentsFoldout);

                EditorGUILayout.Space();

                EditorGUILayout.LabelField("All Objects", labelStyle);
                DrawValidationInfoCollection(allObjects, allGoObjectFoldout, allComponentsFoldout);
            }
            GUILayout.EndScrollView();
        }

        void DrawValidationInfoCollection(List<ValidationInfoGameObjectCollection> infoCollection, bool[] foldoutBoolsObject, bool[][] foldoutBoolsCombonents)
        {
            var foldout = EditorStyles.foldoutHeader;

            for (int i = 0; i < infoCollection.Count; i++)
            {
                foldout.fontSize = 14; 
                foldoutBoolsObject[i] = EditorGUILayout.Foldout(foldoutBoolsObject[i], infoCollection[i].name, foldout);
                foldout.fontSize = 12;

                if (foldoutBoolsObject[i])
                {
                    EditorGUI.indentLevel += 1;
                    List<ValidationInfoComponentCollection> componentCollections = infoCollection[i].componentCollections;

                    for (int j = 0; j < componentCollections.Count; j++)
                    {
                        foldoutBoolsCombonents[i][j] = EditorGUILayout.Foldout(foldoutBoolsCombonents[i][j], componentCollections[j].name, foldout);

                        if (foldoutBoolsCombonents[i][j])
                        {
                            foreach (ValidationInfo info in componentCollections[j].validationInfos)
                            {
                                EditorGUI.indentLevel += 1;

                                Rect rect = EditorGUILayout.GetControlRect(false, 2);
                                rect.height = 2;
                                EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));

                                DrawValidationInfo(info);
                                EditorGUI.indentLevel -= 1;
                            }
                        }
                    }
                    EditorGUI.indentLevel -= 1;
                }
            }
        }

        void DrawValidationInfo(ValidationInfo validationInfo)
        {
            EditorGUILayout.BeginHorizontal();//GUILayout.Height(EditorGUIUtility.singleLineHeight * 2f));
            {
                if (validationInfo.prefabPath != "")
                {
                    EditorGUILayout.LabelField(validationInfo.fieldInfo.FieldType.ToString(), GUILayout.Width(position.width*0.15f));              
                    EditorGUILayout.LabelField(validationInfo.fieldInfo.Name, GUILayout.Width(position.width * 0.2f));
                    EditorGUILayout.LabelField(validationInfo.prefabPath, GUILayout.Width(position.width * 0.3f));
                }
                else
                {
                    EditorGUILayout.LabelField(validationInfo.fieldInfo.FieldType.ToString(), GUILayout.Width(position.width * 0.2f));
                    EditorGUILayout.LabelField(validationInfo.fieldInfo.Name, GUILayout.Width(position.width * 0.4f));
                }

                //EditorGUILayout.Space(position.width * 0.1f);

                if (validationInfo.isValid)
                {
                    EditorGUILayout.LabelField("Is Correct", GUILayout.Width(position.width * 0.2f));
                }
                else
                {
                    EditorGUILayout.HelpBox(validationInfo.attribute.errorMessage, MessageType.Error);
                }

                if (GUILayout.Button("Select", GUILayout.Width(position.width * 0.05f), GUILayout.Height(EditorGUIUtility.singleLineHeight * 2f)))
                {
                    Selection.activeObject = validationInfo.gameObject;
                    
                }
                EditorGUILayout.LabelField("", GUILayout.Width(position.width * 0.001f)); // Small space so the button doesnt go too far to the right


            }
            EditorGUILayout.EndHorizontal();
        }

        void ExpandAll()
        {
            for (int i = 0; i < allGoObjectFoldout.Length; i++)
            {
                allGoObjectFoldout[i] = true;
            }

            for (int i = 0; i < allComponentsFoldout.Length; i++)
            {
                for (int j = 0; j < allComponentsFoldout[i].Length; j++)
                {
                    allComponentsFoldout[i][j] = true;
                }
            }

            for (int i = 0; i < invalidGoObjectFoldout.Length; i++)
            {
                invalidGoObjectFoldout[i] = true;
            }

            for (int i = 0; i < invalidComponentsFoldout.Length; i++)
            {
                for (int j = 0; j < invalidComponentsFoldout[i].Length; j++)
                {
                    invalidComponentsFoldout[i][j] = true;
                }
            }
        }

        void CollapseAll()
        {
            for (int i = 0; i < allGoObjectFoldout.Length; i++)
            {
                allGoObjectFoldout[i] = false;
            }

            for (int i = 0; i < allComponentsFoldout.Length; i++)
            {
                for (int j = 0; j < allComponentsFoldout[i].Length; j++)
                {
                    allComponentsFoldout[i][j] = false;
                }
            }

            for (int i = 0; i < invalidGoObjectFoldout.Length; i++)
            {
                invalidGoObjectFoldout[i] = false;
            }

            for (int i = 0; i < invalidComponentsFoldout.Length; i++)
            {
                for (int j = 0; j < invalidComponentsFoldout[i].Length; j++)
                {
                    invalidComponentsFoldout[i][j] = false;
                }
            }
        }

        #endregion

        #region Scanning for Results

        void SearchForObjectsInScene()
        {
            GameObject[] allGoInScene = FindObjectsOfType<GameObject>(true);

            List<ObjectToValidate> objectsInScene = new List<ObjectToValidate>();
            for (int i = 0; i < allGoInScene.Length; i++)
            {
                objectsInScene.Add(new ObjectToValidate(allGoInScene[i]));
            }

            CheckObjectsForInvalidValues(objectsInScene, out allObjects, out objectsWithInvalidValues);
        }

        void SearchForPrefabsInFolder()
        {
            string[] prefabGUIDs = AssetDatabase.FindAssets("t: prefab");
            Debug.Log("prefabGUIDs: " + prefabGUIDs.Length);

            List<ObjectToValidate> objectsInFolder = new List<ObjectToValidate>();
            foreach (string guid in prefabGUIDs)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Debug.Log("check at path: " + path);

                if (AssetDatabase.LoadMainAssetAtPath(path) is GameObject prefab)
                {
                    Debug.Log("add asset object: " + path);
                    objectsInFolder.Add(new ObjectToValidate(prefab, path));
                }
            }


            CheckObjectsForInvalidValues(objectsInFolder, out allObjects, out objectsWithInvalidValues);
        }

        void CheckObjectsForInvalidValues(List<ObjectToValidate> gosToCheck, out List<ValidationInfoGameObjectCollection> allObjects, out List<ValidationInfoGameObjectCollection> invalidObjects)
        {
            allObjects = new List<ValidationInfoGameObjectCollection>();
            invalidObjects = new List<ValidationInfoGameObjectCollection>();

            for (int i = 0; i < gosToCheck.Count; i++)
            {
                if (EditorUtility.DisplayCancelableProgressBar($"Looking for Objects to validate ...", gosToCheck[i].gameObject.name, (i / (float)gosToCheck.Count)))
                    break;


                List<ValidationInfoComponentCollection> allComponentCollections = new List<ValidationInfoComponentCollection>();
                List<ValidationInfoComponentCollection> invalidComponentCollections = new List<ValidationInfoComponentCollection>();

                Component[] components = gosToCheck[i].gameObject.GetComponents<Component>();

                foreach (Component component in components)
                {
                    if (component == null)
                        continue;

                    List<ValidationInfo> allInfosOnComponent = new List<ValidationInfo>();
                    List<ValidationInfo> invalidInfosOnComponent = new List<ValidationInfo>();

                    FieldInfo[] fieldInfos = component.GetType().GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

                    foreach (FieldInfo fieldInfo in fieldInfos)
                    {
                        ValidateAttribute attribute = fieldInfo.GetCustomAttribute<ValidateAttribute>();
                        if (attribute != null)
                        {
                            SerializedObject serializedObject = new SerializedObject(component);

                            string errorMessage;
                            MessageType messageType;

                            ValidationInfo validationInfo = new ValidationInfo(
                                fieldInfo,
                                attribute,
                                ValidationHelper.IsPropertyValid(serializedObject.FindProperty(fieldInfo.Name), attribute, out errorMessage, out messageType),
                                gosToCheck[i].gameObject,
                                gosToCheck[i].prefabPath
                                );

                            allInfosOnComponent.Add(validationInfo);

                            if (!validationInfo.isValid)
                                invalidInfosOnComponent.Add(validationInfo);
                        }
                    }

                    if (allInfosOnComponent.Count > 0)
                        allComponentCollections.Add(new ValidationInfoComponentCollection(component.GetType().ToString(), allInfosOnComponent));

                    if (invalidInfosOnComponent.Count > 0)
                        invalidComponentCollections.Add(new ValidationInfoComponentCollection(component.GetType().ToString(), invalidInfosOnComponent));
                }

                if (allComponentCollections.Count > 0)
                    allObjects.Add(new ValidationInfoGameObjectCollection(gosToCheck[i].gameObject.name, allComponentCollections));

                if (invalidComponentCollections.Count > 0)
                    invalidObjects.Add(new ValidationInfoGameObjectCollection(gosToCheck[i].gameObject.name, invalidComponentCollections));
            }

            EditorUtility.ClearProgressBar();

            // Prepare Foldout Bools
            allGoObjectFoldout = new bool[allObjects.Count];
            allComponentsFoldout = new bool[allObjects.Count][];
            for (int i = 0; i < allObjects.Count; i++)
            {
                allComponentsFoldout[i] = new bool[allObjects[i].componentCollections.Count];
            }

            invalidGoObjectFoldout = new bool[invalidObjects.Count];
            invalidComponentsFoldout = new bool[invalidObjects.Count][];
            for (int i = 0; i < invalidObjects.Count; i++)
            {
                invalidComponentsFoldout[i] = new bool[invalidObjects[i].componentCollections.Count];
            }


        }

        #endregion

    }
}
