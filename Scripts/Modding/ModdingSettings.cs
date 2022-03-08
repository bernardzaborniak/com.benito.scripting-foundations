using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Benito.ScriptingFoundations.Utilities;
using System.IO;

namespace Benito.ScriptingFoundations.Modding
{
    /// <summary>
    /// Holds References to where to export and import Mods.
    /// Used in Editor for exporting and inGame for importing.
    /// </summary>
    public class ModdingSettings : ScriptableObject
    {
        const string DefaultSettingsPathInResourcesFolder = "Settings/Modding Settings";
        [HideInInspector]
        public string lastExportModPath;

        [SerializeField]
        IOUtilities.AssigneableGameDataPath modFolderPath = new IOUtilities.AssigneableGameDataPath(IOUtilities.AssigneableGameDataPath.PathPrefix.PersistendData, "Mods");

    
        public static ModdingSettings GetOrCreateSettings()
        {
            return RessourceSettingsUtilities.GetOrCreateSettingAsset<ModdingSettings>(DefaultSettingsPathInResourcesFolder);
        }
        public string GetModFolderPath()
        {
            string path = modFolderPath.GetPath();
            IOUtilities.EnsurePathExists(path);
            return path;
        }
    }
}

