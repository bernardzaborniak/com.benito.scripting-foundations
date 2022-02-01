using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Benito.ScriptingFoundations.Utilities;


namespace Benito.ScriptingFoundations.Managers
{   
    public class GlobalManagersSettings : ScriptableObject
    {
        const string DefaultSettingsPathInResourcesFolder = "Settings/Global Managers Settings";

        [Space(10)]
        [Tooltip("Prefab Containing all the Global Managers, use \"Create/Scripting Foundations/Global Managers\" Prefab to create.")]
        public GameObject globalManagersPrefab;

        public static GlobalManagersSettings GetOrCreateSettings()
        {
            return SettingsUtilities.GetOrCreateSettingAsset<GlobalManagersSettings>(DefaultSettingsPathInResourcesFolder);
        }
    }  
}
