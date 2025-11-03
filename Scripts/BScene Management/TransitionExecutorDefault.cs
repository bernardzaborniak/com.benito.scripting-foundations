using Benito.ScriptingFoundations.Fades;
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

        BFade enterNextSceneFade;

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
            BFade exitCurrentSceneFade = null;
            if (exitCurrentSceneFadePrefab != null)
            {
                stage = Stage.PlayingExitCurrentSceneFade;
                exitCurrentSceneFade = BFade.CreateFade(exitCurrentSceneFadePrefab, sceneManagerTransform);
                exitCurrentSceneFade.RestartFade(BFade.FadeDirection.Forward);

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
                h1_OnFinishedLoadTargetScene?.Invoke();
                h2_OnFinishedLoadSceneLoadSaveAndInitialize?.Invoke();
            };

            sceneLoader.OnSwitchedToPreloadedScene += switchDoneHandler;
            sceneLoader.SwitchToPreloadedScene();

            yield return new WaitUntil(() => switchDone);
            sceneLoader.OnSwitchedToPreloadedScene -= switchDoneHandler;

            if (exitCurrentSceneFade)
                GameObject.Destroy(exitCurrentSceneFade.gameObject);


            // 5 Play enter next scene fade in
            enterNextSceneFade = null;
            if (enterNextSceneFadePrefab != null)
            {
                stage = Stage.PlayingEnterNextSceneFade;
                h3_OnFinishedStillPlayingLastFadeIn?.Invoke();
                enterNextSceneFade = BFade.CreateFade(enterNextSceneFadePrefab, sceneManagerTransform);
                enterNextSceneFade.RestartFade(BFade.FadeDirection.Backward);

                yield return new WaitUntil(() => enterNextSceneFade.HasFinished);
            }

            if (enterNextSceneFade)
                GameObject.Destroy(enterNextSceneFade.gameObject);

            stage = Stage.Finished;
            h4_OnFinished?.Invoke();
        }

        public override float GetProgress()
        {
            return (int)stage;
        }

        public override string GetProgressString()
        {
            return stage.ToString();
        }

        public override bool CanBeFinishedPrematurely()
        {
            return stage == Stage.PlayingEnterNextSceneFade;
        }

        public override void FinishUpPrematurely()
        {
            if (!CanBeFinishedPrematurely() || enterNextSceneFade == null)
            {
                Debug.Log($"[{this.GetType()}] Can't finish up prematurely");
                return;
            }

            enterNextSceneFade.FinishUp();
            stage = Stage.Finished;
            h4_OnFinished?.Invoke();
        }
    }
}