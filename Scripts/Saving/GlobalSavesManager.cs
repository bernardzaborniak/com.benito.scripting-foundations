using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Benito.ScriptingFoundations.Managers;
using Benito.ScriptingFoundations.BSceneManagement;
using System.IO;


namespace Benito.ScriptingFoundations.Saving
{
    /// <summary>
    /// Takes care of loading and saving different savegames to files.
    /// </summary>
    public class GlobalSavesManager : SingletonManagerGlobal
    {
        public enum State 
        { 
            Idle,
            CreatingSceneSave,
            ReadingSceneSave,
            LoadingSceneSave,
            CreatingProgressSave
        }

        SceneSavegame currentyLoadingSave;

        public State ManagerState { get; private set; }


        public override void InitialiseManager()
        {
            //throw new System.NotImplementedException();
        }

        public override void UpdateManager()
        {
            if(ManagerState == State.LoadingSceneSave)
            {

            }
        }

        public void CreateSceneSave(List<SaveableObjectData> objectsData)
        {
            ManagerState = State.CreatingSceneSave;

            SceneSavegame save = new SceneSavegame(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, objectsData);
            string contents = save.GetJsonString();
            File.WriteAllText(Path.Combine(Application.persistentDataPath, "Saves/test.json"), contents);

            ManagerState = State.Idle;
        }

        public void LoadSceneSave(string saveFilePath, string transitionSceneName,
            GameObject exitCurrentSceneFadePrefab = null, GameObject enterTransitionSceneFadePrefab = null,
            GameObject exitTransitiontSceneFadePrefab = null, GameObject enterNextSceneFadePrefab = null)
        {
            ManagerState = State.LoadingSceneSave;

            StreamReader reader = new StreamReader(saveFilePath);
            string fileContent = reader.ReadToEnd();
            reader.Close();

            currentyLoadingSave = SceneSavegame.CreateFromJsonString(fileContent);
            Debug.Log("LoadSceneSave for scene: " + currentyLoadingSave.GetSceneName());

            BSceneManager sceneManager = GlobalManagers.Get<BSceneManager>();

            sceneManager.LoadSceneSaveThroughTransitionScene(currentyLoadingSave, transitionSceneName,
               exitCurrentSceneFadePrefab, enterTransitionSceneFadePrefab, exitTransitiontSceneFadePrefab, enterNextSceneFadePrefab);

            sceneManager.OnTransitionFinishes += OnTransitionToSavedSceneFinishes;
        }

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

        public void OnTransitionToSavedSceneFinishes()
        {
            ManagerState = State.Idle;
            GlobalManagers.Get<BSceneManager>().OnTransitionFinishes -= OnTransitionToSavedSceneFinishes;
        }
    }
}
