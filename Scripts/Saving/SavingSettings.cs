using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Benito.ScriptingFoundations.Utilities;

namespace Benito.ScriptingFoundations.Saving
{
    public class SavingSettings : ScriptableObject
    {
        const string DefaultSettingsPathInResourcesFolder = "Settings/Saving Settings";

        [SerializeField]
        IOUtilities.AssigneableGameDataPath savesPath = new IOUtilities.AssigneableGameDataPath(IOUtilities.AssigneableGameDataPath.PathPrefix.PersistendData, "Saves");
        [Space]
        [SerializeField]
        [Tooltip("How many miliseconds does the load system have aviable per frame - higher values will speed up the loading time but introduce stuttering - 1/120 s is pretty good")]
        public float loadingSceneSaveMsBudgetPerFrame = 3f;
        
        public float savingSceneSaveMsBudgetPerFrame = 3f;


        public static SavingSettings GetOrCreateSettings()
        {
            return RessourceSettingsUtilities.GetOrCreateSettingAsset<SavingSettings>(DefaultSettingsPathInResourcesFolder);
        }
        public string GetSavesFolderPath()
        {
            string path = savesPath.GetPath();
            IOUtilities.EnsurePathExists(path);
            return path;
        }
    }
}
