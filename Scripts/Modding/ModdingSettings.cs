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

        public enum PathPrefix
        {
            PersistendData,
            GameData,
            None
        }

        [Space(10)]
        [Tooltip("Combines persistent or game data path with the following string")]
        [SerializeField] PathPrefix modFolderPathPrefix;
        [Tooltip("Leave out the / or \\ at the start of this string")]
        [SerializeField] string modFolderPath;

    
        public static ModdingSettings GetOrCreateSettings()
        {
            return SettingsUtilities.GetOrCreateSettingAsset<ModdingSettings>(DefaultSettingsPathInResourcesFolder);
        }
        public string GetModFolderPath()
        {
            if (modFolderPath == string.Empty)
                return null;

            switch (modFolderPathPrefix)
            {
                case PathPrefix.PersistendData:
                    {
                        return Path.Combine(Application.persistentDataPath, modFolderPath);
                    }

                case PathPrefix.GameData:
                    {
                        return Path.Combine(Application.dataPath, modFolderPath);
                    }

                case PathPrefix.None:
                    {
                        return modFolderPath;
                    }
            }
            return null;
        }


    }
}

