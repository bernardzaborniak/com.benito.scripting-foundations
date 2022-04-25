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
            createSceneSavePathInSavesFolder = pathInSavesFolder;
            createSceneSaveInfo = savegameInfo;
            createSavePreviewImage = savegamePreviewImage;

            sceneManagerForSavingScene.OnSavingFinished += CreateSceneSaveForCurrentSceneOnSceneManagerFinished;
            sceneManagerForSavingScene.SaveAllObjects();
        }

        async void CreateSceneSaveForCurrentSceneOnSceneManagerFinished(List<SaveableSceneObjectData> objectsData)
        {
            sceneManagerForSavingScene.OnSavingFinished -= CreateSceneSaveForCurrentSceneOnSceneManagerFinished;

            SceneSavegame save = new SceneSavegame(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, objectsData);

            // 1. Create actual Savefile
            var progress = new Progress<float>(OnGetJsonStringAsyncProgressUpdate);
            string contents = await SceneSavegameUtility.ConvertSaveGameToJsonStringAsync(save, progress);
            string savePath = Path.Combine(SavingSettings.GetOrCreateSettings().GetSavesFolderPath(), createSceneSavePathInSavesFolder);
            IOUtilities.EnsurePathExists(savePath);
            //await File.WriteAllTextAsync(Path.Combine(savePath, createSceneSaveInfo.savegameName + ".bsave"), contents);

            // 2. Create Saveinfo and 
            string saveInfoContanets = JsonUtility.ToJson(createSceneSaveInfo);
            await File.WriteAllTextAsync(Path.Combine(savePath, createSceneSaveInfo.savegameName + ".json"), saveInfoContanets);
            
            // 3. Create optional preview Image
            if(createSavePreviewImage != null)
            {
                byte[] bytes = createSavePreviewImage.EncodeToPNG();
                await File.WriteAllBytesAsync(Path.Combine(savePath, createSceneSaveInfo.savegameName + ".png"), bytes);
            }

            // 4. Reset Values after completing Save
            ManagerState = State.Idle;
            sceneManagerForSavingScene = null;
            CreateSceneSaveFileProgress = 0;
            createSceneSavePathInSavesFolder = "";
            createSceneSaveInfo = null;
            createSavePreviewImage = null;

            // 5. Call callbacks
            OnCreatingSceneSaveFileFinished?.Invoke();
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
