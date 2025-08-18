using System;
using System.Collections;
using UnityEngine;

namespace Benito.ScriptingFoundations.BSceneManagement
{
    /// <summary>
    /// Handles the seperate steps of playing fades to switch between 2 scenes.
    /// 
    /// This executor preloads the next scene himself
    /// </summary>
    public class TransitionExecutorDefault : TransitionExecuter
    {
        // Fades
        GameObject exitCurrentSceneFadePrefab;
        GameObject enterNextSceneFadePrefab;

        // Refs
        string targetScene;
        MonoBehaviour coroutineHost;
        Transform sceneManagerTransform;
        BSceneLoader sceneLoader;

        enum Stage
        {
            Idle,
            PlayingExitCurrentSceneFade,
            WaitingForTargetSceneToPreload,
            PlayingEnterNextSceneFade,
            Finished
        }

        Stage stage;

        public TransitionExecutorDefault(string targetScene,
           MonoBehaviour coroutineHost, Transform sceneManagerTransform, BSceneLoader sceneLoader,
           GameObject exitCurrentSceneFadePrefab = null, GameObject enterNextSceneFadePrefab = null)
        {
            stage = Stage.Idle;

            this.targetScene = targetScene;
            this.coroutineHost = coroutineHost;
            this.sceneLoader = sceneLoader;
            this.sceneManagerTransform = sceneManagerTransform;
            this.exitCurrentSceneFadePrefab = exitCurrentSceneFadePrefab;
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
            sceneLoader.PreloadScene(targetScene);

            // 2 play ExitCurrentScene Fade
            BSceneFade exitCurrentSceneFade = null;
            if (exitCurrentSceneFadePrefab != null)
            {
                stage = Stage.PlayingExitCurrentSceneFade;
                exitCurrentSceneFade = CreateFade(exitCurrentSceneFadePrefab, sceneManagerTransform);
                exitCurrentSceneFade.StartFade();

                yield return new WaitUntil(() => exitCurrentSceneFade.HasFinished);
            }

            // 3 Wait for target scene to finish preloading
            stage = Stage.WaitingForTargetSceneToPreload;

            if (!sceneLoader.IsPreloadingComplete())
            {
                yield return new WaitUntil(() => sceneLoader.IsPreloadingComplete());
            }

            // 4 Switch to preloaded scene
            bool switchDone = false;
            Action switchDoneHandler = () =>
            {
                switchDone = true;
                OnFinishedLoadingTargetScene?.Invoke();
            };

            sceneLoader.OnSwitchedToPreloadedScene += switchDoneHandler;
            sceneLoader.SwitchToPreloadedScene();

            yield return new WaitUntil(() => switchDone);
            sceneLoader.OnSwitchedToPreloadedScene -= switchDoneHandler;

            if (exitCurrentSceneFade)
                GameObject.Destroy(exitCurrentSceneFade.gameObject);


            // 5 Play enter next scene fade
            BSceneFade enterNextSceneFade = null;
            if (enterNextSceneFadePrefab != null)
            {
                stage = Stage.PlayingEnterNextSceneFade;
                enterNextSceneFade = CreateFade(enterNextSceneFadePrefab, sceneManagerTransform);
                enterNextSceneFade.StartFade();

                yield return new WaitUntil(() => enterNextSceneFade.HasFinished);
            }

            if (enterNextSceneFade)
                GameObject.Destroy(enterNextSceneFade.gameObject);

            stage = Stage.Finished;
            OnFinished?.Invoke();
        }

        public override float GetProgress()
        {
            return (int)stage;
        }

        public override string GetCurrentStageDebugString()
        {
            return stage.ToString();
        }
    }
}