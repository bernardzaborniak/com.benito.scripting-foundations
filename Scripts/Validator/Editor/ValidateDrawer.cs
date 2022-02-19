using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Benito.ScriptingFoundations.Validator.Editor
{
    [CustomPropertyDrawer(typeof(ValidateAttribute))]

    public class ValidateDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{

			EditorGUI.PropertyField(position, property, new GUIContent(property.displayName));
			position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
			EditorGUI.PropertyField(position, property, new GUIContent(property.displayName));
			position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUIUtility.singleLineHeight * 2;
		}
	}
}
