using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Benito.ScriptingFoundations.InspectorAttributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ButtonAttribute : Attribute
    {
        readonly string buttonName;
        public ButtonAttribute(string buttonName) 
        {
            this.buttonName = buttonName;
        }
    }
}
