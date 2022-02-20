using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

namespace Benito.ScriptingFoundations.Validator
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |
    AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
    public class ValidateAttribute : PropertyAttribute
    {
        public enum ValidateAttributeType
        {
            Custom,
            ObjectNotNull
        }

        public enum MessageType
        {
            Error,
            Warning
        }

        public readonly ValidateAttributeType type;
        public readonly MessageType messageType;
        public readonly string validateMethodName;
      
        public readonly string errorMessage;

        public ValidateAttribute(string errorMessage, MessageType messageType = MessageType.Error)
        {
            this.type = ValidateAttributeType.ObjectNotNull;
            this.errorMessage = errorMessage;
            this.messageType = messageType;           
        }

        public ValidateAttribute(string errorMessage, string validateMethodName, MessageType messageType = MessageType.Error)
        {
            this.type = ValidateAttributeType.Custom; 
            this.errorMessage = errorMessage;
            this.validateMethodName = validateMethodName;
            this.messageType = messageType;
        }        
    }
}
