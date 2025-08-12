using Benito.ScriptingFoundations.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Benito.ScriptingFoundations.BSceneManagement
{
    /// <summary>
    /// Handles the seperate steps of playing fades to switch between 2 scenes.
    /// 
    /// Requires an already preloaded Scene. This change is there to allow the game to control how and when to start preloading
    /// as this transition type will be used more creatively
    /// </summary>
    public class BTransitionExecutorDefaultRequiresPreloadedScene : BTransitionExecuter
    {
        // Fades
        GameObject exitCurrentSceneFadePrefab;
        GameObject enterNextSceneFadePrefab;

        BSceneFade exitCurrentSceneFade;
        BSceneFade enterNextSceneFade;

        // Refs
        Transform sceneManagerTransform;
        BSceneLoader sceneLoader;


        enum Stage
        {
            Idle,
            PlayingExitCurrentSceneFade,
            PlayingEnterNextSceneFade,
            Finished
        }

        Stage stage;

        public BTransitionExecutorDefaultRequiresPreloadedScene(Transform sceneManagerTransform, BSceneLoader sceneLoader, GameObject exitCurrentSceneFadePrefab = null, GameObject enterNextSceneFadePrefab = null)
        {
            stage = Stage.Idle;

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
            sceneLoader.OnSwitchedToPreloadedScene += _3_OnLoadingNextSceneComplete;
            sceneLoader.SwitchToPreloadedScene();
        }

        void _3_OnLoadingNextSceneComplete()
        {
            sceneLoader.OnSwitchedToPreloadedScene -= _3_OnLoadingNextSceneComplete;

            if (exitCurrentSceneFade)
                GameObject.Destroy(exitCurrentSceneFade.gameObject);

            OnFinishedLoadingTargetScene?.Invoke();

            _4_StartEnterNextSceneFade();
        }
        
        void _4_StartEnterNextSceneFade()
        {
            if (enterNextSceneFadePrefab != null)
            {
                enterNextSceneFade = CreateFade(enterNextSceneFadePrefab, sceneManagerTransform);
                enterNextSceneFade.OnFadeFinished += _5_OnEnterNextSceneFadeFinished;
                enterNextSceneFade.StartFade();

                stage = Stage.PlayingEnterNextSceneFade;
            }
            else
            {
                _5_OnEnterNextSceneFadeFinished();
            }

        }

        void _5_OnEnterNextSceneFadeFinished()
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
