using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Benito.ScriptingFoundations.Utilities;

namespace Benito.ScriptingFoundations.Saving
{
    [CreateAssetMenu(menuName = "My Assets/SavingSettings")]
    public class SavingSettings : ScriptableObject
    {
        const string DefaultSettingsPathInResourcesFolder = "Settings/Saving Settings";

        [SerializeField]
        IOUtilities.AssigneableGameDataPath savesPath = new IOUtilities.AssigneableGameDataPath(IOUtilities.AssigneableGameDataPath.PathPrefix.PersistendData, "Saves");

        [SerializeField]
        [Tooltip("How many miliseconds does the load system have aviable per frame - higher values will speed up the loading time but introduce stuttering - 1/120 s is pretty good")]
        public float loadingSceneSaveBudgetPerFrame = 0.0083f;
        
        public float savingSceneSaveBudgetPerFrame = 0.0083f;


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
