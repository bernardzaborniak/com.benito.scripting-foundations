using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Benito.ScriptingFoundations.BSceneManagement
{
    public class BSceneTransitionWithTransitionScene : BSceneTransition
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

        // Other 
        string targetScene;
        AsyncOperation preloadSceneOperation;
        Transform sceneManagerTransform;

        enum Stage
        {
            Idle,
            PlayingExitCurrentSceneFade,
            PlayingEnterTransitionSceneFade,
            WaitingForTransitionScene,
            PlayingExitTransitionSceneFade,
            PlayingEnterNextSceneFade,
            Finished
        }

        Stage stage;

        public BSceneTransitionWithTransitionScene(string targetScene, Transform sceneManagerTransform, AsyncOperation preloadSceneOperation,
            GameObject exitCurrentSceneFadePrefab, GameObject enterTransitionSceneFadePrefab,
            GameObject exitTransitionSceneFadePrefab, GameObject enterNextSceneFadePrefab)
        {
            Finished = false;
            stage = Stage.Idle;

            this.targetScene = targetScene;
            this.sceneManagerTransform = sceneManagerTransform;
            this.preloadSceneOperation = preloadSceneOperation;

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
            StartExitCurrentSceneFade();
        }

        /// <summary>
        /// Call this to allow exiting a transition scene.
        /// </summary>
        public void OnTransitionSceneAllowsContinuation()
        {
            if (stage == Stage.WaitingForTransitionScene)
            {
                StartExitTransitionSceneFade();
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
        }

        void OnExitCurrentSceneFadeFinished()
        {
            preloadSceneOperation.allowSceneActivation = true;
            preloadSceneOperation.completed += OnLoadingTransitionSceneComplete;
        }

        void OnLoadingTransitionSceneComplete(AsyncOperation asyncOperation)
        {
            preloadSceneOperation.completed -= OnLoadingTransitionSceneComplete;

            if (exitCurrentSceneFade)
                GameObject.Destroy(exitCurrentSceneFade.gameObject);

            preloadSceneOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(targetScene);
            preloadSceneOperation.allowSceneActivation = false;

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
            stage = Stage.WaitingForTransitionScene;
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
            preloadSceneOperation.allowSceneActivation = true;
            preloadSceneOperation.completed += OnLoadingNextSceneComplete;
        }

        void OnLoadingNextSceneComplete(AsyncOperation asyncOperation)
        {
            preloadSceneOperation.completed -= OnLoadingNextSceneComplete;

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
            OnTransitionFinished?.Invoke();
            Finished = true;
        }

        public override float GetProgress()
        {
            if(stage == Stage.WaitingForTransitionScene)
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
