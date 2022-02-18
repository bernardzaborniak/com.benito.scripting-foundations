using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Benito.ScriptingFoundations.InspectorAttributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |
    AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
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

        public readonly string boolFieldName2;
        public readonly bool boolDesiredValue2;

        public readonly string enumFieldName1;
        public readonly int enumDesiredValue1;

        public readonly string enumFieldName2;
        public readonly int enumDesiredValue2;


        public ShowIfAttribute(string boolFieldName1, bool boolDesiredValue1)
        {
            showIfType = ShowIfType.OneBool;
            this.boolFieldName1 = boolFieldName1;
            this.boolDesiredValue1 = boolDesiredValue1;
        }

        public ShowIfAttribute(string boolFieldName1, bool boolDesiredValue1, string boolFieldName2, bool boolDesiredValue2)
        {
            showIfType = ShowIfType.TwoBools;
            this.boolFieldName1 = boolFieldName1;
            this.boolDesiredValue1 = boolDesiredValue1;
            this.boolFieldName2 = boolFieldName2;
            this.boolDesiredValue2 = boolDesiredValue2;
        }

        public ShowIfAttribute(string enumFieldName1, int enumDesiredValue1)
        {
            showIfType = ShowIfType.OneEnum;
            this.enumFieldName1 = enumFieldName1;
            this.enumDesiredValue1 = enumDesiredValue1;
        }

        public ShowIfAttribute(string enumFieldName1, int enumDesiredValue1, string enumFieldName2, int enumDesiredValue2)
        {
            showIfType = ShowIfType.TwoEnums;
            this.enumFieldName1 = enumFieldName1;
            this.enumDesiredValue1 = enumDesiredValue1;
            this.enumFieldName2 = enumFieldName2;
            this.enumDesiredValue2 = enumDesiredValue2;
        }

        public ShowIfAttribute(string boolFieldName1, bool boolDesiredValue1, string enumFieldName1, int enumDesiredValue1)
        {
            showIfType = ShowIfType.OneBoolAndOneEnum;
            this.boolFieldName1 = boolFieldName1;
            this.boolDesiredValue1 = boolDesiredValue1;
            this.boolDesiredValue1 = boolDesiredValue1;
            this.enumFieldName1 = enumFieldName1;
            this.enumDesiredValue1 = enumDesiredValue1;
        }
    }
}


