using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Benito.ScriptingFoundations.Saving;
using Benito.ScriptingFoundations.Managers;
using System.Threading.Tasks;

namespace Benito.ScriptingFoundations.BSceneManagement
{

    /// <summary>
    /// The Savefile is being loaded after exit transition scene Fade finishes and before enter nextSceneFade Starts
    /// </summary>
    public class BSceneTransitionLoadSceneSave : BSceneTransition
    {
        string targetScene;
        string transitionScene;
        string savegamePath;
        SceneSavegame savegame;

        GameObject exitCurrentSceneFadePrefab;
        GameObject enterTransitionSceneFadePrefab;
        GameObject exitTransitionSceneFadePrefab;
        GameObject enterNextSceneFadePrefab;

        BSceneFade exitCurrentSceneFade;
        BSceneFade enterTransitionSceneFade;
        BSceneFade exitTransitionSceneFade;
        BSceneFade enterNextSceneFade;

        AsyncOperation preloadSceneOperation;
        AsyncOperation unloadSceneOperation;
        Task<SceneSavegame> readSceneSaveFileTask;

        Transform sceneManagerTransform;

        enum Stage
        {
            Idle,
            PlayingExitCurrentSceneFade,
            WaitingForTransitionSceneToPreload,
            PlayingEnterTransitionSceneFade,
            WaitingForNextSceneToPreloadAndSaveToLoad,
            LoadingSaveFile,
            WaitingForTransitionSceneToAllowExit,
            PlayingExitTransitionSceneFade,
            PlayingEnterNextSceneFade,
            Finished
        }

        Stage stage;

        public BSceneTransitionLoadSceneSave(string targetScene, string transitionScene, string savegamePath, Transform sceneManagerTransform, AsyncOperation preloadSceneOperation,
            GameObject exitCurrentSceneFadePrefab = null, GameObject enterTransitionSceneFadePrefab = null,
            GameObject exitTransitionSceneFadePrefab = null, GameObject enterNextSceneFadePrefab = null)
        {
            Finished = false;
            stage = Stage.Idle;

            this.targetScene = targetScene;
            this.transitionScene = transitionScene;
            this.savegamePath = savegamePath;

            this.sceneManagerTransform = sceneManagerTransform;
            this.preloadSceneOperation = preloadSceneOperation;
         

            this.exitCurrentSceneFadePrefab = exitCurrentSceneFadePrefab;
            this.enterTransitionSceneFadePrefab = enterTransitionSceneFadePrefab;
            this.exitTransitionSceneFadePrefab = exitTransitionSceneFadePrefab;
            this.enterNextSceneFadePrefab = enterNextSceneFadePrefab;
        }

        public override void StartTransition()
        {
            StartExitCurrentSceneFade();
        }

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
            else if(stage == Stage.WaitingForNextSceneToPreloadAndSaveToLoad)
            {
                if (preloadSceneOperation.progress >= 0.9f && readSceneSaveFileTask.IsCompleted)
                {
                    OnNextSceneFinishedPreloadingAndSavefileFinishedReading();
                }
            }
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

            readSceneSaveFileTask = GlobalManagers.Get<GlobalSavesManager>().ReadSceneSaveFileAsync(savegamePath);

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

            //if(preloadSceneOperation.progress < 0.9f)
            stage = Stage.WaitingForNextSceneToPreloadAndSaveToLoad;
        }

        void OnNextSceneFinishedPreloadingAndSavefileFinishedReading()
        {
            preloadSceneOperation.allowSceneActivation = true;
            preloadSceneOperation.completed += OnLoadingNextSceneComplete;
            //TODO hide the new scene somehow

        }

        void OnLoadingNextSceneComplete(AsyncOperation asyncOperation)
        {
            preloadSceneOperation.completed -= OnLoadingNextSceneComplete;
            //preloadSceneOperation = null;
            StartLoadingSavegame();
        }

        void StartLoadingSavegame()
        {
            stage = Stage.LoadingSaveFile;

            savegame = readSceneSaveFileTask.Result;

            SaveableObjectsSceneManager saveManager = LocalSceneManagers.Get<SaveableObjectsSceneManager>();
            saveManager.OnLoadingFinished += OnLoadingSavegameFinished;
            saveManager.LoadFromSaveData(savegame.GetSavedObjectsFromSave());

        }

        void OnLoadingSavegameFinished()
        {
            LocalSceneManagers.Get<SaveableObjectsSceneManager>().OnLoadingFinished -= OnLoadingSavegameFinished;

            if (exitTransitionSceneFade)
                GameObject.Destroy(exitTransitionSceneFade.gameObject);


            stage = Stage.WaitingForTransitionSceneToAllowExit;
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
            //preloadSceneOperation.allowSceneActivation = true;
            //preloadSceneOperation.completed += OnLoadingNextSceneComplete;

            unloadSceneOperation = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(transitionScene);
            unloadSceneOperation.completed += OnUnloadTransitionSceneCompleted;
        }

        void OnUnloadTransitionSceneCompleted(AsyncOperation operation)
        {
            unloadSceneOperation.completed -= OnUnloadTransitionSceneCompleted;
            unloadSceneOperation = null;
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
            OnTransitionFinished?.Invoke();
            Finished = true;
        }

        public override float GetProgress()
        {
            if (stage == Stage.WaitingForTransitionSceneToAllowExit)
            {
                return preloadSceneOperation.progress / 0.9f;
            }

            return 0;
        }

        public override string GetCurrentStageDebugString()
        {
            return stage.ToString();
        }
    }
}
