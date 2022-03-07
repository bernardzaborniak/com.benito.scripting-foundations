using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Benito.ScriptingFoundations.InGameSettings
{
    [System.Serializable]
    public class InGameSettings
    {
        [SerializeField]
        public string settingsTypeName;
        [SerializeField]
        public string settingsAssemblyName;

        /// <summary>
        /// All InGameSettings should be in one folder, so the path is just "name".json
        /// </summary>
        public string RelativeSettingsPath { get => this.GetType() + ".json"; }
    }
}
