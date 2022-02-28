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
        bool[] allGoObjectFoldout = new bool[0];
        bool[][] allComponentsFoldout = new bool[0][];

        List<ValidationInfoGameObjectCollection> objectsWithInvalidValues = new List<ValidationInfoGameObjectCollection>();
        bool[] invalidGoObjectFoldout = new bool[0];
        bool[][] invalidComponentsFoldout = new bool[0][];

        List<ValidationInfoCollection> allScriptableObjects = new List<ValidationInfoCollection>();
        List<ValidationInfoCollection> scriptableObjectsWithInvalidValues = new List<ValidationInfoCollection>();
        bool[] allScriptableObjectsFoldout = new bool[0];
        bool[] invalidScriptableObjectsFoldout = new bool[0];

        Vector2 scroll;

        enum SearchMode
        {
            Scene,
            Prefabs,
            ScriptableObjects
        }
        SearchMode currentSearchMode;

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
            public readonly string path;
            public readonly Object validationObject;

            public readonly bool isValid;

            public ValidationInfo(FieldInfo fieldInfo, ValidateAttribute attribute, bool isValid, Object validationObject, string path = "")
            {
                type = Type.GameObject;
                this.fieldInfo = fieldInfo;
                this.attribute = attribute;
                this.isValid = isValid;
                this.validationObject = validationObject;
                this.path = path;
            }
        }

        class ValidationInfoCollection
        {
            public string name;
            public List<ValidationInfo> validationInfos;

            public ValidationInfoCollection(string name, List<ValidationInfo> validationInfos)
            {
                this.name = name;
                this.validationInfos = validationInfos;
            }
        }

        class ValidationInfoGameObjectCollection
        {
            public string name;
            public List<ValidationInfoCollection> componentCollections;

            public ValidationInfoGameObjectCollection(string name, List<ValidationInfoCollection> componentCollections)
            {
                this.name = name;
                this.componentCollections = componentCollections;
            }
        }


        class ObjectToValidate
        {
            public readonly string path;
            public readonly Object validationObject;

            public ObjectToValidate(Object validationObject, string path = "")
            {
                this.validationObject = validationObject;
                this.path = path;
            }
        }

        class ScriptableObjectToValidate
        {
            public readonly string path;
            public readonly ScriptableObject scriptableObject;

            public ScriptableObjectToValidate(ScriptableObject scriptableObject, string prefabPath = "")
            {
                this.scriptableObject = scriptableObject;
                this.path = prefabPath;
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
                if (GUILayout.Button("Search in Scene", GUILayout.Width(position.width * 0.32f), GUILayout.Height(buttonHeight)))
                {
                    SearchForObjectsInScene();
                }
                if (GUILayout.Button("Search in Prefabs in Assets and Packages", GUILayout.Width(position.width * 0.32f), GUILayout.Height(buttonHeight)))
                {
                    SearchForPrefabsInFolder();
                }
                if (GUILayout.Button("Search in Scriptable Objects in Assets and Packages", GUILayout.Width(position.width * 0.32f), GUILayout.Height(buttonHeight)))
                {
                    SearchForScriptableObjectsInFolder();
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
                if(currentSearchMode == SearchMode.ScriptableObjects)
                {
                    DrawValidationInfoColletionForSO(scriptableObjectsWithInvalidValues, invalidScriptableObjectsFoldout);
                }
                else
                {
                    DrawValidationInfoCollectionForGO(objectsWithInvalidValues, invalidGoObjectFoldout, invalidComponentsFoldout);
                }

                EditorGUILayout.Space();

                EditorGUILayout.LabelField("All Objects", labelStyle);
                if (currentSearchMode == SearchMode.ScriptableObjects)
                {
                    DrawValidationInfoColletionForSO(allScriptableObjects, allScriptableObjectsFoldout);
                }
                else
                {
                    DrawValidationInfoCollectionForGO(allObjects, allGoObjectFoldout, allComponentsFoldout);
                }
            }
            GUILayout.EndScrollView();
        }

        void DrawValidationInfoCollectionForGO(List<ValidationInfoGameObjectCollection> infoCollection, bool[] foldoutBoolsObjects, bool[][] foldoutBoolsCombonents)
        {
            var foldout = EditorStyles.foldoutHeader;

            for (int i = 0; i < infoCollection.Count; i++)
            {
                foldout.fontSize = 14;
                foldoutBoolsObjects[i] = EditorGUILayout.Foldout(foldoutBoolsObjects[i], infoCollection[i].name, foldout);
                foldout.fontSize = 12;

                if (foldoutBoolsObjects[i])
                {
                    EditorGUI.indentLevel += 1;
                    List<ValidationInfoCollection> componentCollections = infoCollection[i].componentCollections;

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

        void DrawValidationInfoColletionForSO(List<ValidationInfoCollection> infoCollection, bool[] foldoutBoolsObjects)
        {
            for (int i = 0; i < infoCollection.Count; i++)
            {
                foldoutBoolsObjects[i] = EditorGUILayout.Foldout(foldoutBoolsObjects[i], infoCollection[i].name, EditorStyles.foldoutHeader);

                if (foldoutBoolsObjects[i])
                {
                    foreach (ValidationInfo info in infoCollection[i].validationInfos)
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
        }

        void DrawValidationInfo(ValidationInfo validationInfo)
        {
            EditorGUILayout.BeginHorizontal();//GUILayout.Height(EditorGUIUtility.singleLineHeight * 2f));
            {
                if (validationInfo.path != "")
                {
                    EditorGUILayout.LabelField(validationInfo.fieldInfo.FieldType.ToString(), GUILayout.Width(position.width * 0.15f));
                    EditorGUILayout.LabelField(validationInfo.fieldInfo.Name, GUILayout.Width(position.width * 0.2f));
                    EditorGUILayout.LabelField(validationInfo.path, GUILayout.Width(position.width * 0.3f));
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
                    Selection.activeObject = validationInfo.validationObject;

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

            for (int i = 0; i < invalidScriptableObjectsFoldout.Length; i++)
            {
                invalidScriptableObjectsFoldout[i] = true;
            }
            for (int i = 0; i < allScriptableObjectsFoldout.Length; i++)
            {
                allScriptableObjectsFoldout[i] = true;
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

            for (int i = 0; i < invalidScriptableObjectsFoldout.Length; i++)
            {
                invalidScriptableObjectsFoldout[i] = false;
            }
            for (int i = 0; i < allScriptableObjectsFoldout.Length; i++)
            {
                allScriptableObjectsFoldout[i] = false;
            }
        }

        #endregion

        #region Scanning for Results

        void SearchForObjectsInScene()
        {
            currentSearchMode = SearchMode.Scene;

            GameObject[] allGoInScene = FindObjectsOfType<GameObject>(true);

            List<ObjectToValidate> objectsInScene = new List<ObjectToValidate>();
            for (int i = 0; i < allGoInScene.Length; i++)
            {
                objectsInScene.Add(new ObjectToValidate(allGoInScene[i]));
            }

            CheckGameObjectsForInvalidValues(objectsInScene, out allObjects, out objectsWithInvalidValues);
        }

        void SearchForPrefabsInFolder()
        {
            currentSearchMode = SearchMode.Prefabs;

            string[] prefabAndSoGUIDs = AssetDatabase.FindAssets("t: prefab");

            List<ObjectToValidate> objectsInFolder = new List<ObjectToValidate>();
            for (int i = 0; i < prefabAndSoGUIDs.Length; i++)
            {
                if (EditorUtility.DisplayCancelableProgressBar($"Looking for GameObjects to validate in Folder...", prefabAndSoGUIDs[i], (i / (float)prefabAndSoGUIDs.Length)))
                    break;

                string path = AssetDatabase.GUIDToAssetPath(prefabAndSoGUIDs[i]);

                if (AssetDatabase.LoadMainAssetAtPath(path) is GameObject prefab)
                {
                    objectsInFolder.Add(new ObjectToValidate(prefab, path));
                }
            }
            EditorUtility.ClearProgressBar();

            CheckGameObjectsForInvalidValues(objectsInFolder, out allObjects, out objectsWithInvalidValues);
        }

        void SearchForScriptableObjectsInFolder()
        {
            currentSearchMode = SearchMode.ScriptableObjects;

            string[] prefabAndSoGUIDs = AssetDatabase.FindAssets("t: Object");

            List<ObjectToValidate> scriptableObjectsInFolder = new List<ObjectToValidate>();


            for (int i = 0; i < prefabAndSoGUIDs.Length; i++)
            {
                if (EditorUtility.DisplayCancelableProgressBar($"Looking for Scriptable Objects to validate in Folder...", prefabAndSoGUIDs[i], (i / (float)prefabAndSoGUIDs.Length)))
                    break;

                string path = AssetDatabase.GUIDToAssetPath(prefabAndSoGUIDs[i]);

                if (AssetDatabase.LoadMainAssetAtPath(path) is ScriptableObject so)
                {
                    scriptableObjectsInFolder.Add(new ObjectToValidate(so, path));
                }
            }
            EditorUtility.ClearProgressBar();

            CheckScriptableObjectsForInvalidValues(scriptableObjectsInFolder, out allScriptableObjects, out scriptableObjectsWithInvalidValues);
        }

        void CheckScriptableObjectsForInvalidValues(List<ObjectToValidate> objectsToCheck, out List<ValidationInfoCollection> allScriptableObjects, out List<ValidationInfoCollection> invalidScriptableObjects)
        {
            allScriptableObjects = new List<ValidationInfoCollection>();
            invalidScriptableObjects = new List<ValidationInfoCollection>();

            for (int i = 0; i < objectsToCheck.Count; i++)
            {
                if (EditorUtility.DisplayCancelableProgressBar($"Validating Scriptable Objects ...", objectsToCheck[i].validationObject.name, (i / (float)objectsToCheck.Count)))
                    break;

                List<ValidationInfo> allValidationInfosForObject = new List<ValidationInfo>();
                List<ValidationInfo> invalidValidationInfosForObject = new List<ValidationInfo>();


                FieldInfo[] fieldInfos = objectsToCheck[i].validationObject.GetType().GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

                foreach (FieldInfo fieldInfo in fieldInfos)
                {
                    ValidateAttribute attribute = fieldInfo.GetCustomAttribute<ValidateAttribute>();
                    if (attribute != null)
                    {
                        SerializedObject serializedObject = new SerializedObject(objectsToCheck[i].validationObject);

                        string errorMessage;
                        MessageType messageType;

                        ValidationInfo validationInfo = new ValidationInfo(
                            fieldInfo,
                            attribute,
                            ValidationHelper.IsPropertyValid(serializedObject.FindProperty(fieldInfo.Name), attribute, out errorMessage, out messageType),
                            objectsToCheck[i].validationObject,
                            objectsToCheck[i].path
                            );

                        allValidationInfosForObject.Add(validationInfo);

                        if (!validationInfo.isValid)
                            invalidValidationInfosForObject.Add(validationInfo);
                    }
                }

                if (allValidationInfosForObject.Count > 0)
                    allScriptableObjects.Add(new ValidationInfoCollection(objectsToCheck[i].validationObject.GetType().Name, allValidationInfosForObject));

                if (invalidValidationInfosForObject.Count > 0)
                    invalidScriptableObjects.Add(new ValidationInfoCollection(objectsToCheck[i].validationObject.GetType().Name, invalidValidationInfosForObject));

            }

            EditorUtility.ClearProgressBar();

            // Prepare Foldout Bools
            allScriptableObjectsFoldout = new bool[allScriptableObjects.Count];
            invalidScriptableObjectsFoldout = new bool[invalidScriptableObjects.Count];
        }

        void CheckGameObjectsForInvalidValues(List<ObjectToValidate> gosToCheck, out List<ValidationInfoGameObjectCollection> allObjects, out List<ValidationInfoGameObjectCollection> invalidObjects)
        {
            allObjects = new List<ValidationInfoGameObjectCollection>();
            invalidObjects = new List<ValidationInfoGameObjectCollection>();

            for (int i = 0; i < gosToCheck.Count; i++)
            {
                if (EditorUtility.DisplayCancelableProgressBar($"Validating Objects ...", gosToCheck[i].validationObject.name, (i / (float)gosToCheck.Count)))
                    break;


                List<ValidationInfoCollection> allComponentCollections = new List<ValidationInfoCollection>();
                List<ValidationInfoCollection> invalidComponentCollections = new List<ValidationInfoCollection>();

                Component[] components = (gosToCheck[i].validationObject as GameObject).GetComponents<Component>();

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
                                gosToCheck[i].validationObject,
                                gosToCheck[i].path
                                );

                            allInfosOnComponent.Add(validationInfo);

                            if (!validationInfo.isValid)
                                invalidInfosOnComponent.Add(validationInfo);
                        }
                    }

                    if (allInfosOnComponent.Count > 0)
                        allComponentCollections.Add(new ValidationInfoCollection(component.GetType().ToString(), allInfosOnComponent));

                    if (invalidInfosOnComponent.Count > 0)
                        invalidComponentCollections.Add(new ValidationInfoCollection(component.GetType().ToString(), invalidInfosOnComponent));
                }

                if (allComponentCollections.Count > 0)
                    allObjects.Add(new ValidationInfoGameObjectCollection(gosToCheck[i].validationObject.name, allComponentCollections));

                if (invalidComponentCollections.Count > 0)
                    invalidObjects.Add(new ValidationInfoGameObjectCollection(gosToCheck[i].validationObject.name, invalidComponentCollections));
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
