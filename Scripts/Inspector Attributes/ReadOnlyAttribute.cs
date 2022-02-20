using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Benito.ScriptingFoundations.InspectorAttributes 
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |
    AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
    public class ReadOnlyAttribute : PropertyAttribute
    {
        
    }
}



