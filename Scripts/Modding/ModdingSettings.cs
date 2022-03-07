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
        [Tooltip("Leave out the / or \\ at the start of this string, use \\ for subfolders")]
        [SerializeField] string modFolderRelativePath = "Mods";

    
        public static ModdingSettings GetOrCreateSettings()
        {
            return RessourceSettingsUtilities.GetOrCreateSettingAsset<ModdingSettings>(DefaultSettingsPathInResourcesFolder);
        }
        public string GetModFolderPath()
        {
            string path = null;

            if (modFolderRelativePath == string.Empty)
                return path;

            switch (modFolderPathPrefix)
            {
                case PathPrefix.PersistendData:
                    {
                        path = Path.Combine(Application.persistentDataPath, modFolderRelativePath);
                        break;
                    }

                case PathPrefix.GameData:
                    {
                        path = Path.Combine(Application.dataPath, modFolderRelativePath);
                        break;
                    }

                case PathPrefix.None:
                    {
                        path = modFolderRelativePath;
                        break;
                    }
            }

            IOUtilities.EnsurePathExists(path);

            return path;
        }
    }
}

