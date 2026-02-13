using Benito.ScriptingFoundations.Managers;
using Benito.ScriptingFoundations.Saving.SceneSaves;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Benito.ScriptingFoundations.Saving.GlobalSavesManager;

namespace Benito.ScriptingFoundations.Saving
{
    /// <summary>
    /// Derive it with just the info specifying which sceneSave info your game uses
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SceneSaveInfoLoader<T> : SingletonManagerGlobal where T: SceneSaveInfo, new()
    {
        [SerializeField]
        List<T> allSceneSaveInfos = new List<T>(); // loaded on start and updated dynamically

        public override void InitialiseManager()
        {
            GlobalSavesManager savesManager = GlobalManagers.Get<GlobalSavesManager>();
            allSceneSaveInfos = savesManager.GetSceneSaveInfosInsideFolderAndSubFolders<T>("");
            Debug.Log($"SceneSaveInfoLoader Initialize: allSceneSaveInfos.Count: {allSceneSaveInfos.Count} ");

            savesManager.OnCreatingSceneSaveFileFinished += UpdateInfosOnCreatedSave;
            savesManager.OnDeletedSceneSaveFile += UpdateInfosOnDeletedSave;
        }

        public override void UpdateManager()
        {
            
        }

        public List<T> GetLoadedSceneSaveInfos()
        {
            return allSceneSaveInfos;
        }

        // Check by name, as info reference will be recreated
        T GetInfoFromLoadedInfos(string saveName)
        {
            return allSceneSaveInfos.SingleOrDefault(info => info.saveName == saveName);
        }

        void UpdateInfosOnCreatedSave(SceneSaveInfo info)
        {
            // check if overwriting
            T castedInfo = info as T;
            T overwrittenInfo = GetInfoFromLoadedInfos(castedInfo.saveName);
            if (overwrittenInfo != null) allSceneSaveInfos.Remove(overwrittenInfo);
            allSceneSaveInfos.Add(castedInfo);
        }

        void UpdateInfosOnDeletedSave(SceneSaveInfo info)
        {
            T castedInfo = info as T;
            T infoToDelete = GetInfoFromLoadedInfos(castedInfo.saveName);
            allSceneSaveInfos.Remove(infoToDelete);
        }
    }
}
