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
            ShowIfAttribute attribute = (ShowIfAttribute)this.attribute;
            
            switch (attribute.showIfType)
            {
                case ShowIfAttribute.ShowIfType.OneBool:
                    {
                        SerializedProperty conditionField = property.serializedObject.FindProperty(attribute.boolFieldName1);
                        showField = conditionField.boolValue == attribute.boolDesiredValue1;
                        break;
                    }
            }

            if (showField)
                EditorGUI.PropertyField(position, property, new GUIContent(property.displayName), true);
        }

        void ShowError(Rect position)
        {
            EditorGUI.HelpBox(position, "[ShowIfDrawer] Error getting the condition Field. Check the name.", MessageType.Error);
            return;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (showField)
                return EditorGUI.GetPropertyHeight(property);
            else
                return -EditorGUIUtility.standardVerticalSpacing;
        }

    }
}

