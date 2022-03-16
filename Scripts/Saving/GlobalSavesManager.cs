using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Benito.ScriptingFoundations.Managers;
using Benito.ScriptingFoundations.BSceneManagement;
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
        Task<SceneSavegame> readSceneSaveFileTask;

        public enum State
        {
            Idle,
            LoadingSceneSave,
            CreatingSceneSave
        }

        public float ReadSceneSaveFileProgress { get; private set;}
        public float CreateSceneSaveFileProgress { get; private set;}
        public State ManagerState { get; private set; }

        SaveableObjectsSceneManager sceneManagerForSavingScene;


        public override void InitialiseManager()
        {
        }

        public override void UpdateManager()
        {
            if(ManagerState == State.CreatingSceneSave)
            {
                if(sceneManagerForSavingScene.ManagerState == SaveableObjectsSceneManager.State.SavingSceneSave)
                {
                    CreateSceneSaveFileProgress = sceneManagerForSavingScene.SavingProgress * 0.2f;
                }
                else
                {
                    CreateSceneSaveFileProgress = 0.2f + CreateSceneSaveFileProgress * 0.8f;
                }
            }
        }

        public void CreateSceneSaveForCurrentScene()
        {
            if(ManagerState != State.Idle)
            {
                Debug.LogError("Cant Create Scene Save, as globals saves manager is doing something else:  " + ManagerState);
                return;
            }

            sceneManagerForSavingScene = LocalSceneManagers.Get<SaveableObjectsSceneManager>();

            if(sceneManagerForSavingScene == null)
            {
                Debug.LogError("Saving for current only works if LocalSceneManagers with a SaveableObjectsSceneManageris present in the scene");
                return;
            }

            ManagerState = State.CreatingSceneSave;

            sceneManagerForSavingScene.SaveAllObjects();
            sceneManagerForSavingScene.OnSavingFinished += CreateSceneSaveForCurrentSceneOnSceneManagerFinished;          
        }

        async void CreateSceneSaveForCurrentSceneOnSceneManagerFinished(List<SaveableObjectData> objectsData)
        {
            sceneManagerForSavingScene.OnSavingFinished -= CreateSceneSaveForCurrentSceneOnSceneManagerFinished;

            Debug.Log("CreateSceneSaveForCurrentSceneOnSceneManagerFinished");
            SceneSavegame save = new SceneSavegame(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, objectsData);

            var progress = new Progress<float>(OnGetJsonStringAsyncProgressUpdate);

            string contents = await SceneSavegameUtility.ConvertSaveGameToJsonStringAsync(save,progress);
            Debug.Log("after save.GetJsonStringAsync");

            await File.WriteAllTextAsync(Path.Combine(Application.persistentDataPath, "Saves/test.json"), contents);

            ManagerState = State.Idle;
            sceneManagerForSavingScene = null;
            CreateSceneSaveFileProgress = 0;
        }

        void OnGetJsonStringAsyncProgressUpdate(float progress)
        {
            CreateSceneSaveFileProgress = progress;
        }

        void OnReadSceneSaveFileAsyncProgressUpdate (float progress)
        {
            ReadSceneSaveFileProgress = progress;
        }

        public async Task<SceneSavegame> ReadSceneSaveFileAsync(string saveFilePath)
        {
            string fileContent;

            using (StreamReader reader = new StreamReader(saveFilePath))
            {
                fileContent = await reader.ReadToEndAsync();
                reader.Close();
            }

            var progress = new Progress<float>(OnReadSceneSaveFileAsyncProgressUpdate);
            var result = await SceneSavegameUtility.CreateSavegameFromJsonStringAsync(fileContent, progress);

            return result;
        }

        /// <summary>
        /// Automaticly switches to target scene and loads savegame using BSceneManager
        /// </summary>
        public void LoadSceneSave(string saveFilePath, string transitionSceneName,
            GameObject exitCurrentSceneFadePrefab = null, GameObject enterTransitionSceneFadePrefab = null,
            GameObject exitTransitiontSceneFadePrefab = null, GameObject enterNextSceneFadePrefab = null)
        {
            if (ManagerState != State.Idle)
            {
                Debug.LogError("Cant Create Scene Save, as globals saves manager is doing something else:  " + ManagerState);
                return;
            }

            ManagerState = State.LoadingSceneSave;

            BSceneManager sceneManager = GlobalManagers.Get<BSceneManager>();

            sceneManager.LoadSceneSaveThroughTransitionScene(SceneSavegame.GetTargetSceneFromSceneSavegamePath(saveFilePath), transitionSceneName, saveFilePath,
               exitCurrentSceneFadePrefab, enterTransitionSceneFadePrefab, exitTransitiontSceneFadePrefab, enterNextSceneFadePrefab);

            sceneManager.OnTransitionFinishes += OnTransitionToSavedSceneFinishes;
        }

        void OnTransitionToSavedSceneFinishes()
        {
            ManagerState = State.Idle;
            GlobalManagers.Get<BSceneManager>().OnTransitionFinishes -= OnTransitionToSavedSceneFinishes;
        }     
    }
}
