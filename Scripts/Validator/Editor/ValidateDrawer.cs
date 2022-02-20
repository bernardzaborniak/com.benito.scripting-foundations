using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Benito.ScriptingFoundations.Validator.Editor
{
    [CustomPropertyDrawer(typeof(ValidateAttribute))]

    public class ValidateDrawer : PropertyDrawer
	{
		bool isInvalid;
		float defaultHeight;
		float helpBoxHeight;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			isInvalid = false;
			defaultHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
			helpBoxHeight = EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing;

			EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, defaultHeight), property, new GUIContent(property.displayName));

			ValidateAttribute attribute = (ValidateAttribute)this.attribute;

			string errorMessage = "";
			MessageType errorMessageType;

			if(!ValidationHelper.IsPropertyValid(property,attribute, out errorMessage, out errorMessageType))
			{
				isInvalid = true;
				EditorGUI.HelpBox(new Rect(position.x, position.y + defaultHeight, position.width, helpBoxHeight), errorMessage, errorMessageType);
			}
		}


		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
            if (isInvalid)
            {
				return base.GetPropertyHeight(property, label) + helpBoxHeight;
			}
            else
            {
				return base.GetPropertyHeight(property, label);
			}
		}


	}
}
