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
            ReadingSceneSaveFile,
        }

        public float ReadSceneSaveFileProgress { get; private set;}
        public State ManagerState { get; private set; }


        public override void InitialiseManager()
        {
        }

        public override void UpdateManager()
        {
            
        }

        public void CreateSceneSave(List<SaveableObjectData> objectsData)
        {
            SceneSavegame save = new SceneSavegame(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, objectsData);
            string contents = save.GetJsonString();
            File.WriteAllText(Path.Combine(Application.persistentDataPath, "Saves/test.json"), contents);

            ManagerState = State.Idle;
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
            var result = await SceneSavegame.CreateSavegameFromJsonStringAsync(fileContent, progress);

            return result;
        }

        /// <summary>
        /// Automaticly switches to target scene and loads savegame using BSceneManager
        /// </summary>
        public void LoadSceneSave(string saveFilePath, string transitionSceneName,
            GameObject exitCurrentSceneFadePrefab = null, GameObject enterTransitionSceneFadePrefab = null,
            GameObject exitTransitiontSceneFadePrefab = null, GameObject enterNextSceneFadePrefab = null)
        {
            ManagerState = State.ReadingSceneSaveFile;

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
