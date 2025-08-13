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
using UnityEngine.SceneManagement;
using System.Diagnostics;


namespace Benito.ScriptingFoundations.Saving
{
    /// <summary>
    ///  Takes care of loading and saving different savegames to files.
    /// </summary>
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

        public float ReadSceneLoadFileprogress { get; private set; }

        Stopwatch stopwatch;

        // References for creating Save
        SaveableObjectsSceneManager saveableObjectsSceneManager;
        string createSceneSaveFolderPathInSavesFolder;
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

            bool waitedOneFrameBeforeInvokingOnFinished;

            public CreateSceneSaveJsonStringBudgetedOperation(SceneSavegame savegame, float timeBudget)
            {
                this.savegame = savegame;
                this.TimeBudget = timeBudget;
                lastStoppedIndex = 0;
                Finished = false;
                waitedOneFrameBeforeInvokingOnFinished = false;

                stringBuilder = new StringBuilder(savegame.SceneName + "\n", savegame.SavedObjects.Count * 200);
            }

            public void Update(float deltaTime)
            {
                Profiler.BeginSample("GlobalSavesManager.CreateSceneSaveJsonStringBudgetedOperation.Update");

                float startUpdateTime = Time.realtimeSinceStartup;

                for (int i = lastStoppedIndex; i < savegame.SavedObjects.Count; i++)
                {
                    stringBuilder.AppendLine(JsonUtility.ToJson(savegame.SavedObjects[i], false));

                    if (Time.realtimeSinceStartup - startUpdateTime > TimeBudget)
                    {
                        lastStoppedIndex = i + 1;
                        Progress = (1.0f * i) / savegame.SavedObjects.Count;

                        Profiler.EndSample();
                        return;
                    }
                }

                lastStoppedIndex = savegame.SavedObjects.Count;
                Progress = 1;
                Profiler.EndSample();

                // why do we wait here actually?
                if (waitedOneFrameBeforeInvokingOnFinished)
                {
                    Finished = true;
                    OnCreatingJsonStringFinished?.Invoke(stringBuilder.ToString());
                }
                else
                {
                    waitedOneFrameBeforeInvokingOnFinished = true;
                }           
            }
        }

        CreateSceneSaveJsonStringBudgetedOperation createSceneSaveJsonStringBudgetedOperation;

        #endregion

        public override void InitialiseManager()
        {
            ManagerState = State.Idle;
            CreatingSceneSaveState = SceneSavingState.Idle;

            stopwatch = new Stopwatch();
        }

        public override void UpdateManager()
        {
            if (ManagerState == State.CreatingSceneSave)
            {
                if (saveableObjectsSceneManager.ManagerState == SaveableObjectsSceneManager.State.SavingSceneSave)
                {
                    CreateSceneSaveFileProgress = saveableObjectsSceneManager.SavingProgress * 0.2f;
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
        public void CreateSceneSaveForCurrentScene(string pathInSavesFolder, string savegameName, SceneSavegameType savegameType, string unitySceneName, string missionName, Texture2D savegamePreviewImage = null)
        {
            SceneSavegameInfo info = new SceneSavegameInfo(savegameName, savegameType, unitySceneName, missionName);
            CreateSceneSaveForCurrentScene(pathInSavesFolder, info, savegamePreviewImage);
        }

        /// <summary>
        /// Write pathInSavesFolder without actual save name .
        /// Write saveName without file extension.
        /// </summary>
        public void CreateSceneSaveForCurrentScene(string folderPathInSavesFolder, SceneSavegameInfo savegameInfo, Texture2D savegamePreviewImage = null)
        {
            Debug.Log($"[GlobalSavesManager] Start CreateSceneSaveForCurrentScene");
            stopwatch.Start();

            if (ManagerState != State.Idle)
            {
                Debug.LogError($"[GlobalSavesManager] Cant Create Scene Save, as globals saves manager is doing something else:  " + ManagerState);
                return;
            }

            saveableObjectsSceneManager = LocalSceneManagers.Get<SaveableObjectsSceneManager>();

            if (saveableObjectsSceneManager == null)
            {
                Debug.LogError("[GlobalSavesManager] SaveableObjectsSceneManager not present in scene. Saving for current only works if an LocalSceneManager with a SaveableObjectsSceneManager is present in the scene");
                return;
            }

            ManagerState = State.CreatingSceneSave;
            CreatingSceneSaveState = SceneSavingState.SceneManagerIsSavingObjects;

            createSceneSaveFolderPathInSavesFolder = folderPathInSavesFolder;
            createSceneSaveInfo = savegameInfo;
            createSavePreviewImage = savegamePreviewImage;

            saveableObjectsSceneManager.OnSavingFinished += CreateSceneSaveForCurrentSceneOnSceneManagerFinished;
            saveableObjectsSceneManager.SaveAllObjects();
        }

        void CreateSceneSaveForCurrentSceneOnSceneManagerFinished(List<SaveableSceneObjectData> objectsData)
        {
            saveableObjectsSceneManager.OnSavingFinished -= CreateSceneSaveForCurrentSceneOnSceneManagerFinished;

            SceneSavegame save = new SceneSavegame(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, objectsData);

            createSceneSaveJsonStringBudgetedOperation = new CreateSceneSaveJsonStringBudgetedOperation(save, SavingSettings.GetOrCreateSettings().savingSceneSaveMsBudgetPerFrame / 1000);
            createSceneSaveJsonStringBudgetedOperation.OnCreatingJsonStringFinished += OnCreatingSceneSaveJsonStringFinished;
            CreatingSceneSaveState = SceneSavingState.CreatingJsonString;

            Debug.Log($"[GlobalSavesManager] Start Creating Scene Save Json String");
        }

        void OnCreatingSceneSaveJsonStringFinished(string jsonString)
        {    
            Profiler.BeginSample("[GlobalSavesManager] GlobalSavesManager.OnCreatingSceneSaveJsonStringFinished");

            createSceneSaveJsonStringBudgetedOperation.OnCreatingJsonStringFinished -= OnCreatingSceneSaveJsonStringFinished;

            CreatingSceneSaveState = SceneSavingState.WritingToFile;

            string savePath = Path.Combine(SavingSettings.GetOrCreateSettings().GetSavesFolderPath(), createSceneSaveFolderPathInSavesFolder);
            IOUtilities.EnsurePathExists(savePath);
            File.WriteAllText(Path.Combine(savePath, createSceneSaveInfo.savegameName + ".bsave"), jsonString);

            // 2. Create Saveinfo and 
            string saveInfoContent = JsonUtility.ToJson(createSceneSaveInfo);
            File.WriteAllText(Path.Combine(savePath, createSceneSaveInfo.savegameName + ".json"), saveInfoContent);

            // 3. Create optional preview Image
            if (createSavePreviewImage != null)
            {
                byte[] imageBytes = createSavePreviewImage.EncodeToPNG();
                File.WriteAllBytes(Path.Combine(savePath, createSceneSaveInfo.savegameName + ".png"), imageBytes);
            }

            // 4. Reset Values after completing Save
            ManagerState = State.Idle;
            CreatingSceneSaveState = SceneSavingState.Idle;
            saveableObjectsSceneManager = null;
            CreateSceneSaveFileProgress = 0;
            createSceneSaveFolderPathInSavesFolder = "";
            createSceneSaveInfo = null;
            createSavePreviewImage = null;

            // 5. Call callbacks
            OnCreatingSceneSaveFileFinished?.Invoke();

            Profiler.EndSample();

            stopwatch.Stop();
            Debug.Log($"[GlobalSavesManager] Finished Writing Save, took {(float)stopwatch.Elapsed.TotalSeconds} seconds");
            stopwatch.Reset();
        }

        void OnGetJsonStringAsyncProgressUpdate(float progress)
        {
            CreateSceneSaveFileProgress = progress;
        }

        #endregion

        // TODO move all scene loading into BScene Manager? it can just use the saves manager methods

        #region Load Scene Save Infos

        public List<(SceneSavegameInfo info, Texture2D image)> GetSceneSavegameInfosInsideFolder(string folderPathInSavesFolder)
        {
            List<(SceneSavegameInfo info, Texture2D image)> infoList = new List<(SceneSavegameInfo info, Texture2D image)>();

            DirectoryInfo directoryInfo = new DirectoryInfo(Path.Combine(SavingSettings.GetOrCreateSettings().GetSavesFolderPath(), folderPathInSavesFolder));

            IOUtilities.EnsurePathExists(directoryInfo.FullName);

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
        public async Task<SceneSavegame> ReadSceneSaveFileAsync(string folderPathInSavesFolder, string savefileName)
        {
            return await ReadSceneSaveFileAsync(Path.Combine(folderPathInSavesFolder, savefileName));
        }


        /// <summary>
        ///  Write saveFilePathInSavesFolder without file extension. Called by save game scene transition.
        /// </summary>
        public async Task<SceneSavegame> ReadSceneSaveFileAsync(string saveFilePathInSavesFolder)
        {
            Debug.Log($"[GlobalSavesManager] Start Reading Save File");
            stopwatch.Start();

            string fileContent;
            string path = Path.Combine(SavingSettings.GetOrCreateSettings().GetSavesFolderPath(), saveFilePathInSavesFolder) + ".bsave";

            using (StreamReader reader = new StreamReader(path))
            {
                fileContent = await reader.ReadToEndAsync();
                reader.Close();
            }

            var progress = new Progress<float>(OnReadSceneSaveFileAsyncProgressUpdate);
            var result = await SceneSavegameUtility.CreateSavegameFromJsonStringAsync(fileContent, progress);

            stopwatch.Stop();
            Debug.Log($"[GlobalSavesManager] Finished reading Save File, took {(float)stopwatch.Elapsed.TotalSeconds} seconds");
            stopwatch.Reset();

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
        /// 

        // TODO move this into BScene Mangement somehow
        public void LoadSceneSave(SceneSavegame sceneSavegame)
        {
            Debug.Log($"[GlobalSavesManager] Start Loading Scene Save");
            stopwatch.Start();


            if (ManagerState != State.Idle)
            {
                Debug.LogError("[GlobalSavesManager] Cant Load Scene Save, as globals saves manager is doing something else:  " + ManagerState);
                return;
            }

            saveableObjectsSceneManager = LocalSceneManagers.Get<SaveableObjectsSceneManager>();

            if (saveableObjectsSceneManager == null)
            {
                Debug.LogError("[GlobalSavesManager] SaveableObjectsSceneManager not present in scene Loading only works if an LocalSceneManager with a SaveableObjectsSceneManager is present in the scene");
                return;
            }


            ManagerState = State.LoadingSceneSave;

            saveableObjectsSceneManager.OnLoadingFinished += LoadSceneSaveOnLoadingFinished;
            saveableObjectsSceneManager.LoadFromSaveData(sceneSavegame.SavedObjects);
        }

        void LoadSceneSaveOnLoadingFinished()
        {
            stopwatch.Stop();
            Debug.Log($"[GlobalSavesManager] Finished loading Scene Save, took {(float)stopwatch.Elapsed.TotalSeconds} seconds");
            stopwatch.Reset();


            saveableObjectsSceneManager.OnLoadingFinished -= LoadSceneSaveOnLoadingFinished;


            ManagerState = State.Idle;
            OnLoadingSceneSaveFileCompleted?.Invoke();

        }

        /*
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

            BSceneLoader sceneManager = GlobalManagers.Get<BSceneLoader>();

            sceneManager.LoadSceneSaveThroughTransitionScene(SceneSavegame.GetTargetSceneFromSceneSavegamePath(fullSaveFilePath), transitionSceneName, saveFilePathInsideSavesFolder,
               exitCurrentSceneFadePrefab, enterTransitionSceneFadePrefab, exitTransitiontSceneFadePrefab, enterNextSceneFadePrefab);

            sceneManager.OnTransitionFinishes += OnTransitionToSavedSceneFinishes;
        }

        void OnTransitionToSavedSceneFinishes()
        {
            ManagerState = State.Idle;
            GlobalManagers.Get<BSceneLoader>().OnTransitionFinishes -= OnTransitionToSavedSceneFinishes;
            OnLoadingSceneSaveFileCompleted.Invoke();
        }

        */

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
