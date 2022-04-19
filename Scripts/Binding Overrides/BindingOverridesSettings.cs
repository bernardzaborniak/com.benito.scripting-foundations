using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Benito.ScriptingFoundations.Utilities;

namespace Benito.ScriptingFoundations.BindingOverrides
{
    [System.Serializable]
    public class BindingOverridesSettings : ScriptableObject
    {
        const string DefaultSettingsPathInResourcesFolder = "Settings/Binding Overrides Settings";

        [SerializeField]
        IOUtilities.AssigneableGameDataPath overridesPath = new IOUtilities.AssigneableGameDataPath(IOUtilities.AssigneableGameDataPath.PathPrefix.PersistendData, "BindingOverrides");


        public static BindingOverridesSettings GetOrCreateSettings()
        {
            return RessourceSettingsUtilities.GetOrCreateSettingAsset<BindingOverridesSettings>(DefaultSettingsPathInResourcesFolder);
        }
        public string GetOverridesFolderPath()
        {
            string path = overridesPath.GetPath();
            IOUtilities.EnsurePathExists(path);
            return path;
        }
    }
}
