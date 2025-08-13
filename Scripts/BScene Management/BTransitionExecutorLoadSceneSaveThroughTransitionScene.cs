using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Benito.ScriptingFoundations.Saving;
using Benito.ScriptingFoundations.Managers;
//using UnityEngine.SceneManagement;
using UnitysSceneManager = UnityEngine.SceneManagement.SceneManager;
using System.Threading.Tasks;

namespace Benito.ScriptingFoundations.BSceneManagement
{
    public class BTransitionExecutorLoadSceneSaveThroughTransitionScene : BTransitionExecuter
    {
        // Fades
        GameObject exitCurrentSceneFadePrefab;
        GameObject enterTransitionSceneFadePrefab;
        GameObject exitTransitionSceneFadePrefab;
        GameObject enterNextSceneFadePrefab;

        BSceneFade exitCurrentSceneFade;
        BSceneFade enterTransitionSceneFade;
        BSceneFade exitTransitionSceneFade;
        BSceneFade enterNextSceneFade;

        // Refs
        string targetScene;
        string transitionScene;
        string savegamePathInSavesFolder;
        BSceneLoader sceneLoader;
        GlobalSavesManager globalSavesManager;
        Transform sceneManagerTransform;


        // Dynamic Refs
        BTransitionSceneController currentTransitionSceneController;


        // Other
        SceneSavegame savegame;
        // Async operations and Tasks
        //AsyncOperation preloadSceneOperation;
        //AsyncOperation unloadSceneOperation;
        Task<SceneSavegame> readSceneSaveFileTask;

        SaveableObjectsSceneManager saveSceneManager;




        enum Stage
        {
            Idle,
            PlayingExitCurrentSceneFade,
            WaitingForTransitionSceneToPreload,
            PlayingEnterTransitionSceneFade,
            WaitingForNextSceneToPreloadAndSaveToRead,
            WaitingForNextSceneToFullyLoad,
            LoadingSaveFile,
            WaitingForTransitionSceneToAllowExit,
            PlayingExitTransitionSceneFade,
            WaitingForTransitionSceneToUnload,
            PlayingEnterNextSceneFade,
            Finished
        }

        Stage stage;

        public BTransitionExecutorLoadSceneSaveThroughTransitionScene(string targetScene, string transitionScene, 
            string savegamePathInSavesFolder, GlobalSavesManager globalSavesManager,
            Transform sceneManagerTransform, BSceneLoader sceneLoader,
            GameObject exitCurrentSceneFadePrefab = null, GameObject enterTransitionSceneFadePrefab = null,
            GameObject exitTransitionSceneFadePrefab = null, GameObject enterNextSceneFadePrefab = null)
        {
            stage = Stage.Idle;

            this.targetScene = targetScene;
            this.transitionScene = transitionScene;
            this.savegamePathInSavesFolder = savegamePathInSavesFolder;
            this.sceneLoader = sceneLoader;
            this.globalSavesManager = globalSavesManager;
            this.sceneManagerTransform = sceneManagerTransform;         

            this.exitCurrentSceneFadePrefab = exitCurrentSceneFadePrefab;
            this.enterTransitionSceneFadePrefab = enterTransitionSceneFadePrefab;
            this.exitTransitionSceneFadePrefab = exitTransitionSceneFadePrefab;
            this.enterNextSceneFadePrefab = enterNextSceneFadePrefab;
        }

        public override void StartTransition()
        {
            _1_StartExitCurrentSceneFade();
        }

        public override void UpdateTransition()
        {
            /*
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
            */
        }

     
        void _1_StartExitCurrentSceneFade()
        {
            sceneLoader.PreloadScene(transitionScene);

            if (exitCurrentSceneFadePrefab != null)
            {
                exitCurrentSceneFade = CreateFade(exitCurrentSceneFadePrefab, sceneManagerTransform);
                exitCurrentSceneFade.OnFadeFinished += _2_OnExitCurrentSceneFadeFinished;
                exitCurrentSceneFade.StartFade();
                stage = Stage.PlayingExitCurrentSceneFade;
            }
            else
            {
                _2_OnExitCurrentSceneFadeFinished();
            }
        }

        void _2_OnExitCurrentSceneFadeFinished()
        {
            // If preloading of the transition scene has finished already, we jump straight into the next stage
            // we only assign the sceneLoader.OnPreloadingSceneFinished now to not have 2 hooks at the same time
            // as sceneLoader.OnPreloadingSceneFinished and exitCurrentSceneFade.OnFadeFinished could conflict with another

            stage = Stage.WaitingForTransitionSceneToPreload;

            if (sceneLoader.IsPreloadingComplete())
            {
                _3_OnPreloadingTransitionSceneComplete();
            }
            else
            {
                stage = Stage.WaitingForTransitionSceneToPreload;
                sceneLoader.OnPreloadingSceneFinished += _3_OnPreloadingTransitionSceneComplete;
            }
        }

        void _3_OnPreloadingTransitionSceneComplete()
        {
            sceneLoader.OnPreloadingSceneFinished -= _3_OnPreloadingTransitionSceneComplete;

            sceneLoader.OnSwitchedToPreloadedScene += _4_OnLoadingTransitionSceneComplete;
            sceneLoader.SwitchToPreloadedScene();
        }

        void _4_OnLoadingTransitionSceneComplete()
        {
            sceneLoader.OnSwitchedToPreloadedScene -= _4_OnLoadingTransitionSceneComplete;

            if (exitCurrentSceneFade)
                GameObject.Destroy(exitCurrentSceneFade.gameObject);



            // Get the controller which must be present in the transition scene.
            currentTransitionSceneController = GameObject.FindFirstObjectByType<BTransitionSceneController>();
            if (currentTransitionSceneController == null)
            {
                Debug.LogError($"[{this.GetType()}] The transition scene you are using has no object of " +
                    $"Type {typeof(BTransitionSceneController)}. Make sure the transition scene has this object");
            }
            currentTransitionSceneController.SetUp(this);

            sceneLoader.PreloadScene(targetScene);

            // TODO actually if we call it like this it wont run asynchronously?
            readSceneSaveFileTask = globalSavesManager.ReadSceneSaveFileAsync(savegamePathInSavesFolder);

            // try this instead?
            // This offloads the task to a background thread, but it won't block the UI thread
            Task.Run(async () =>
            {
                var result = await globalSavesManager.ReadSceneSaveFileAsync("savefile.bsave");
                // Do something with the result here
            });

            // or maybe its wrong, alternatively maybe just check it inside update like i did before

            _5_StartEnterTransitionSceneFade();
        }

        void _5_StartEnterTransitionSceneFade()
        {
            if (enterTransitionSceneFadePrefab != null)
            {
                enterTransitionSceneFade = CreateFade(enterTransitionSceneFadePrefab, sceneManagerTransform);
                enterTransitionSceneFade.OnFadeFinished += _6_OnEnterTransitionSceneFadeFinished;
                enterTransitionSceneFade.StartFade();
                stage = Stage.PlayingEnterTransitionSceneFade;
            }
            else
            {
                _6_OnEnterTransitionSceneFadeFinished();
            }
        }

        void _6_OnEnterTransitionSceneFadeFinished()
        {
            if (enterTransitionSceneFade)
                GameObject.Destroy(enterTransitionSceneFade.gameObject);

            stage = Stage.WaitingForNextSceneToPreloadAndSaveToRead;

            // TODO continue implementing here 2025.08.12
            // maybe actually leave this one check inside update
        }

        void OnNextSceneFinishedPreloadingAndSavefileFinishedReading()
        {
            stage = Stage.WaitingForNextSceneToFullyLoad;
            //preloadSceneOperation.allowSceneActivation = true;
            //preloadSceneOperation.completed += OnLoadingNextSceneComplete;
        }

        void OnLoadingNextSceneComplete(AsyncOperation asyncOperation)
        {
            //preloadSceneOperation.completed -= OnLoadingNextSceneComplete;
            UnitysSceneManager.SetActiveScene(UnitysSceneManager.GetSceneByName(targetScene));
            globalSavesManager = null;
            StartLoadingSavegame();
        }

        void StartLoadingSavegame()
        {
            stage = Stage.LoadingSaveFile;
            savegame = readSceneSaveFileTask.Result;

            saveSceneManager = LocalSceneManagers.Get<SaveableObjectsSceneManager>();
            saveSceneManager.OnLoadingFinished += OnLoadingSavegameFinished;
            //saveSceneManager.LoadFromSaveData(savegame.SavedObjects);
            // to be moved to global saves manager
        }

        void OnLoadingSavegameFinished()
        {
            saveSceneManager.OnLoadingFinished -= OnLoadingSavegameFinished;

            stage = Stage.WaitingForTransitionSceneToAllowExit;
            saveSceneManager = null;
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

        void StartExitTransitionSceneFade()
        {
            if (exitTransitionSceneFadePrefab != null)
            {
                exitTransitionSceneFade = CreateFade(exitTransitionSceneFadePrefab, sceneManagerTransform);
                exitTransitionSceneFade.OnFadeFinished += OnExitTransitionSceneFadeFinished;
                exitTransitionSceneFade.StartFade();
                stage = Stage.PlayingExitTransitionSceneFade;
            }
            else
            {
                OnExitTransitionSceneFadeFinished();
            }
        }

        void OnExitTransitionSceneFadeFinished()
        {
            if (exitTransitionSceneFade)
                GameObject.Destroy(exitTransitionSceneFade.gameObject);

            stage = Stage.WaitingForTransitionSceneToUnload;
            //unloadSceneOperation = UnitysSceneManager.UnloadSceneAsync(transitionScene); // todo move this into BSceneLoader
            //unloadSceneOperation.completed += OnUnloadTransitionSceneCompleted;
        }

        void OnUnloadTransitionSceneCompleted(AsyncOperation operation)
        {
             //unloadSceneOperation.completed -= OnUnloadTransitionSceneCompleted;
             //unloadSceneOperation = null;

            if (exitTransitionSceneFade)
                GameObject.Destroy(exitTransitionSceneFade.gameObject);

            StartEnterNextSceneFade();
        }
       
        void StartEnterNextSceneFade()
        {
            if (enterNextSceneFadePrefab != null)
            {
                enterNextSceneFade = CreateFade(enterNextSceneFadePrefab, sceneManagerTransform);
                enterNextSceneFade.OnFadeFinished += OnEnterNextSceneFadeFinished;
                enterNextSceneFade.StartFade();
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
                //return preloadSceneOperation.progress / 0.9f * 0.1f + (globalSavesManager.ReadSceneSaveFileProgress*0.2f);
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
        /*public override bool HasFinished()
        {
            return stage == Stage.Finished || stage == Stage.PlayingEnterNextSceneFade;
        }*/
    }
}
