using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace Benito.ScriptingFoundations.InGameSettings
{
    [System.Serializable]
    public class InGameSettingsJsonWrapper <T>
    {
        [SerializeField]
        public string settingsTypeName;
        [SerializeField]
        public string settingsAssemblyName;

        [SerializeField]
        public T settings;

        public InGameSettingsJsonWrapper(T settings)
        {
            settingsTypeName = settings.GetType().FullName;
            settingsAssemblyName = settings.GetType().Assembly.FullName;

            //this.settingsTypeName = settingsTypeName;
            //this.settingsAssemblyName = settingsAssemblyName;
            this.settings = settings;
        }
    }
}

