using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Benito.ScriptingFoundations.InspectorAttributes
{
    public class ShowIfAttribute : PropertyAttribute
    {
        public enum ShowIfType
        {
            OneBool,
            TwoBools,
            OneEnum,
            TwoEnums,
            OneBoolAndOneEnum
        }

        public ShowIfType showIfType;

        public readonly string boolFieldName1;
        public readonly bool boolDesiredValue1;
        //public readonly object comparisionValue;
        //public readonly object[] comparisionValueArray;

        public ShowIfAttribute(string boolFieldName1, bool boolDesiredValue1)
        {
            showIfType = ShowIfType.OneBool;
            this.boolFieldName1 = boolFieldName1;
            this.boolDesiredValue1 = boolDesiredValue1;
        }

        /*public ShowIfAttribute(string conditionFieldName1, object comparisionValue)
        {
            this.conditionFieldName1 = conditionFieldName1;
            this.comparisionValue = comparisionValue;
        }

        public ShowIfAttribute(string conditionFieldName1, object[] comparisionValueArray)
        {
            this.conditionFieldName1 = conditionFieldName1;
            this.comparisionValueArray = comparisionValueArray;
        }*/
    }
}


