
using Benito.ScriptingFoundations.BSceneManagement.TransitionScene;
using Benito.ScriptingFoundations.Fades;
using Benito.ScriptingFoundations.Managers;
using Benito.ScriptingFoundations.Saving;
using Benito.ScriptingFoundations.Saving.SceneSaves;
using Benito.ScriptingFoundations.SceneInitializers;
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
    public class TransitionExecutorLoadSceneSaveThroughTransitionScene : TransitionExecuter
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
        string savePathInSavesFolder;

        // Dynamic Refs
        TransitionSceneController currentTransitionSceneController;

        // Progress feedback for loading screens
        float progress;
        string progressString;

        enum Stage
        {
            Idle,
            PlayingExitCurrentSceneFade,
            WaitingForTransitionSceneToPreload,
            PlayingEnterTransitionSceneFade,
            WaitingForTargetSceneToPreloadAndSaveToRead,
            WaitingForTargetSceneToFullyLoad,
            WaitingForSceneInitializers,
            LoadingSaveFile,
            WaitingForTransitionScenePlayerInteraction,
            PlayingExitTransitionSceneFade,
            WaitingForTransitionSceneToUnload,
            PlayingEnterTargetSceneFade,
            Finished
        }

        Stage stage;

        BFade enterTargetSceneFade;

        public TransitionExecutorLoadSceneSaveThroughTransitionScene(string targetScene, string transitionScene,
            string savePathInSavesFolder, GlobalSavesManager globalSavesManager,
            MonoBehaviour coroutineHost, Transform sceneManagerTransform, BSceneLoader sceneLoader,
            GameObject exitCurrentSceneFadePrefab, GameObject enterTransitionSceneFadePrefab,
            GameObject exitTransitionSceneFadePrefab, GameObject enterNextSceneFadePrefab)
        {
            stage = Stage.Idle;

            this.targetScene = targetScene;
            this.transitionScene = transitionScene;

            this.savePathInSavesFolder = savePathInSavesFolder;
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
            BFade exitCurrentSceneFade = null;
            if (exitCurrentSceneFadePrefab != null)
            {
                stage = Stage.PlayingExitCurrentSceneFade;
                exitCurrentSceneFade = BFade.CreateFade(exitCurrentSceneFadePrefab, sceneManagerTransform);
                exitCurrentSceneFade.RestartFade(BFade.FadeDirection.Forward);

                yield return new WaitUntil(() => exitCurrentSceneFade.HasFinished);
            }

            // 3 Wait for transition scene to finish preloading
            stage = Stage.WaitingForTransitionSceneToPreload;
            progressString = "Loading transition";

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
                currentTransitionSceneController = GameObject.FindFirstObjectByType<TransitionSceneController>();
                if (currentTransitionSceneController == null)
                {
                    Debug.LogError($"[{this.GetType()}] The transition scene you are using has no object of " +
                        $"Type {typeof(TransitionSceneController)}. Make sure the transition scene has this object");
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
            Task<SceneSave> readSceneSaveFileTask = globalSavesManager.ReadSceneSaveFileAsync(savePathInSavesFolder);
            progressString = "Loading savefile";

            // 6 Play enter transition scene fade
            if (enterTransitionSceneFadePrefab != null)
            {
                stage = Stage.PlayingEnterTransitionSceneFade;
                BFade enterTransitionSceneFade = BFade.CreateFade(enterTransitionSceneFadePrefab, sceneManagerTransform);
                enterTransitionSceneFade.RestartFade(BFade.FadeDirection.Backward);

                yield return new WaitUntil(() => enterTransitionSceneFade.HasFinished);
                GameObject.Destroy(enterTransitionSceneFade.gameObject);
            }

            // 7 Wait for target Scene to preload & save to read
            stage = Stage.WaitingForTargetSceneToPreloadAndSaveToRead;

            if (!readSceneSaveFileTask.IsCompleted)
            {
                progress = 0.1f * globalSavesManager.ReadSceneSaveFileProgress;
                yield return null;
            }

            progressString = "Preloading target scene";

            if (!sceneLoader.IsPreloadingComplete())
            {
                progress = 0.1f + sceneLoader.NormalizedPreloadingSceneProgress * 0.4f;
                yield return null;
            }
            progress = 0.5f;

            // 8 Enable Second Scene, but dont unload transition yet, it enables in the background?
            stage = Stage.WaitingForTargetSceneToFullyLoad;
            bool switchToTargetDone = false;
            Action switchToTargetDoneHandler = () =>
            {
                switchToTargetDone = true;
                UnityEngine.SceneManagement.SceneManager.SetActiveScene(SceneManager.GetSceneByName(targetScene));
                h1_OnFinishedLoadTargetScene?.Invoke();
            };
            sceneLoader.OnSwitchedToPreloadedScene += switchToTargetDoneHandler;
            sceneLoader.SwitchToPreloadedScene();
            yield return new WaitUntil(() => switchToTargetDone);
            sceneLoader.OnSwitchedToPreloadedScene -= switchToTargetDoneHandler;

            // 9 Wait for initializers to finish in target scene
            stage = Stage.WaitingForSceneInitializers;
            SceneInitializersManager initializersManager = SceneInitializersManager.Instance;
            while (initializersManager!= null && !initializersManager.IsFinished)
            {
                progressString = $"Initialializing Scene: {initializersManager.ProgressString}";
                progress = 0.5f + (initializersManager.Progress * 0.3f);
                yield return null;
            }
            progress = 0.8f;

            // 10 Load Scene Save
            stage = Stage.LoadingSaveFile;
            progressString = $"Loading scene save";


            SceneSave sceneSave = readSceneSaveFileTask.Result;

            bool loadingFinished = false;
            Action loadingFinishedHandler = () => loadingFinished = true;

            globalSavesManager.OnLoadingSceneSaveFileCompleted += loadingFinishedHandler;
            globalSavesManager.LoadSceneSave(sceneSave);

            if (!loadingFinished)
            {
                progress = 0.8f * globalSavesManager.LoadSceneSaveFileProgress;

                yield return null;
            }

            progress = 1f;
            progressString = "";
            h2_OnFinishedLoadSceneLoadSaveAndInitialize?.Invoke();

            globalSavesManager.OnLoadingSceneSaveFileCompleted -= loadingFinishedHandler;

            // 11 Wait for player interaction to exit transition (if set)
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

            // 12 play exit transition scene fade
            BFade exitTransitionSceneFade = null;
            if (exitTransitionSceneFadePrefab != null)
            {
                stage = Stage.PlayingExitTransitionSceneFade;
                exitTransitionSceneFade = BFade.CreateFade(exitTransitionSceneFadePrefab, sceneManagerTransform);
                exitTransitionSceneFade.RestartFade(BFade.FadeDirection.Forward);
                yield return new WaitUntil(() => exitTransitionSceneFade.HasFinished);
            }
                
            // 13 Wait for transition scene to unload
            stage = Stage.WaitingForTransitionSceneToUnload;
            AsyncOperation unloadSceneOperation = SceneManager.UnloadSceneAsync(transitionScene);
            yield return new WaitUntil(() => unloadSceneOperation.isDone);

            if (exitTransitionSceneFade)
                GameObject.Destroy(exitTransitionSceneFade.gameObject);


            // 14 Play enter target scene fade
            h3_OnFinishedStillPlayingLastFadeIn?.Invoke();

            if (enterNextSceneFadePrefab != null)
            {
                stage = Stage.PlayingEnterTargetSceneFade;

                enterTargetSceneFade = BFade.CreateFade(enterNextSceneFadePrefab, sceneManagerTransform);
                enterTargetSceneFade.RestartFade(BFade.FadeDirection.Backward);

                yield return new WaitUntil(() => enterTargetSceneFade.HasFinished);
                GameObject.Destroy(enterTargetSceneFade.gameObject);
            }

            stage = Stage.Finished;

            h4_OnFinished?.Invoke();
        }

        public override float GetProgress()
        {
            return progress;
        }

        public override string GetProgressString()
        {
            return progressString;
        }

        public override bool CanBeFinishedPrematurely()
        {
            return stage == Stage.PlayingEnterTargetSceneFade;
        }

        public override void FinishUpPrematurely()
        {
            if (!CanBeFinishedPrematurely() || enterTargetSceneFade == null)
            {
                Debug.Log($"[{this.GetType()}] Can't finish up prematurely");
                return;
            }

            enterTargetSceneFade.FinishUp();
            stage = Stage.Finished;
            h4_OnFinished?.Invoke();
        }
    }
}