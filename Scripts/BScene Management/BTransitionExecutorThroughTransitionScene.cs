using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;

namespace Benito.ScriptingFoundations.BSceneManagement
{
    public class BTransitionExecutorThroughTransitionScene : BTransitionExecuter
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

        Transform sceneManagerTransform;
        BSceneLoader sceneLoader;

        // Dynamic Refs
        BTransitionSceneController currentTransitionSceneController;

        enum Stage
        {
            Idle,
            PlayingExitCurrentSceneFade,
            PlayingEnterTransitionSceneFade,
            WaitingForTargetSceneToPreload,
            WaitingForTransitionScenePlayerInteraction,
            PlayingExitTransitionSceneFade,
            PlayingEnterTargetSceneFade,
            Finished
        }

        Stage stage;

        public BTransitionExecutorThroughTransitionScene(string targetScene, string transitionScene, 
            Transform sceneManagerTransform, BSceneLoader sceneLoader,
            GameObject exitCurrentSceneFadePrefab, GameObject enterTransitionSceneFadePrefab,
            GameObject exitTransitionSceneFadePrefab, GameObject enterNextSceneFadePrefab)
        {
            stage = Stage.Idle;

            this.targetScene = targetScene;
            this.transitionScene = transitionScene;

            this.sceneLoader = sceneLoader;
            this.sceneManagerTransform = sceneManagerTransform;

            this.exitCurrentSceneFadePrefab = exitCurrentSceneFadePrefab;
            this.enterTransitionSceneFadePrefab = enterTransitionSceneFadePrefab;
            this.exitTransitionSceneFadePrefab = exitTransitionSceneFadePrefab;
            this.enterNextSceneFadePrefab = enterNextSceneFadePrefab;
        }

        public override void UpdateTransition()
        {

        }

        public override void StartTransition()
        {
            _1_StartExitCurrentSceneFade();

          
            // also do scene preload here somehow
        }

        /// <summary>
        /// Call this to allow exiting a transition scene.
        /// </summary>
        /*
         public void OnTransitionSceneAllowsContinuation()
        {
            if (stage == Stage.WaitingForTargetSceneToPreload)
            {
                _6_StartExitTransitionSceneFade();
            }
        }
        */

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

            if (sceneLoader.IsPreloadingComplete())
            {
                _3_OnPreloadingTransitionSceneComplete();
            }
            else
            {
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
            stage = Stage.WaitingForTargetSceneToPreload;

            if (sceneLoader.IsPreloadingComplete())
            {
                _7_OnTargetSceneFinishedPreloading();
            }
            else
            {
                sceneLoader.OnPreloadingSceneFinished += _7_OnTargetSceneFinishedPreloading;
            }
            
        }

        void _7_OnTargetSceneFinishedPreloading()
        {
            sceneLoader.OnPreloadingSceneFinished -= _7_OnTargetSceneFinishedPreloading;

            if (currentTransitionSceneController.TransitionWaitsForPlayerInteractionToFinish)
            {
                // We wait for the transition controller inside the scene to give us the GO to continue.
                stage = Stage.WaitingForTransitionScenePlayerInteraction;

                currentTransitionSceneController.OnPlayerTriggeredTransitionCompletion
                    += _8_OnPlayerInteractionTriggeredTransitionCompletion;
            }
            else
            {
                _9_StartExitTransitionSceneFade();
            }
        }

        void _8_OnPlayerInteractionTriggeredTransitionCompletion()
        {
            currentTransitionSceneController.OnPlayerTriggeredTransitionCompletion
                    -= _8_OnPlayerInteractionTriggeredTransitionCompletion;

            _9_StartExitTransitionSceneFade();
        }

        void _9_StartExitTransitionSceneFade()
        {
            Debug.Log("_9_StartExitTransitionSceneFade ");
            if (exitTransitionSceneFadePrefab != null)
            {
                exitTransitionSceneFade = CreateFade(exitTransitionSceneFadePrefab, sceneManagerTransform);
                exitTransitionSceneFade.OnFadeFinished += _10_OnExitTransitionSceneFadeFinished;
                exitTransitionSceneFade.StartFade();
                stage = Stage.PlayingExitTransitionSceneFade;
            }
            else
            {
                _10_OnExitTransitionSceneFadeFinished();
            }

        }

        void _10_OnExitTransitionSceneFadeFinished()
        {
            sceneLoader.OnSwitchedToPreloadedScene += _11_OnLoadingNextSceneComplete;
            sceneLoader.SwitchToPreloadedScene();
        }

        void _11_OnLoadingNextSceneComplete()
        {
            sceneLoader.OnSwitchedToPreloadedScene -= _11_OnLoadingNextSceneComplete;

            OnFinishedLoadingTargetScene?.Invoke();

            if (exitTransitionSceneFade)
                GameObject.Destroy(exitTransitionSceneFade.gameObject);

            _12_StartEnterNextSceneFade();
        }

        void _12_StartEnterNextSceneFade()
        {
            if (enterNextSceneFadePrefab != null)
            {
                enterNextSceneFade = CreateFade(enterNextSceneFadePrefab, sceneManagerTransform);
                enterNextSceneFade.OnFadeFinished += _13_OnEnterNextSceneFadeFinished;
                enterNextSceneFade.StartFade();
                stage = Stage.PlayingEnterTargetSceneFade;
            }
            else
            {

                _13_OnEnterNextSceneFadeFinished();
            }
        }

        void _13_OnEnterNextSceneFadeFinished()
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
