
using Benito.ScriptingFoundations.Managers;
using Benito.ScriptingFoundations.Saving;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Benito.ScriptingFoundations.BSceneManagement
{
    /// <summary>
    /// Handles the seperate steps of playing fades to switch between 2 scenes.
    /// Automatically goes through a transition scene that exits either automatically or on player input 
    /// Requires a trransition scene with a BTransitionSceneController object inside
    /// 
    /// This executor preloads the next scene himself
    /// </summary>
    public class BTransitionExecutorLoadSceneSaveThroughTransitionScene : BTransitionExecuter
    {
        // Fades
        GameObject exitCurrentSceneFadePrefab;
        GameObject enterTransitionSceneFadePrefab;
        GameObject exitTransitionSceneFadePrefab;
        GameObject enterNextSceneFadePrefab;

        // Refs
        string targetScene;
        string transitionScene;

        MonoBehaviour coroutineHost;
        Transform sceneManagerTransform;

        BSceneLoader sceneLoader;
        GlobalSavesManager globalSavesManager;
        string savegamePathInSavesFolder;

        // Dynamic Refs
        BTransitionSceneController currentTransitionSceneController;

        enum Stage
        {
            Idle,
            PlayingExitCurrentSceneFade,
            WaitingForTransitionSceneToPreload,
            PlayingEnterTransitionSceneFade,
            WaitingForTargetSceneToPreloadAndSaveToRead,
            WaitingForTargetSceneToFullyLoad,
            LoadingSaveFile,
            WaitingForTransitionScenePlayerInteraction,
            PlayingExitTransitionSceneFade,
            WaitingForTransitionSceneToUnload,
            PlayingEnterTargetSceneFade,
            Finished
        }

        Stage stage;

        public BTransitionExecutorLoadSceneSaveThroughTransitionScene(string targetScene, string transitionScene,
            string savegamePathInSavesFolder, GlobalSavesManager globalSavesManager,
            MonoBehaviour coroutineHost, Transform sceneManagerTransform, BSceneLoader sceneLoader,
            GameObject exitCurrentSceneFadePrefab, GameObject enterTransitionSceneFadePrefab,
            GameObject exitTransitionSceneFadePrefab, GameObject enterNextSceneFadePrefab)
        {
            stage = Stage.Idle;

            this.targetScene = targetScene;
            this.transitionScene = transitionScene;

            this.savegamePathInSavesFolder = savegamePathInSavesFolder;
            this.globalSavesManager = globalSavesManager;

            this.coroutineHost = coroutineHost;
            this.sceneManagerTransform = sceneManagerTransform;
            this.sceneLoader = sceneLoader;

            this.exitCurrentSceneFadePrefab = exitCurrentSceneFadePrefab;
            this.enterTransitionSceneFadePrefab = enterTransitionSceneFadePrefab;
            this.exitTransitionSceneFadePrefab = exitTransitionSceneFadePrefab;
            this.enterNextSceneFadePrefab = enterNextSceneFadePrefab;
        }

        public override void StartTransition()
        {
            coroutineHost.StartCoroutine(TransitionCoroutine());
        }

        public override void UpdateTransition()
        {

        }

        IEnumerator TransitionCoroutine()
        {
            // 1 Start Preloading 
            sceneLoader.PreloadScene(transitionScene);

            // 2 play ExitCurrentScene Fade
            BSceneFade exitCurrentSceneFade = null;
            if (exitCurrentSceneFadePrefab != null)
            {
                stage = Stage.PlayingExitCurrentSceneFade;
                exitCurrentSceneFade = CreateFade(exitCurrentSceneFadePrefab, sceneManagerTransform);
                exitCurrentSceneFade.StartFade();

                yield return new WaitUntil(() => exitCurrentSceneFade.HasFinished);
            }

            // 3 Wait for transition scene to finish preloading
            stage = Stage.WaitingForTransitionSceneToPreload;

            if (!sceneLoader.IsPreloadingComplete())
            {
                yield return new WaitUntil(() => sceneLoader.IsPreloadingComplete());
            }

            // 4 Switch to preloaded transition scene
            bool switchDone = false;
            Action switchDoneHandler = () =>
            {
                switchDone = true;
                // Set up the transition controller
                currentTransitionSceneController = GameObject.FindFirstObjectByType<BTransitionSceneController>();
                if (currentTransitionSceneController == null)
                {
                    Debug.LogError($"[{this.GetType()}] The transition scene you are using has no object of " +
                        $"Type {typeof(BTransitionSceneController)}. Make sure the transition scene has this object");
                }
                currentTransitionSceneController.SetUp(this);
            };

            sceneLoader.OnSwitchedToPreloadedScene += switchDoneHandler;
            sceneLoader.SwitchToPreloadedScene();
            yield return new WaitUntil(() => switchDone);
            sceneLoader.OnSwitchedToPreloadedScene -= switchDoneHandler;

            if (exitCurrentSceneFade)
                GameObject.Destroy(exitCurrentSceneFade.gameObject);


            // 5 Start Preloading target scene & loading save
            sceneLoader.PreloadScene(targetScene,LoadSceneMode.Additive);
            Task<SceneSavegame> readSceneSaveFileTask = globalSavesManager.ReadSceneSaveFileAsync(savegamePathInSavesFolder);

            // 6 Play enter transition scene fade
            if (enterTransitionSceneFadePrefab != null)
            {
                stage = Stage.PlayingEnterTransitionSceneFade;
                BSceneFade enterTransitionSceneFade = CreateFade(enterTransitionSceneFadePrefab, sceneManagerTransform);
                enterTransitionSceneFade.StartFade();

                yield return new WaitUntil(() => enterTransitionSceneFade.HasFinished);
                GameObject.Destroy(enterTransitionSceneFade.gameObject);
            }

            // 7 Wait for target Scene to preload & save to read
            stage = Stage.WaitingForTargetSceneToPreloadAndSaveToRead;

            yield return new WaitUntil(() => sceneLoader.IsPreloadingComplete());
            yield return new WaitUntil(() => readSceneSaveFileTask.IsCompleted);

            // 8 Enable Second Scene, but dont unload transition yet, it enables in the background?
            stage = Stage.WaitingForTargetSceneToFullyLoad;
            bool switchToTargetDone = false;
            Action switchToTargetDoneHandler = () =>
            {
                switchToTargetDone = true;
                UnityEngine.SceneManagement.SceneManager.SetActiveScene(SceneManager.GetSceneByName(targetScene));
            };
            sceneLoader.OnSwitchedToPreloadedScene += switchToTargetDoneHandler;
            sceneLoader.SwitchToPreloadedScene();
            yield return new WaitUntil(() => switchToTargetDone);
            sceneLoader.OnSwitchedToPreloadedScene -= switchToTargetDoneHandler;

            // 9 Load Scene Save
            stage = Stage.LoadingSaveFile;

            SceneSavegame savegame = readSceneSaveFileTask.Result;

            bool loadingFinished = false;
            Action loadingFinishedHandler = () => loadingFinished = true;

            globalSavesManager.OnLoadingSceneSaveFileCompleted += loadingFinishedHandler;
            globalSavesManager.LoadSceneSave(savegame);
            yield return new WaitUntil(() => loadingFinished);
            globalSavesManager.OnLoadingSceneSaveFileCompleted -= loadingFinishedHandler;

            // 8 Wait for player interaction to exit transition (if set)
            if (currentTransitionSceneController.TransitionWaitsForPlayerInteractionToFinish)
            {
                stage = Stage.WaitingForTransitionScenePlayerInteraction;

                bool playerGaveGo = false;
                Action playerInteractionHandler = () => playerGaveGo = true;

                // We wait for the transition controller inside the scene to give us the GO to continue.
                currentTransitionSceneController.OnPlayerTriggeredTransitionCompletion += playerInteractionHandler;
                yield return new WaitUntil(() => playerGaveGo);
                currentTransitionSceneController.OnPlayerTriggeredTransitionCompletion -= playerInteractionHandler;
            }

            // 9 play exit transition scene fade
            BSceneFade exitTransitionSceneFade = null;
            if (exitTransitionSceneFadePrefab != null)
            {
                stage = Stage.PlayingExitTransitionSceneFade;
                exitTransitionSceneFade = CreateFade(exitTransitionSceneFadePrefab, sceneManagerTransform);
                exitTransitionSceneFade.StartFade();
                yield return new WaitUntil(() => exitTransitionSceneFade.HasFinished);
            }
                
            // 10 Wait for transition scene to unload
            stage = Stage.WaitingForTransitionSceneToUnload;

            AsyncOperation unloadSceneOperation = SceneManager.UnloadSceneAsync(transitionScene);

            yield return new WaitUntil(() => unloadSceneOperation.isDone);

            if (exitTransitionSceneFade)
                GameObject.Destroy(exitTransitionSceneFade.gameObject);


            // 11 Play enter target scene fade
            OnFinishedLoadingTargetScene?.Invoke();

            if (enterNextSceneFadePrefab != null)
            {
                stage = Stage.PlayingEnterTargetSceneFade;

                BSceneFade enterNextSceneFade = CreateFade(enterNextSceneFadePrefab, sceneManagerTransform);
                enterNextSceneFade.StartFade();

                yield return new WaitUntil(() => enterNextSceneFade.HasFinished);
                GameObject.Destroy(enterNextSceneFade.gameObject);
            }

            stage = Stage.Finished;

            OnFinished?.Invoke();
        }

        public override float GetProgress()
        {
            if (stage == Stage.WaitingForTargetSceneToPreloadAndSaveToRead)
            {
                //return preloadSceneOperation.progress / 0.9f * 0.1f + (globalSavesManager.ReadSceneSaveFileProgress*0.2f);
            }
            else if (stage == Stage.LoadingSaveFile)
            {
                return 0.3f + globalSavesManager.ReadSceneLoadFileProgress * 0.5f;
            }
            else if (stage == Stage.WaitingForTransitionScenePlayerInteraction)
            {
                return 1;
            }

            return 0;
        }

        public override string GetCurrentStageDebugString()
        {
            return stage.ToString();
        }
    }
}