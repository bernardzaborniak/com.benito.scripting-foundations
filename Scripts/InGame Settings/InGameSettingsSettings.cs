using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Benito.ScriptingFoundations.Utilities;
using System.IO;

namespace Benito.ScriptingFoundations.InGameSettings
{
    public class InGameSettingsSettings : ScriptableObject
    {
        const string DefaultSettingsPathInResourcesFolder = "Scripting Foundations Settings/InGame Settings Settings";

        [SerializeField]
        IOUtilities.AssigneableGameDataPath inGameSettingsPath = new IOUtilities.AssigneableGameDataPath(IOUtilities.AssigneableGameDataPath.PathPrefix.PersistendData, "InGameSettings");


        public static InGameSettingsSettings GetOrCreateSettings()
        {
            return RessourceSettingsUtilities.GetOrCreateSettingAsset<InGameSettingsSettings>(DefaultSettingsPathInResourcesFolder);
        }
        public string GetInGameSettingsFolderPath()
        {
            string path = inGameSettingsPath.GetPath();
            IOUtilities.EnsurePathExists(path);
            return path;
        }
    }
}
