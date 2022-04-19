using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Benito.ScriptingFoundations.Saving;
using Benito.ScriptingFoundations.Managers;
using System.Threading.Tasks;

namespace Benito.ScriptingFoundations.BSceneManagement
{
    public class BSceneTransitionLoadSceneSave : BSceneTransition
    {
        // Scenes & Savegames
        string targetScene;
        string transitionScene;
        string savegamePathInSavesFolder;
        SceneSavegame savegame;

        // Fades
        GameObject exitCurrentSceneFadePrefab;
        GameObject enterTransitionSceneFadePrefab;
        GameObject exitTransitionSceneFadePrefab;
        GameObject enterNextSceneFadePrefab;

        BSceneFade exitCurrentSceneFade;
        BSceneFade enterTransitionSceneFade;
        BSceneFade exitTransitionSceneFade;
        BSceneFade enterNextSceneFade;

        // Async operations and Tasks
        AsyncOperation preloadSceneOperation;
        AsyncOperation unloadSceneOperation;
        Task<SceneSavegame> readSceneSaveFileTask;

        // Other
        SaveableObjectsSceneManager saveSceneManager;
        GlobalSavesManager globalSavesManager;

        Transform sceneManagerTransform;

        enum Stage
        {
            Idle,
            PlayingExitCurrentSceneFade,
            WaitingForTransitionSceneToPreload,
            PlayingEnterTransitionSceneFade,
            WaitingForNextSceneToPreloadAndSaveToRead,
            LoadingSaveFile,
            WaitingForTransitionSceneToAllowExit,
            PlayingExitTransitionSceneFade,
            PlayingEnterNextSceneFade,
            Finished
        }

        Stage stage;

        public BSceneTransitionLoadSceneSave(string targetScene, string transitionScene, string savegamePathInSavesFolder, Transform sceneManagerTransform, AsyncOperation preloadSceneOperation,
            GameObject exitCurrentSceneFadePrefab = null, GameObject enterTransitionSceneFadePrefab = null,
            GameObject exitTransitionSceneFadePrefab = null, GameObject enterNextSceneFadePrefab = null)
        {
            stage = Stage.Idle;

            this.targetScene = targetScene;
            this.transitionScene = transitionScene;
            this.savegamePathInSavesFolder = savegamePathInSavesFolder;

            this.sceneManagerTransform = sceneManagerTransform;
            this.preloadSceneOperation = preloadSceneOperation;
         

            this.exitCurrentSceneFadePrefab = exitCurrentSceneFadePrefab;
            this.enterTransitionSceneFadePrefab = enterTransitionSceneFadePrefab;
            this.exitTransitionSceneFadePrefab = exitTransitionSceneFadePrefab;
            this.enterNextSceneFadePrefab = enterNextSceneFadePrefab;
        }


        /// <summary>
        /// Call this to allow exiting a transition scene.
        /// </summary>
        public void OnTransitionSceneAllowsContinuation()
        {
            if (stage == Stage.WaitingForTransitionSceneToAllowExit)
            {
                StartExitTransitionSceneFade();
            }
        }

        public override void UpdateTransition()
        {
            if(stage == Stage.WaitingForTransitionSceneToPreload)
            {
                if (preloadSceneOperation.progress >= 0.9f)
                {
                    OnExitCurrentSceneFadeFinishedAndTransitionSceneIsPreloaded();
                }
            }
            else if(stage == Stage.WaitingForNextSceneToPreloadAndSaveToRead)
            {
                if (preloadSceneOperation.progress >= 0.9f && readSceneSaveFileTask.IsCompleted)
                {
                    OnNextSceneFinishedPreloadingAndSavefileFinishedReading();
                }
            }
        }

        public override void StartTransition()
        {
            StartExitCurrentSceneFade();
        }

        void StartExitCurrentSceneFade()
        {
            if (exitCurrentSceneFadePrefab != null)
            {
                exitCurrentSceneFade = CreateFade(exitCurrentSceneFadePrefab, sceneManagerTransform);
                exitCurrentSceneFade.OnTransitionFinished += OnExitCurrentSceneFadeFinished;
                exitCurrentSceneFade.StartTransition();
                stage = Stage.PlayingExitCurrentSceneFade;
            }
            else
            {
                OnExitCurrentSceneFadeFinished();
            }

            preloadSceneOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(transitionScene);
            preloadSceneOperation.allowSceneActivation = false;
        }

        void OnExitCurrentSceneFadeFinished()
        {
            stage = Stage.WaitingForTransitionSceneToPreload;
        }

        void OnExitCurrentSceneFadeFinishedAndTransitionSceneIsPreloaded()
        {
            preloadSceneOperation.allowSceneActivation = true;
            preloadSceneOperation.completed += OnLoadingTransitionSceneComplete;
        }

        void OnLoadingTransitionSceneComplete(AsyncOperation asyncOperation)
        {
            preloadSceneOperation.completed -= OnLoadingTransitionSceneComplete;

            if (exitCurrentSceneFade)
                GameObject.Destroy(exitCurrentSceneFade.gameObject);

            preloadSceneOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(targetScene, UnityEngine.SceneManagement.LoadSceneMode.Additive);
            preloadSceneOperation.allowSceneActivation = false;

            globalSavesManager = GlobalManagers.Get<GlobalSavesManager>();
            readSceneSaveFileTask = globalSavesManager.ReadSceneSaveFileAsync(savegamePathInSavesFolder);

            StartEnterTransitionSceneFade();
        }

        void StartEnterTransitionSceneFade()
        {
            if (enterTransitionSceneFadePrefab != null)
            {
                enterTransitionSceneFade = CreateFade(enterTransitionSceneFadePrefab, sceneManagerTransform);
                enterTransitionSceneFade.OnTransitionFinished += OnEnterTransitionSceneFadeFinished;
                enterTransitionSceneFade.StartTransition();
                stage = Stage.PlayingEnterTransitionSceneFade;
            }
            else
            {
                OnEnterTransitionSceneFadeFinished();
            }
        }

        void OnEnterTransitionSceneFadeFinished()
        {
            if (enterTransitionSceneFade)
                GameObject.Destroy(enterTransitionSceneFade.gameObject);

            stage = Stage.WaitingForNextSceneToPreloadAndSaveToRead;
        }

        void OnNextSceneFinishedPreloadingAndSavefileFinishedReading()
        {
            preloadSceneOperation.allowSceneActivation = true;
            preloadSceneOperation.completed += OnLoadingNextSceneComplete;
        }

        void OnLoadingNextSceneComplete(AsyncOperation asyncOperation)
        {
            preloadSceneOperation.completed -= OnLoadingNextSceneComplete;
            globalSavesManager = null;
            StartLoadingSavegame();
        }

        void StartLoadingSavegame()
        {
            stage = Stage.LoadingSaveFile;
            savegame = readSceneSaveFileTask.Result;

            saveSceneManager = LocalSceneManagers.Get<SaveableObjectsSceneManager>();
            saveSceneManager.OnLoadingFinished += OnLoadingSavegameFinished;
            saveSceneManager.LoadFromSaveData(savegame.SavedObjects);
        }

        void OnLoadingSavegameFinished()
        {
            saveSceneManager.OnLoadingFinished -= OnLoadingSavegameFinished;

            if (exitTransitionSceneFade)
                GameObject.Destroy(exitTransitionSceneFade.gameObject);

            stage = Stage.WaitingForTransitionSceneToAllowExit;
            saveSceneManager = null;
        }

        void StartExitTransitionSceneFade()
        {
            if (exitTransitionSceneFadePrefab != null)
            {
                exitTransitionSceneFade = CreateFade(exitTransitionSceneFadePrefab, sceneManagerTransform);
                exitTransitionSceneFade.OnTransitionFinished += OnExitTransitionSceneFadeFinished;
                exitTransitionSceneFade.StartTransition();
                stage = Stage.PlayingExitTransitionSceneFade;
            }
            else
            {
                OnExitTransitionSceneFadeFinished();
            }
        }

        void OnExitTransitionSceneFadeFinished()
        {
            unloadSceneOperation = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(transitionScene);
            unloadSceneOperation.completed += OnUnloadTransitionSceneCompleted;
        }

        void OnUnloadTransitionSceneCompleted(AsyncOperation operation)
        {
            unloadSceneOperation.completed -= OnUnloadTransitionSceneCompleted;
            unloadSceneOperation = null;

            if (exitTransitionSceneFade)
                GameObject.Destroy(exitTransitionSceneFade.gameObject);

            StartEnterNextSceneFade();
        }
       
        void StartEnterNextSceneFade()
        {
            if (enterNextSceneFadePrefab != null)
            {
                enterNextSceneFade = CreateFade(enterNextSceneFadePrefab, sceneManagerTransform);
                enterNextSceneFade.OnTransitionFinished += OnEnterNextSceneFadeFinished;
                enterNextSceneFade.StartTransition();
                stage = Stage.PlayingEnterNextSceneFade;
            }
            else
            {
                OnEnterNextSceneFadeFinished();
            }
        }

        void OnEnterNextSceneFadeFinished()
        {
            if (enterNextSceneFade)
                GameObject.Destroy(enterNextSceneFade.gameObject);

            stage = Stage.Finished;
        }

        public override float GetProgress()
        {
            if (stage == Stage.WaitingForNextSceneToPreloadAndSaveToRead)
            {
                return preloadSceneOperation.progress / 0.9f * 0.1f + (globalSavesManager.ReadSceneSaveFileProgress*0.2f);
            }
            else if (stage == Stage.LoadingSaveFile)
            {
                return 0.3f + saveSceneManager.LoadingProgress *0.5f;
            }
            else if (stage == Stage.WaitingForTransitionSceneToAllowExit)
            {
                return 1;
            }

            return 0;
        }

        public override string GetCurrentStageDebugString()
        {
            return stage.ToString();
        }
        public override bool IsFinished()
        {
            return stage == Stage.Finished;
        }
    }
}
