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

        public class ValidationInfo
        {
            public enum Type
            {
                Prefab,
                GameObject
            }
            public readonly Type type;

            public readonly string fieldName;
            public readonly ValidateAttribute attribute;
            public readonly string prefabPath;
            public readonly GameObject gameObjectInScene;

            public readonly bool isValid;

            public ValidationInfo(string fieldName, ValidateAttribute attribute, bool isValid, GameObject gameObjectInScene)
            {
                type = Type.GameObject;
                this.fieldName = fieldName;
                this.attribute = attribute;
                this.isValid = isValid;
                this.gameObjectInScene = gameObjectInScene;
            }

            public ValidationInfo(string fieldName, ValidateAttribute attribute, bool isValid, string prefabPath)
            {
                type = Type.Prefab;
                this.fieldName = fieldName;
                this.attribute = attribute;
                this.isValid = isValid;
                this.prefabPath = prefabPath;
            }
        }

        public class ValidationInfoComponentCollection
        {
            public string name;
            public List<ValidationInfo> validationInfos;

            public ValidationInfoComponentCollection(string name, List<ValidationInfo> validationInfos)
            {
                this.name = name;
                this.validationInfos = validationInfos;
            }
        }

        public class ValidationInfoGameObjectCollection
        {
            public string name;
            public List<ValidationInfoComponentCollection> componentCollections;

            public ValidationInfoGameObjectCollection(string name, List<ValidationInfoComponentCollection> componentCollections)
            {
                this.name = name;
                this.componentCollections = componentCollections;
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
            if (GUILayout.Button("Search in Prefabs in Assets and Packages", GUILayout.Width(position.width * 0.49f), GUILayout.Height(buttonHeight)))
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
                EditorGUILayout.LabelField("Invalid Values");
                DrawValidationInfoCollection(objectsWithInvalidValues, invalidGoObjectFoldout, invalidComponentsFoldout);

                EditorGUILayout.Space();

                EditorGUILayout.LabelField("All Objects");
                DrawValidationInfoCollection(allObjects, allGoObjectFoldout, allComponentsFoldout);
            }
            GUILayout.EndScrollView();
        }

        void DrawValidationInfoCollection(List<ValidationInfoGameObjectCollection> infoCollection, bool[] foldoutBoolsObject, bool[][] foldoutBoolsCombonents)
        {
            for (int i = 0; i < infoCollection.Count; i++)
            {
                foldoutBoolsObject[i] = EditorGUILayout.Foldout(foldoutBoolsObject[i], infoCollection[i].name);

                if (foldoutBoolsObject[i])
                {
                    EditorGUI.indentLevel += 1;
                    //EditorGUILayout.LabelField();
                    List<ValidationInfoComponentCollection> componentCollections = infoCollection[i].componentCollections;

                    for (int j = 0; j < componentCollections.Count; j++)
                    {
                        foldoutBoolsCombonents[i][j] = EditorGUILayout.Foldout(foldoutBoolsCombonents[i][j], componentCollections[j].name);
                        
                        if(foldoutBoolsCombonents[i][j])
                        {
                            foreach (ValidationInfo info in componentCollections[j].validationInfos)
                            {
                                DrawValidationInfo(info);
                            }
                        }
                    }
                    EditorGUI.indentLevel -= 1;
                }
                // EditorGUILayout.EndFoldoutHeaderGroup();
            }
        }

        void DrawValidationInfo(ValidationInfo validationInfo)
        {
            EditorGUILayout.BeginHorizontal();
            {

                EditorGUILayout.LabelField(validationInfo.fieldName);
                if (validationInfo.isValid)
                {
                    EditorGUILayout.LabelField("Is Correct");
                }
                else
                {
                    EditorGUILayout.HelpBox(validationInfo.attribute.errorMessage, MessageType.Error);
                }

            }
            EditorGUILayout.EndHorizontal();
        }


        void SearchForObjectsInScene()
        {
            GameObject[] allGoInScene = FindObjectsOfType<GameObject>(true);
            CheckGameObjectsForInvalidValues(allGoInScene, out allObjects, out objectsWithInvalidValues);
            //objectsWithInvalidValues = CheckObjectsForInvalidValues(objectsToValidate);
        }

        void SearchForPrefabsInFolder()
        {
            string[] prefabGUIDs = AssetDatabase.FindAssets("t: prefab");
            CheckPrefabsForInvalidValues(prefabGUIDs, out allObjects, out objectsWithInvalidValues);
            //objectsWithInvalidValues = CheckObjectsForInvalidValues(objectsToValidate);
        }

        /*List<ValidationInfo> CheckObjectsForInvalidValues(List<ValidationInfo> objectsToCheck)
        {
            List<ValidationInfo> objectsWithInvalidValues = new List<ValidationInfo>();

            for (int i = 0; i < objectsToCheck.Count; i++)
            {
                if (EditorUtility.DisplayCancelableProgressBar($"Scanning for Invalid Values ...", "", (i / (float)objectsToCheck.Count)))
                    break;

                objectsToCheck[i].isValid = 
            }
        }*/


        void CheckPrefabsForInvalidValues(string[] prefabsToCheckGUIDs, out List<ValidationInfoGameObjectCollection> allObjects, out List<ValidationInfoGameObjectCollection> invalidObjects)
        {
            allObjects = new List<ValidationInfoGameObjectCollection>();
            invalidObjects = new List<ValidationInfoGameObjectCollection>();

            /*for (int i = 0; i < prefabsToCheckGUIDs.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(prefabsToCheckGUIDs[i]);

                if (AssetDatabase.LoadMainAssetAtPath(path) is GameObject prefab)
                {
                    if (EditorUtility.DisplayCancelableProgressBar($"Looking for Objects to validate ...", prefab.name, (i / (float)prefabsToCheckGUIDs.Length)))
                        break;

                    ValidateAttribute attribute;
                    if (DoesGameObjectHasValidateAttribute(prefab, out attribute))
                    {
                        if(ValidationHelper.IsPropertyValid(attribute))

                        prefabsWithInvalidValues.Add(new ValidationInfo(attribute, path));
                    }
                }
            }

            EditorUtility.ClearProgressBar();

            return prefabsWithInvalidValues;*/
        }

        void CheckGameObjectsForInvalidValues(GameObject[] gosToCheck, out List<ValidationInfoGameObjectCollection> allObjects, out List<ValidationInfoGameObjectCollection> invalidObjects)
        {
            allObjects = new List<ValidationInfoGameObjectCollection>();
            invalidObjects = new List<ValidationInfoGameObjectCollection>();

            for (int i = 0; i < gosToCheck.Length; i++)
            {
                if (EditorUtility.DisplayCancelableProgressBar($"Looking for Objects to validate ...", gosToCheck[i].name, (i / (float)gosToCheck.Length)))
                    break;


                List<ValidationInfoComponentCollection> allComponentCollections = new List<ValidationInfoComponentCollection>();
                List<ValidationInfoComponentCollection> invalidComponentCollections = new List<ValidationInfoComponentCollection>();

                Component[] components = gosToCheck[i].GetComponents<Component>();

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
                                fieldInfo.Name,
                                attribute,
                                ValidationHelper.IsPropertyValid(serializedObject.FindProperty(fieldInfo.Name), attribute, out errorMessage, out messageType),
                                gosToCheck[i]);

                            allInfosOnComponent.Add(validationInfo);

                            if (!validationInfo.isValid)
                                invalidInfosOnComponent.Add(validationInfo);
                        }
                    }

                    if(allInfosOnComponent.Count > 0)
                        allComponentCollections.Add(new ValidationInfoComponentCollection(component.GetType().ToString(), allInfosOnComponent));

                    if (invalidInfosOnComponent.Count > 0)
                        invalidComponentCollections.Add(new ValidationInfoComponentCollection(component.GetType().ToString(), invalidInfosOnComponent));
                }

                if(allComponentCollections.Count>0)
                    allObjects.Add(new ValidationInfoGameObjectCollection(gosToCheck[i].name, allComponentCollections));

                if (invalidComponentCollections.Count > 0)
                    invalidObjects.Add(new ValidationInfoGameObjectCollection(gosToCheck[i].name, invalidComponentCollections));
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

        bool DoesGameObjectHasValidateAttribute(GameObject gameObject, out ValidateAttribute attribute, out FieldInfo fieldInfo, out SerializedObject serializedObject)
        {
            Component[] components = gameObject.GetComponents<Component>();

            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] == null)
                    continue;

                FieldInfo[] fieldInfos = components[i].GetType().GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

                foreach (FieldInfo info in fieldInfos)
                {
                    attribute = info.GetCustomAttribute<ValidateAttribute>();
                    if (attribute != null)
                    {
                        fieldInfo = info;
                        serializedObject = new SerializedObject(components[i]);
                        return true;
                    }
                }
            }

            attribute = null;
            fieldInfo = null;
            serializedObject = null;
            return false;
        }

    }
}
