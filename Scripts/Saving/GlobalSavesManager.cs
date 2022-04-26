using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using Benito.ScriptingFoundations.Managers;
using Benito.ScriptingFoundations.BSceneManagement;
using Benito.ScriptingFoundations.Utilities;
using Benito.ScriptingFoundations.Optimisation;
using System.IO;
using System;
using System.Text;
using System.Threading.Tasks;

using Debug = UnityEngine.Debug;


namespace Benito.ScriptingFoundations.Saving
{
    // Takes care of loading and saving different savegames to files.
    public class GlobalSavesManager : SingletonManagerGlobal
    {
        #region Fields

        //State
        public enum State
        {
            Idle,
            LoadingSceneSave,
            CreatingSceneSave
        }
        public State ManagerState { get; private set; }

        public enum SceneSavingState 
        { 
            Idle,
            SceneManagerIsSavingObjects,
            CreatingJsonString,
            WritingToFile
        }

        public SceneSavingState CreatingSceneSaveState { get; private set; }


        // Progress
        public float ReadSceneSaveFileProgress { get; private set; }
        public float CreateSceneSaveFileProgress { get; private set; }

        // References for creating Save
        SaveableObjectsSceneManager sceneManagerForSavingScene;
        string createSceneSavePathInSavesFolder;
        SceneSavegameInfo createSceneSaveInfo;
        Texture2D createSavePreviewImage;

        // OnFinished Callbacks
        public Action OnCreatingSceneSaveFileFinished;
        public Action OnCreatingProgressSaveFileFinished; 

        public Action OnLoadingSceneSaveFileCompleted;

        #endregion

        #region Budgeted Operation

        public class CreateSceneSaveJsonStringBudgetedOperation : IBudgetedOperation
        {

            public bool Finished { get; private set; }
            public float Progress { get; private set; }
            public float TimeBudget { get; private set; }

            SceneSavegame savegame;
            int lastStoppedIndex;

            StringBuilder stringBuilder;

            public Action<string> OnCreatingJsonStringFinished;

            public CreateSceneSaveJsonStringBudgetedOperation(SceneSavegame savegame, float timeBudget)
            {
                this.savegame = savegame;
                this.TimeBudget = timeBudget;
                lastStoppedIndex = 0;
                Finished = false;

                stringBuilder = new StringBuilder(savegame.SceneName + "\n", savegame.SavedObjects.Count * 200);
            }

            public void Update(float deltaTime)
            {
                Profiler.BeginSample("Create JSON Budgeted");


                float startUpdateTime = Time.realtimeSinceStartup;

                for (int i = lastStoppedIndex; i < savegame.SavedObjects.Count; i++)
                {
                    stringBuilder.AppendLine(JsonUtility.ToJson(savegame.SavedObjects[i], false));// + "\n";

                    //Debug.Log("check CreateSceneSaveJsonStringBudgetedOperation: Time.realtimeSinceStartup" + Time.realtimeSinceStartup + " Time.realtimeSinceStartup - startUpdateTime " + ((Time.realtimeSinceStartup - startUpdateTime)*1000).ToString("F2"));
                    if (Time.realtimeSinceStartup - startUpdateTime > TimeBudget)
                    {
                        //Debug.Log("stopped CreateSceneSaveJsonStringBudgetedOperation  operation after : " + ((Time.realtimeSinceStartup - startUpdateTime) * 1000).ToString("F2") + " ms");
                        lastStoppedIndex = i + 1;
                        Progress = (1.0f * i) / savegame.SavedObjects.Count;

                        Profiler.EndSample();

                        return;
                    }
                }
                Progress = 1;
                Finished = true;
                //Debug.Log("invoke on saving fisnished");
                OnCreatingJsonStringFinished?.Invoke(stringBuilder.ToString());
                Profiler.EndSample();


            }
        }

        CreateSceneSaveJsonStringBudgetedOperation createSceneSaveJsonStringBudgetedOperation;

        #endregion

        public override void InitialiseManager()
        {
            ManagerState = State.Idle;
            CreatingSceneSaveState = SceneSavingState.Idle;
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

                if(CreatingSceneSaveState == SceneSavingState.CreatingJsonString)
                {
                    createSceneSaveJsonStringBudgetedOperation.Update(Time.deltaTime);
                }
            }
        }

        #region Scene Saves

        #region Create Scene Save

        /// <summary>
        /// Write pathInSavesFolder without actual save name .
        /// Write saveName without file extension.
        /// </summary>
        public void CreateSceneSaveForCurrentScene(string pathInSavesFolder, SceneSavegameInfo savegameInfo, Texture2D savegamePreviewImage = null)
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
            CreatingSceneSaveState = SceneSavingState.SceneManagerIsSavingObjects;

            createSceneSavePathInSavesFolder = pathInSavesFolder;
            createSceneSaveInfo = savegameInfo;
            createSavePreviewImage = savegamePreviewImage;

            sceneManagerForSavingScene.OnSavingFinished += CreateSceneSaveForCurrentSceneOnSceneManagerFinished;
            sceneManagerForSavingScene.SaveAllObjects();
        }

        void CreateSceneSaveForCurrentSceneOnSceneManagerFinished(List<SaveableSceneObjectData> objectsData)
        {
            sceneManagerForSavingScene.OnSavingFinished -= CreateSceneSaveForCurrentSceneOnSceneManagerFinished;

            SceneSavegame save = new SceneSavegame(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, objectsData);

            // 1. Create actual Savefile
            //var progress = new Progress<float>(OnGetJsonStringAsyncProgressUpdate);
            //string contents = await SceneSavegameUtility.ConvertSaveGameToJsonStringAsync(save, progress);
            //string contents = "";
            createSceneSaveJsonStringBudgetedOperation = new CreateSceneSaveJsonStringBudgetedOperation(save, SavingSettings.GetOrCreateSettings().savingSceneSaveMsBudgetPerFrame / 1000);
            createSceneSaveJsonStringBudgetedOperation.OnCreatingJsonStringFinished += OnCreatingSceneSaveJsonStringFinished;
            CreatingSceneSaveState = SceneSavingState.CreatingJsonString;

        }

        async void OnCreatingSceneSaveJsonStringFinished(string jsonString)
        {
            float timeAtStart = Time.realtimeSinceStartup;

            createSceneSaveJsonStringBudgetedOperation.OnCreatingJsonStringFinished -= OnCreatingSceneSaveJsonStringFinished;

            CreatingSceneSaveState = SceneSavingState.WritingToFile;

            string savePath = Path.Combine(SavingSettings.GetOrCreateSettings().GetSavesFolderPath(), createSceneSavePathInSavesFolder);
            IOUtilities.EnsurePathExists(savePath);
            //await File.WriteAllTextAsync(Path.Combine(savePath, createSceneSaveInfo.savegameName + ".bsave"), jsonString);
            File.WriteAllText(Path.Combine(savePath, createSceneSaveInfo.savegameName + ".bsave"), jsonString);

            // 2. Create Saveinfo and 
            string saveInfoContent = JsonUtility.ToJson(createSceneSaveInfo);
            //await File.WriteAllTextAsync(Path.Combine(savePath, createSceneSaveInfo.savegameName + ".json"), saveInfoContent);
            File.WriteAllText(Path.Combine(savePath, createSceneSaveInfo.savegameName + ".json"), saveInfoContent);

            // 3. Create optional preview Image
            if (createSavePreviewImage != null)
            {
                byte[] imageBytes = createSavePreviewImage.EncodeToPNG();
                await File.WriteAllBytesAsync(Path.Combine(savePath, createSceneSaveInfo.savegameName + ".png"), imageBytes);
            }

            // 4. Reset Values after completing Save
            ManagerState = State.Idle;
            CreatingSceneSaveState = SceneSavingState.Idle;
            sceneManagerForSavingScene = null;
            CreateSceneSaveFileProgress = 0;
            createSceneSavePathInSavesFolder = "";
            createSceneSaveInfo = null;
            createSavePreviewImage = null;

            // 5. Call callbacks
            OnCreatingSceneSaveFileFinished?.Invoke();

            Debug.Log("OnCreatingSceneSaveJsonStringFinished took " + (Time.realtimeSinceStartup-timeAtStart));
        }

        void OnGetJsonStringAsyncProgressUpdate(float progress)
        {
            CreateSceneSaveFileProgress = progress;
        }

        #endregion

        #region Load Scene Save Infos

        public List<(SceneSavegameInfo info, Texture2D image)> GetSceneSavegameInfosInsideFolder(string folderPathInSavesFolder)
        {
            List<(SceneSavegameInfo info, Texture2D image)> infoList = new List<(SceneSavegameInfo info, Texture2D image)>();

            DirectoryInfo directoryInfo = new DirectoryInfo(Path.Combine(SavingSettings.GetOrCreateSettings().GetSavesFolderPath(), folderPathInSavesFolder));

            foreach (FileInfo info in directoryInfo.GetFiles())
            {
                if(info.Extension == ".json")
                {
                    infoList.Add(GetSceneSavegameInfoAtPath(Path.Combine(folderPathInSavesFolder, Path.GetFileNameWithoutExtension(info.FullName))));
                }
            }

            return infoList;
        }

        /// <summary>
        /// Write file path without extension.
        /// </summary>
        public (SceneSavegameInfo info, Texture2D image) GetSceneSavegameInfoAtPath(string filePathInSavesFolder)
        {
            (SceneSavegameInfo info, Texture2D image) infoTouple = (null,null);
            string savesFolderPath = SavingSettings.GetOrCreateSettings().GetSavesFolderPath();

            // Read info file
            string infoFileContent;
            string saveInfoPath = Path.Combine(savesFolderPath, filePathInSavesFolder) + ".json";

            using (StreamReader reader = new StreamReader(saveInfoPath))
            {
                infoFileContent = reader.ReadToEnd();
                reader.Close();
            }

            infoTouple.info = JsonUtility.FromJson<SceneSavegameInfo>(infoFileContent);

            // read image, if available
            string previewImagePath = Path.Combine(savesFolderPath, filePathInSavesFolder) + ".png";
            if (File.Exists(previewImagePath))
            {
                //  Texture size does not matter, since LoadImage will replace with with incoming image size.
                infoTouple.image = new Texture2D(2, 2);
                byte[] imageBytes = File.ReadAllBytes(previewImagePath);
                infoTouple.image.LoadImage(imageBytes);
            }


            return infoTouple;
        }

        #endregion

        #region Load Scene Save

        /// <summary>
        ///  Write saveFilePathInSavesFolder without file extension. Called by save game scene transition.
        /// </summary>
        public async Task<SceneSavegame> ReadSceneSaveFileAsync(string saveFilePathInSavesFolder)
        {
            string fileContent;
            string path = Path.Combine(SavingSettings.GetOrCreateSettings().GetSavesFolderPath(), saveFilePathInSavesFolder) + ".bsave";

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
            string fullSaveFilePath = Path.Combine(SavingSettings.GetOrCreateSettings().GetSavesFolderPath(), saveFilePathInsideSavesFolder) + ".bsave";

            if (ManagerState != State.Idle)
            {
                Debug.LogError("Cant Load Scene Save, as globals saves manager is doing something else:  " + ManagerState);
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
            OnLoadingSceneSaveFileCompleted.Invoke();
        }

        #endregion


        #endregion

        #region Progress Saves

        public async void CreateProgressSave<T>(T save, string pathInSavesFolder, string saveName) where T: ISaveableProgress
        {
            string fileString = JsonUtility.ToJson(save);
            string folderPath = Path.Combine(SavingSettings.GetOrCreateSettings().GetSavesFolderPath(), pathInSavesFolder);
            IOUtilities.EnsurePathExists(folderPath);
            await File.WriteAllTextAsync(Path.Combine(folderPath, saveName + ".json"), fileString);
            OnCreatingProgressSaveFileFinished?.Invoke();
        }

        public T ReadProgressSave<T>(string saveFilePathInsideSavesFolder) where T: ISaveableProgress
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
