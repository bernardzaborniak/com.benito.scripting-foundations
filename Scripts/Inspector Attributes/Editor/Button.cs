using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace Benito.ScriptingFoundations.InspectorAttributes
{
    public class Button
    {
        public readonly string Name;
        public readonly MethodInfo Method;

        public Button(MethodInfo methodInfo, string displayName)
        {
            Method = methodInfo;
            Name = displayName;
        }
    }
}


