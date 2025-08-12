using UnityEngine;

namespace Benito.ScriptingFoundations.BSceneManagement
{
    /// <summary>
    /// Handles the seperate steps of playing fades to switch between 2 scenes.
    /// 
    /// This executor preloads the next scene himself
    /// </summary>
    public class BTransitionExecutorDefault : BTransitionExecuter
    {
        // Fades
        GameObject exitCurrentSceneFadePrefab;
        GameObject enterNextSceneFadePrefab;

        BSceneFade exitCurrentSceneFade;
        BSceneFade enterNextSceneFade;

        // Refs
        Transform sceneManagerTransform;
        BSceneLoader sceneLoader;
        string targetScene;

        enum Stage
        {
            Idle,
            PlayingExitCurrentSceneFade,
            WaitingForTargetSceneToPreload,
            PlayingEnterNextSceneFade,
            Finished
        }

        Stage stage;

        public BTransitionExecutorDefault(string targetScene,
           Transform sceneManagerTransform, BSceneLoader sceneLoader,
           GameObject exitCurrentSceneFadePrefab = null, GameObject enterNextSceneFadePrefab = null)
        {
            stage = Stage.Idle;

            this.targetScene = targetScene;
            this.sceneLoader = sceneLoader;
            this.sceneManagerTransform = sceneManagerTransform;
            this.exitCurrentSceneFadePrefab = exitCurrentSceneFadePrefab;
            this.enterNextSceneFadePrefab = enterNextSceneFadePrefab;
        }

        public override void StartTransition()
        {
            _1_StartExitCurrentSceneFade();
        }

        public override void UpdateTransition()
        {

        }


        void _1_StartExitCurrentSceneFade()
        {
            sceneLoader.PreloadScene(targetScene);

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
            if (sceneLoader.IsPreloadingComplete())
            {
                _3_OnNextScenePreloadingFinished();
            }
            else
            {
                sceneLoader.OnPreloadingSceneFinished += _3_OnNextScenePreloadingFinished;
            }   
        }

        void _3_OnNextScenePreloadingFinished()
        {
            sceneLoader.OnPreloadingSceneFinished -= _3_OnNextScenePreloadingFinished;

            sceneLoader.OnSwitchedToPreloadedScene += _4_OnLoadingNextSceneComplete;
            sceneLoader.SwitchToPreloadedScene();
        }

        void _4_OnLoadingNextSceneComplete()
        {
            sceneLoader.OnSwitchedToPreloadedScene -= _4_OnLoadingNextSceneComplete;

            if (exitCurrentSceneFade)
                GameObject.Destroy(exitCurrentSceneFade.gameObject);

            OnFinishedLoadingTargetScene?.Invoke();

            _5_StartEnterNextSceneFade();
        }

        void _5_StartEnterNextSceneFade()
        {
            if (enterNextSceneFadePrefab != null)
            {
                enterNextSceneFade = CreateFade(enterNextSceneFadePrefab, sceneManagerTransform);
                enterNextSceneFade.OnFadeFinished += _6_OnEnterNextSceneFadeFinished;
                enterNextSceneFade.StartFade();

                stage = Stage.PlayingEnterNextSceneFade;
            }
            else
            {
                _6_OnEnterNextSceneFadeFinished();
            }

        }

        void _6_OnEnterNextSceneFadeFinished()
        {
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