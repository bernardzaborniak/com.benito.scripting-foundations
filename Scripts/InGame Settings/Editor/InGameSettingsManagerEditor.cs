using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

namespace Benito.ScriptingFoundations.InGameSettings.Editor 
{
    // todo allow editiing setting here?

    [CustomEditor(typeof(InGameSettingsManager))]
    public class InGameSettingsManagerEditor : UnityEditor.Editor
    {
        List<InGameSettings> loadedSettings;

        void OnEnable()
        {
            loadedSettings = (target as InGameSettingsManager).LoadedSettings;
        }

        public override void OnInspectorGUI()
        {
            if (loadedSettings == null)
                return;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Loaded Settings", EditorStyles.boldLabel);

            foreach (InGameSettings settings in loadedSettings)
            {
                EditorGUILayout.LabelField(settings.GetType().Name); 

                //DrawInGameSettingsEditor(settings); //TODO improve upon this one

            }
        }

        void DrawInGameSettingsEditor(InGameSettings settings)
        {
            // SerializedObject serializedSettings = new SerializedObject(settings);

            // TODO do foldout here ? 

            Rect rect = EditorGUILayout.GetControlRect();

            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
            // check  that it does not have the hide in inspector attribute

            FieldInfo[] fields = settings.GetType().GetFields(flags);

            Debug.Log("settings.GetType(): " + settings.GetType());
            Debug.Log("fields length: " + fields.Length);

            foreach (FieldInfo fieldInfo in fields)
            {
                Debug.Log("FieldInfo: " + fieldInfo);
                if(fieldInfo.FieldType == typeof(Enum))
                {
                    fieldInfo.SetValue(settings,EditorGUI.EnumPopup(new Rect(rect.x,rect.y,rect.width, EditorGUIUtility.singleLineHeight),(Enum)fieldInfo.GetValue(settings)));
                }
                else if (fieldInfo.FieldType == typeof(string))
                {
                    fieldInfo.SetValue(settings, EditorGUI.TextField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), (string)fieldInfo.GetValue(settings)));
                }
                else if (fieldInfo.FieldType == typeof(int))
                {
                    fieldInfo.SetValue(settings, EditorGUI.IntField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), (int)fieldInfo.GetValue(settings)));
                }
                else if (fieldInfo.FieldType == typeof(float))
                {
                    fieldInfo.SetValue(settings, EditorGUI.FloatField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), (float)fieldInfo.GetValue(settings)));
                }
                else if (fieldInfo.FieldType == typeof(bool))
                {
                    fieldInfo.SetValue(settings, EditorGUI.Toggle(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), (bool)fieldInfo.GetValue(settings)));

                }

                rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            }
        }
    }
}


