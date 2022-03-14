using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Benito.ScriptingFoundations.Managers;
using Benito.ScriptingFoundations.BSceneManagement;
using System.IO;
using System.Threading.Tasks;

using System.Diagnostics;


namespace Benito.ScriptingFoundations.Saving
{
    /// <summary>
    /// Takes care of loading and saving different savegames to files.
    /// </summary>
    public class GlobalSavesManager : SingletonManagerGlobal
    {
        Stopwatch stopwatch = new Stopwatch();

        Task<SceneSavegame> readSceneSaveFileTask;

        public enum State
        {
            Idle,
            ReadingSceneSaveFile,

        }

        // SceneSavegame currentyLoadingSave;

        public State ManagerState { get; private set; }


        public override void InitialiseManager()
        {
            //throw new System.NotImplementedException();
        }

        public override void UpdateManager()
        {
            /*if (ManagerState == State.ReadingSceneSaveFile)
            {
                if (readSceneSaveFileTask.IsCompleted)
                {
                    OnReadingSceneSaveFileFinished(readSceneSaveFileTask.Result);

                    readSceneSaveFileTask = null;
                }
            }*/
        }

        public void CreateSceneSave(List<SaveableObjectData> objectsData)
        {
            stopwatch.Start();

            //ManagerState = State.CreatingSceneSave;

            SceneSavegame save = new SceneSavegame(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, objectsData);
            string contents = save.GetJsonString();
            File.WriteAllText(Path.Combine(Application.persistentDataPath, "Saves/test.json"), contents);

            ManagerState = State.Idle;

            stopwatch.Stop();
            UnityEngine.Debug.Log("stopwatch GlobalSavesManager.CreateSceneSave took " + stopwatch.Elapsed.TotalSeconds + " s");
        }

        public async Task<SceneSavegame> ReadSceneSaveFile(string saveFilePath)
        {
            StreamReader reader = new StreamReader(saveFilePath);
            var fileContent = await reader.ReadToEndAsync();
            reader.Close();

            var result = await Task.Run(() =>
            {
                return SceneSavegame.CreateFromJsonString(fileContent);
            });

            return result;
        }

        public async void LoadSceneSave(string saveFilePath, string transitionSceneName,
            GameObject exitCurrentSceneFadePrefab = null, GameObject enterTransitionSceneFadePrefab = null,
            GameObject exitTransitiontSceneFadePrefab = null, GameObject enterNextSceneFadePrefab = null)
        {
            ManagerState = State.ReadingSceneSaveFile;

            SceneSavegame readSavegame = await ReadSceneSaveFile(saveFilePath);
            BSceneManager sceneManager = GlobalManagers.Get<BSceneManager>();

            sceneManager.LoadSceneSaveThroughTransitionScene(readSavegame, transitionSceneName,
               exitCurrentSceneFadePrefab, enterTransitionSceneFadePrefab, exitTransitiontSceneFadePrefab, enterNextSceneFadePrefab);

            sceneManager.OnTransitionFinishes += OnTransitionToSavedSceneFinishes;
        }

        /*void OnReadingSceneSaveFileFinished(SceneSavegame readSavegame)
        {
            BSceneManager sceneManager = GlobalManagers.Get<BSceneManager>();

            sceneManager.LoadSceneSaveThroughTransitionScene(readSavegame, transitionSceneName,
               exitCurrentSceneFadePrefab, enterTransitionSceneFadePrefab, exitTransitiontSceneFadePrefab, enterNextSceneFadePrefab);

            sceneManager.OnTransitionFinishes += OnTransitionToSavedSceneFinishes;

        }*/

        public async void ReadSceneSaveAsync(string saveFilePath)
        {
            // TODO - next step for large scenes
            //https://stackoverflow.com/questions/13167934/how-to-async-files-readalllines-and-await-for-results

            using (var reader = File.OpenText(saveFilePath))
            {
                var fileText = await reader.ReadToEndAsync();
                // Do something with fileText...
            }

        }

        void OnTransitionToSavedSceneFinishes()
        {
            ManagerState = State.Idle;
            GlobalManagers.Get<BSceneManager>().OnTransitionFinishes -= OnTransitionToSavedSceneFinishes;
        }
    }
}
