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

        public float ReadSceneLoadFileProgress { get; private set; }

        Stopwatch stopwatch;

        // References for creating Save
        //SaveableObjectsSceneManager saveableObjectsSceneManager;
        //string createSceneSaveFolderPathInSavesFolder;
        //SceneSavegameInfo createSceneSaveInfo;
        //Texture2D createSavePreviewImage;

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
            public string Result { get; private set; }

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
                    Result = stringBuilder.ToString();
                    OnCreatingJsonStringFinished?.Invoke(Result);
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
        public void CreateSceneSaveForCurrentScene(string pathInSavesFolder, SceneSavegameInfo savegameInfo, Texture2D savegamePreviewImage = null)
        {
            // Verify data before starting coroutine

            if (ManagerState != State.Idle)
            {
                Debug.LogError($"[GlobalSavesManager] Cant Create Scene Save, as globals saves manager is doing something else:  " + ManagerState);
                return;
            }

            SaveableObjectsSceneManager saveableObjectsSceneManager = LocalSceneManagers.Get<SaveableObjectsSceneManager>();

            if (saveableObjectsSceneManager == null)
            {
                Debug.LogError("[GlobalSavesManager] SaveableObjectsSceneManager not present in scene. Saving for current only works if an LocalSceneManager with a SaveableObjectsSceneManager is present in the scene");
                return;
            }

            StartCoroutine(CreateSceneSaveCoroutine(saveableObjectsSceneManager, pathInSavesFolder, savegameInfo, savegamePreviewImage));

        }

        IEnumerator CreateSceneSaveCoroutine(SaveableObjectsSceneManager saveableObjectsSceneManager, string pathInSavesFolder, SceneSavegameInfo savegameInfo, Texture2D savegamePreviewImage = null)
        {
            Debug.Log($"[GlobalSavesManager] Start CreateSceneSaveForCurrentScene");
            stopwatch.Start();

            //  Set up values
            ManagerState = State.CreatingSceneSave;
            CreatingSceneSaveState = SceneSavingState.SceneManagerIsSavingObjects;


            // Save all Objects with local manager, get their save info
            bool savingFinished = false;
            List<SaveableSceneObjectData> objectsData = null;
            Action<List<SaveableSceneObjectData>> savingFinishedHandler = (data) =>
            {
                savingFinished = true;
                objectsData = data;
            };

            saveableObjectsSceneManager.OnSavingFinished += savingFinishedHandler;
            saveableObjectsSceneManager.SaveAllObjects();

            while (!savingFinished)
            {
                // TODO set the progress 
                yield return null;
            }
            saveableObjectsSceneManager.OnSavingFinished -= savingFinishedHandler;

            // Create Scene Save For Current Scene
            Debug.Log($"[GlobalSavesManager] Start Creating Scene Save Json String");
            CreatingSceneSaveState = SceneSavingState.CreatingJsonString;

            SceneSavegame save = new SceneSavegame(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, objectsData);
            createSceneSaveJsonStringBudgetedOperation = new CreateSceneSaveJsonStringBudgetedOperation(save, SavingSettings.GetOrCreateSettings().savingSceneSaveMsBudgetPerFrame / 1000);


            while (!createSceneSaveJsonStringBudgetedOperation.Finished)
            {
                createSceneSaveJsonStringBudgetedOperation.Update(Time.deltaTime);
                // TODO update progress of this
                yield return null;
            }

            // Save file
            CreatingSceneSaveState = SceneSavingState.WritingToFile;

            string savePath = Path.Combine(SavingSettings.GetOrCreateSettings().GetSavesFolderPath(), pathInSavesFolder);
            IOUtilities.EnsurePathExists(savePath);

            Task writeFileTask = File.WriteAllTextAsync(Path.Combine(savePath, savegameInfo.savegameName + ".bsave"), createSceneSaveJsonStringBudgetedOperation.Result);
            yield return new WaitUntil(() => writeFileTask.IsCompleted);

            // 2. Create Saveinfo  
            string saveInfoContent = JsonUtility.ToJson(savegameInfo);

            Task writeSaveInfoTask = File.WriteAllTextAsync(Path.Combine(savePath, savegameInfo.savegameName + ".json"), saveInfoContent);
            yield return new WaitUntil(() => writeSaveInfoTask.IsCompleted);

            // 3. Create optional preview Image
            if (savegamePreviewImage != null)
            {
                byte[] imageBytes = savegamePreviewImage.EncodeToPNG(); // this is quite performance expensive, but cant be put on another thread sadly

                Task writeImageTask = File.WriteAllBytesAsync(Path.Combine(savePath, savegameInfo.savegameName + ".png"), imageBytes);
                yield return new WaitUntil(() => writeImageTask.IsCompleted);
            }

            // 4. Reset Values after completing Save
            ManagerState = State.Idle;
            CreatingSceneSaveState = SceneSavingState.Idle;
            saveableObjectsSceneManager = null;

            // 5. Call callbacks
            OnCreatingSceneSaveFileFinished?.Invoke();

            //Profiler.EndSample();

            stopwatch.Stop();
            Debug.Log($"[GlobalSavesManager] Finished Writing Save, took {(float)stopwatch.Elapsed.TotalSeconds} seconds");
            stopwatch.Reset();
        }

        async Task<byte[]> EncodeToPNGAsync(Texture2D texture)
        {
            return await Task.Run(() =>
            {
                // This runs on a background thread
                return texture.EncodeToPNG();
            });
        }

        #endregion


        #region Load Scene Save Infos

        public List<(SceneSavegameInfo info, Texture2D image)> GetSceneSavegameInfosInsideFolder(string folderPathInSavesFolder)
        {
            List<(SceneSavegameInfo info, Texture2D image)> infoList = new List<(SceneSavegameInfo info, Texture2D image)>();

            DirectoryInfo directoryInfo = new DirectoryInfo(Path.Combine(SavingSettings.GetOrCreateSettings().GetSavesFolderPath(), folderPathInSavesFolder));

            IOUtilities.EnsurePathExists(directoryInfo.FullName);

            foreach (FileInfo info in directoryInfo.GetFiles())
            {
                if (info.Extension == ".json")
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
            (SceneSavegameInfo info, Texture2D image) infoTouple = (null, null);
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
        /// Write saveFilePathInsideSavesFolder without file extension.
        /// </summary>
        /// 

        public void LoadSceneSave(SceneSavegame sceneSavegame)
        {
            // Verify data before starting coroutine
            if (ManagerState != State.Idle)
            {
                Debug.LogError("[GlobalSavesManager] Cant Load Scene Save, as globals saves manager is doing something else:  " + ManagerState);
                return;
            }

            SaveableObjectsSceneManager saveableObjectsSceneManager = LocalSceneManagers.Get<SaveableObjectsSceneManager>();

            if (saveableObjectsSceneManager == null)
            {
                Debug.LogError("[GlobalSavesManager] SaveableObjectsSceneManager not present in scene Loading only works if an LocalSceneManager with a SaveableObjectsSceneManager is present in the scene");
                return;
            }

            StartCoroutine(LoadSceneSaveCoroutine(saveableObjectsSceneManager, sceneSavegame));

        }

        IEnumerator LoadSceneSaveCoroutine(SaveableObjectsSceneManager saveableObjectsSceneManager, SceneSavegame sceneSavegame)
        {
            Debug.Log($"[GlobalSavesManager] Start Loading Scene Save");
            stopwatch.Start();

            ManagerState = State.LoadingSceneSave;

            // Save all Objects with local manager, get their save info
            bool loadingFinished = false;
            Action loadingFinishedHandler = () => loadingFinished = true;

            saveableObjectsSceneManager.OnLoadingFinished += loadingFinishedHandler;
            saveableObjectsSceneManager.LoadFromSaveData(sceneSavegame.SavedObjects);

            while (!loadingFinished)
            {
                // TODO set the progress 
                yield return null;
            }
            saveableObjectsSceneManager.OnLoadingFinished -= loadingFinishedHandler;

            stopwatch.Stop();
            Debug.Log($"[GlobalSavesManager] Finished loading Scene Save, took {(float)stopwatch.Elapsed.TotalSeconds} seconds");
            stopwatch.Reset();

            ManagerState = State.Idle;
            OnLoadingSceneSaveFileCompleted?.Invoke();
        }


        #endregion


        #endregion

        #region Progress Saves

        public async void CreateProgressSave<T>(T save, string pathInSavesFolder, string saveName) where T : ISaveableProgress
        {
            string fileString = JsonUtility.ToJson(save);
            string folderPath = Path.Combine(SavingSettings.GetOrCreateSettings().GetSavesFolderPath(), pathInSavesFolder);
            IOUtilities.EnsurePathExists(folderPath);
            await File.WriteAllTextAsync(Path.Combine(folderPath, saveName + ".json"), fileString);
            OnCreatingProgressSaveFileFinished?.Invoke();
        }

        public T ReadProgressSave<T>(string saveFilePathInsideSavesFolder) where T : ISaveableProgress
        {
            string fileContent;
            string path = Path.Combine(SavingSettings.GetOrCreateSettings().GetSavesFolderPath(), saveFilePathInsideSavesFolder) + ".json";

            using (StreamReader reader = new StreamReader(path))
            {
                fileContent = reader.ReadToEnd();
                reader.Close();
            }

            return JsonUtility.FromJson<T>(fileContent);
        }


        #endregion
    }
}
