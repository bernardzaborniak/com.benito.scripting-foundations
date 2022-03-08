using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Benito.ScriptingFoundations.InGameSettings
{
    [System.Serializable]
    public class InGameSettings
    {
        public string tempString;

        [HideInInspector]
        [SerializeField]
        public string settingsTypeName;
        [HideInInspector]
        [SerializeField]
        public string settingsAssemblyName;

        /// <summary>
        /// Is being used as path, All InGameSettings should be in one folder, so the path is just "name".json
        /// </summary>
        public string GetFileName { get => this.GetType().Name + ".json"; }
    }
}
