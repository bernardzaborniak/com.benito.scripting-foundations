using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace Benito.ScriptingFoundations.InspectorAttributes
{
    [CustomPropertyDrawer(typeof(ShowIfAttribute))]
    public class ShowIfDrawer : PropertyDrawer
    {
        bool showField = true;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.isArray && property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.HelpBox(position, $"[ShowIfDrawer] Does not work for arrays :/  - used in Field \"{property.displayName}\"", MessageType.Error);
                return;
            }
                

            ShowIfAttribute attribute = (ShowIfAttribute)this.attribute;
            string propertyPath = property.propertyPath;

            switch (attribute.showIfType)
            {
                case ShowIfAttribute.ShowIfType.OneBool:
                    {
                        string fixedConditionPath1 = propertyPath.Replace(property.name, attribute.boolFieldName1); // Makes sure this also works for nexted classes.
                        SerializedProperty conditionField1 = property.serializedObject.FindProperty(fixedConditionPath1);

                        if (conditionField1 == null)
                        {
                            ShowError(position, property.displayName, property.type);
                            return;
                        }

                        showField = conditionField1.boolValue == attribute.boolDesiredValue1;
                        break;
                    }
                case ShowIfAttribute.ShowIfType.TwoBools:
                    {
                        string fixedConditionPath1 = propertyPath.Replace(property.name, attribute.boolFieldName1); 
                        SerializedProperty conditionField1 = property.serializedObject.FindProperty(fixedConditionPath1);

                        string fixedConditionPath2 = propertyPath.Replace(property.name, attribute.boolFieldName2);
                        SerializedProperty conditionField2 = property.serializedObject.FindProperty(fixedConditionPath2);

                        if (conditionField1 == null || conditionField2 == null)
                        {
                            ShowError(position, property.displayName, property.type);
                            return;
                        }

                        showField = conditionField1.boolValue == attribute.boolDesiredValue1 && conditionField2.boolValue == attribute.boolDesiredValue2;
                        break;
                    }
                case ShowIfAttribute.ShowIfType.OneEnum:
                    {
                        string fixedConditionPath1 = propertyPath.Replace(property.name, attribute.enumFieldName1); 
                        SerializedProperty conditionField1 = property.serializedObject.FindProperty(fixedConditionPath1);

                        if (conditionField1 == null)
                        {
                            ShowError(position, property.displayName, property.type);
                            return;
                        }

                        showField = conditionField1.enumValueIndex == attribute.enumDesiredValue1;
                        break;
                    }
                case ShowIfAttribute.ShowIfType.TwoEnums:
                    {
                        string fixedConditionPath1 = propertyPath.Replace(property.name, attribute.enumFieldName1); 
                        SerializedProperty conditionField1 = property.serializedObject.FindProperty(fixedConditionPath1);
                        string fixedConditionPath2 = propertyPath.Replace(property.name, attribute.enumFieldName2);
                        SerializedProperty conditionField2 = property.serializedObject.FindProperty(fixedConditionPath2);

                        if (conditionField1 == null || conditionField2 == null)
                        {
                            ShowError(position, property.displayName, property.type);
                            return;
                        }
 
                        showField = conditionField1.enumValueIndex == attribute.enumDesiredValue1 && conditionField2.enumValueIndex == attribute.enumDesiredValue2;
                        break;
                    }
                case ShowIfAttribute.ShowIfType.OneBoolAndOneEnum:
                    {
                        string fixedConditionPathBool = propertyPath.Replace(property.name, attribute.boolFieldName1);
                        SerializedProperty conditionFieldBool = property.serializedObject.FindProperty(fixedConditionPathBool);

                        string fixedConditionPathEnum = propertyPath.Replace(property.name, attribute.enumFieldName1);
                        SerializedProperty conditionFieldEnum = property.serializedObject.FindProperty(fixedConditionPathEnum);

                        if (conditionFieldBool == null || conditionFieldEnum == null)
                        {
                            ShowError(position, property.displayName, property.type);
                            return;
                        }

                        showField = conditionFieldBool.boolValue == attribute.boolDesiredValue1 && conditionFieldEnum.enumValueIndex == attribute.enumDesiredValue1;
                        break;
                    }

            }

            if (showField)
                EditorGUI.PropertyField(position, property, new GUIContent(property.displayName), true);
        }

        void ShowError(Rect position, string fieldName, string propertyType)
        {
            EditorGUI.HelpBox(position, $"[ShowIfDrawer] Error with the drawing of the field \"{fieldName}\" of Type \"{propertyType}\". Check the name.", MessageType.Error);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (showField )
                return EditorGUI.GetPropertyHeight(property);
            else
                return -EditorGUIUtility.standardVerticalSpacing;
        }

    }
}

