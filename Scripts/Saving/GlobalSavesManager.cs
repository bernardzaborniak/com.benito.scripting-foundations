using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Benito.ScriptingFoundations.Managers;
using Benito.ScriptingFoundations.BSceneManagement;
using Benito.ScriptingFoundations.Utilities;
using System.IO;
using System;
using System.Threading.Tasks;

using Debug = UnityEngine.Debug;


namespace Benito.ScriptingFoundations.Saving
{
    /// <summary>
    /// Takes care of loading and saving different savegames to files.
    /// </summary>
    public class GlobalSavesManager : SingletonManagerGlobal
    {
        #region Fields
        public enum State
        {
            Idle,
            LoadingSceneSave,
            CreatingSceneSave
        }
        public State ManagerState { get; private set; }

        public float ReadSceneSaveFileProgress { get; private set; }
        public float CreateSceneSaveFileProgress { get; private set; }

        SaveableObjectsSceneManager sceneManagerForSavingScene;
        string createSavePathInSavesFolder;
        string createSaveName;

        public Action OnCreatingSceneSaveFileFinished;
        public Action OnCreatingProgressSaveFileFinished;

        #endregion

        public override void InitialiseManager()
        {
        }

        public override void UpdateManager()
        {
            if (ManagerState == State.CreatingSceneSave)
            {
                if (sceneManagerForSavingScene.ManagerState == SaveableObjectsSceneManager.State.SavingSceneSave)
                {
                    CreateSceneSaveFileProgress = sceneManagerForSavingScene.SavingProgress * 0.2f;
                }
                else
                {
                    CreateSceneSaveFileProgress = 0.2f + CreateSceneSaveFileProgress * 0.8f;
                }
            }
        }

        #region Scene Saves

        /// <summary>
        /// Write pathInSavesFolder without actual save name .
        /// Write saveName without file extension.
        /// </summary>
        public void CreateSceneSaveForCurrentScene(string pathInSavesFolder, string saveName)
        {
            if (ManagerState != State.Idle)
            {
                Debug.LogError("Cant Create Scene Save, as globals saves manager is doing something else:  " + ManagerState);
                return;
            }

            sceneManagerForSavingScene = LocalSceneManagers.Get<SaveableObjectsSceneManager>();

            if (sceneManagerForSavingScene == null)
            {
                Debug.LogError("Saving for current only works if an LocalSceneManager with a SaveableObjectsSceneManager is present in the scene");
                return;
            }

            ManagerState = State.CreatingSceneSave;
            createSavePathInSavesFolder = pathInSavesFolder;
            createSaveName = saveName;

            sceneManagerForSavingScene.OnSavingFinished += CreateSceneSaveForCurrentSceneOnSceneManagerFinished;
            sceneManagerForSavingScene.SaveAllObjects();
        }

        async void CreateSceneSaveForCurrentSceneOnSceneManagerFinished(List<SaveableObjectData> objectsData)
        {
            sceneManagerForSavingScene.OnSavingFinished -= CreateSceneSaveForCurrentSceneOnSceneManagerFinished;

            SceneSavegame save = new SceneSavegame(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, objectsData);

            var progress = new Progress<float>(OnGetJsonStringAsyncProgressUpdate);
            string contents = await SceneSavegameUtility.ConvertSaveGameToJsonStringAsync(save, progress);
            string savePath = Path.Combine(SavingSettings.GetOrCreateSettings().GetSavesFolderPath(), createSavePathInSavesFolder);
            IOUtilities.EnsurePathExists(savePath);
            await File.WriteAllTextAsync(Path.Combine(savePath, createSaveName + ".json"), contents);

            ManagerState = State.Idle;
            sceneManagerForSavingScene = null;
            CreateSceneSaveFileProgress = 0;
            createSavePathInSavesFolder = "";
            createSaveName = "";
            OnCreatingSceneSaveFileFinished?.Invoke();
        }

        void OnGetJsonStringAsyncProgressUpdate(float progress)
        {
            CreateSceneSaveFileProgress = progress;
        }

        /// <summary>
        ///  Write saveFilePathInSavesFolder without file extension
        /// </summary>
        public async Task<SceneSavegame> ReadSceneSaveFileAsync(string saveFilePathInSavesFolder)
        {
            string fileContent;
            string path = Path.Combine(SavingSettings.GetOrCreateSettings().GetSavesFolderPath(), saveFilePathInSavesFolder) + ".json";

            using (StreamReader reader = new StreamReader(path))
            {
                fileContent = await reader.ReadToEndAsync();
                reader.Close();
            }

            var progress = new Progress<float>(OnReadSceneSaveFileAsyncProgressUpdate);
            var result = await SceneSavegameUtility.CreateSavegameFromJsonStringAsync(fileContent, progress);

            return result;
        }

        void OnReadSceneSaveFileAsyncProgressUpdate(float progress)
        {
            ReadSceneSaveFileProgress = progress;
        }

        /// <summary>
        /// Automaticly switches to target scene and loads savegame using BSceneManager.
        /// Write saveFilePathInsideSavesFolder without file extension.
        /// </summary>
        public void LoadSceneSave(string saveFilePathInsideSavesFolder, string transitionSceneName,
            GameObject exitCurrentSceneFadePrefab = null, GameObject enterTransitionSceneFadePrefab = null,
            GameObject exitTransitiontSceneFadePrefab = null, GameObject enterNextSceneFadePrefab = null)
        {
            string fullSaveFilePath = Path.Combine(SavingSettings.GetOrCreateSettings().GetSavesFolderPath(), saveFilePathInsideSavesFolder) + ".json";

            if (ManagerState != State.Idle)
            {
                Debug.LogError("Cant Create Scene Save, as globals saves manager is doing something else:  " + ManagerState);
                return;
            }

            ManagerState = State.LoadingSceneSave;

            BSceneManager sceneManager = GlobalManagers.Get<BSceneManager>();

            sceneManager.LoadSceneSaveThroughTransitionScene(SceneSavegame.GetTargetSceneFromSceneSavegamePath(fullSaveFilePath), transitionSceneName, saveFilePathInsideSavesFolder,
               exitCurrentSceneFadePrefab, enterTransitionSceneFadePrefab, exitTransitiontSceneFadePrefab, enterNextSceneFadePrefab);

            sceneManager.OnTransitionFinishes += OnTransitionToSavedSceneFinishes;
        }

        void OnTransitionToSavedSceneFinishes()
        {
            ManagerState = State.Idle;
            GlobalManagers.Get<BSceneManager>().OnTransitionFinishes -= OnTransitionToSavedSceneFinishes;
        }

        #endregion

        #region Progress Saves

        public async void CreateProgressSave<T>(T save, string pathInSavesFolder, string saveName) where T: IProgressSave
        {
            string fileString = JsonUtility.ToJson(save);
            string folderPath = Path.Combine(SavingSettings.GetOrCreateSettings().GetSavesFolderPath(), pathInSavesFolder);
            IOUtilities.EnsurePathExists(folderPath);
            await File.WriteAllTextAsync(Path.Combine(folderPath, saveName + ".json"), fileString);
            OnCreatingProgressSaveFileFinished?.Invoke();
        }

        public T ReadProgressSave<T>(string saveFilePathInsideSavesFolder) where T: IProgressSave
        {
            string fileContent;
            string path = Path.Combine(SavingSettings.GetOrCreateSettings().GetSavesFolderPath(), saveFilePathInsideSavesFolder) + ".json";

            using (StreamReader reader = new StreamReader(path))
            {
                fileContent =  reader.ReadToEnd();
                reader.Close();
            }

            return JsonUtility.FromJson<T>(fileContent);
        }


        #endregion
    }
}
