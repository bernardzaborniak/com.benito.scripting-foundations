using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Benito.ScriptingFoundations.Utilities;
using System.IO;

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

        public string LoadBindingOverridesJson(string overridesName)
        {
            string fileContent = string.Empty;

            string path = Path.Combine(GetOverridesFolderPath(), overridesName + ".json");
            if (File.Exists(path))
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    fileContent = reader.ReadToEnd();
                    reader.Close();
                }
            }

            return fileContent;
        }

        public void SaveBindingOverridesJson(string overridesName, string fileContentInJson)
        {
            File.WriteAllText(Path.Combine(GetOverridesFolderPath(), overridesName + ".json"), fileContentInJson);

        }
    }
}
