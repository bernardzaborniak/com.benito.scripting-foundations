using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace Benito.ScriptingFoundations.Validator.Editor
{
    public static class ValidationHelper
    {
        //TODO validation methods dont work in subclasses :(

        public static bool IsPropertyValid(SerializedProperty property, ValidateAttribute attribute, out string errorMessage, out MessageType errorMessageType)
        {
            if(attribute.type == ValidateAttribute.ValidateAttributeType.ObjectNotNull)
            {
                if (property.propertyType != SerializedPropertyType.ObjectReference)
                {
                    errorMessage = "Validate Not Null only works for type of object refrence";
                    errorMessageType = MessageType.Error;
                    return false;
                }
                else if (property.objectReferenceValue == null)
                {
                    errorMessage = attribute.errorMessage;
                    errorMessageType = ConvertToEditorMessageType(attribute.messageType);
                    return false;
                }
            }
            else if (attribute.type == ValidateAttribute.ValidateAttributeType.Custom)
            {
                const BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
                MethodInfo validateMethod = property.serializedObject.targetObject.GetType().GetMethod(attribute.validateMethodName, flags);

                if(validateMethod == null)
                {
                    errorMessage = $"The Name of the validateMethod \"{attribute.validateMethodName}\" is wrong, no such method exists - Attribute of \"{property.name}\" in \"{property.serializedObject.targetObject.GetType()}\" ";
                    errorMessageType = MessageType.Error;
                    return false;
                }
                else if(validateMethod.ReturnType != typeof(bool))
                {
                    errorMessage = $"The validateMethod \"{validateMethod.Name}\" needs to return a bool - Attribute of \"{property.name}\" in \"{property.serializedObject.targetObject.GetType()}\" ";
                    errorMessageType = MessageType.Error;
                    return false;
                }
                else if (validateMethod.GetParameters().Length >0)
                {
                    errorMessage = $"The validateMethod \"{validateMethod.Name}\" should not have any parameters - Attribute of \"{property.name}\" in \"{property.serializedObject.targetObject.GetType()}\" ";
                    errorMessageType = MessageType.Error;
                    return false;
                }
                else
                {
                    errorMessage = attribute.errorMessage;
                    errorMessageType = ConvertToEditorMessageType(attribute.messageType);
                    return (bool)validateMethod.Invoke(property.serializedObject.targetObject, null);
                }  
            }

            errorMessage = "";
            errorMessageType = MessageType.None;
            return true;
        }

        public static MessageType ConvertToEditorMessageType(ValidateAttribute.MessageType attributeType)
        {
            if(attributeType == ValidateAttribute.MessageType.Error)
            {
                return MessageType.Error;
            }
            else if(attributeType == ValidateAttribute.MessageType.Warning)
            {
                return MessageType.Warning;
            }

            return MessageType.None;
        }
    }
}


