using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Benito.ScriptingFoundations.InspectorAttributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ButtonAttribute : Attribute
    {
        public readonly string displayName;
        public ButtonAttribute(string displayName) 
        {
            this.displayName = displayName;
        }
    }
}
