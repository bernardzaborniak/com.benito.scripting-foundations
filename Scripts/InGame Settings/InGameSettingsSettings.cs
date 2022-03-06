using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Benito.ScriptingFoundations.Utilities;
using System.IO;

public class InGameSettingsSettings : ScriptableObject
{
    const string DefaultSettingsPathInResourcesFolder = "Settings/InGame Settings Settings";

    public enum PathPrefix
    {
        PersistendData,
        GameData,
        None
    }

    [Space(10)]
    [Tooltip("Combines persistent or game data path with the following string")]
    [SerializeField] PathPrefix settingsFolderPathPrefix;
    [Tooltip("Leave out the / or \\ at the start of this string, use \\ for subfolders")]
    [SerializeField] string settingsFolderRelativePath = "InGameSettings";


    public static InGameSettingsSettings GetOrCreateSettings()
    {
        return RessourceSettingsUtilities.GetOrCreateSettingAsset<InGameSettingsSettings>(DefaultSettingsPathInResourcesFolder);
    }
    public string GetInGameSettingsFolderPath()
    {
        string path = null;

        if (settingsFolderRelativePath == string.Empty)
            return path;
      
        switch (settingsFolderPathPrefix)
        {
            case PathPrefix.PersistendData:
                {
                    path =  Path.Combine(Application.persistentDataPath, settingsFolderRelativePath);
                    break;
                }

            case PathPrefix.GameData:
                {
                    path =  Path.Combine(Application.dataPath, settingsFolderRelativePath);
                    break;
                }

            case PathPrefix.None:
                {
                    path = settingsFolderRelativePath;
                    break;
                }
        }

        IOUtilities.EnsurePathExists(path);

        return path;
    }

}
