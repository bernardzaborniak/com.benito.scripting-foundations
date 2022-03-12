using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Benito.ScriptingFoundations.Saving;
using Benito.ScriptingFoundations.Managers;

namespace Benito.ScriptingFoundations.BSceneManagement
{

    /// <summary>
    /// The Savefile is being loaded after exit transition scene Fade finishes and before enter nextSceneFade Starts
    /// </summary>
    public class BSceneTransitionLoadSceneSave : BSceneTransition
    {
        string transitionScene;
        string targetScene;
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

        Transform sceneManagerTransform;

        enum Stage
        {
            Idle,
            PlayingExitCurrentSceneFade,
            WaitingForTransitionSceneToPreload,
            PlayingEnterTransitionSceneFade,
            WaitingForTransitionSceneToAllowExit,
            PlayingExitTransitionSceneFade,
            LoadingSaveFile,
            PlayingEnterNextSceneFade,
            Finished
        }

        Stage stage;

        public BSceneTransitionLoadSceneSave(SceneSavegame savegame, string transitionScene, Transform sceneManagerTransform, AsyncOperation preloadSceneOperation,
            GameObject exitCurrentSceneFadePrefab = null, GameObject enterTransitionSceneFadePrefab = null,
            GameObject exitTransitionSceneFadePrefab = null, GameObject enterNextSceneFadePrefab = null)
        {
            Finished = false;
            stage = Stage.Idle;

            this.savegame = savegame;
            this.transitionScene = transitionScene;
            targetScene = savegame.GetSceneName();
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
                    Debug.Log("OnExitCurrentSceneFadeFinishedAndTransitionSceneIsPreloaded");
                    OnExitCurrentSceneFadeFinishedAndTransitionSceneIsPreloaded();
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
            Debug.Log("OnExitCurrentSceneFadeFinished");
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

            preloadSceneOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(targetScene);
            preloadSceneOperation.allowSceneActivation = false;
            Debug.Log("preloadSceneOperation: " + preloadSceneOperation.progress);


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
            Debug.Log("OnEnterTransitionSceneFadeFinished");
            Debug.Log("preloadSceneOperation: " + preloadSceneOperation.progress);
            if (enterTransitionSceneFade)
                GameObject.Destroy(enterTransitionSceneFade.gameObject);
            stage = Stage.WaitingForTransitionSceneToAllowExit;
        }

        void StartExitTransitionSceneFade()
        {
            Debug.Log("StartExitTransitionSceneFade");
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
            Debug.Log("OnLoadingNextSceneComplete");


            preloadSceneOperation.completed -= OnLoadingNextSceneComplete;

            StartLoadingSavegame();
        }

        void StartLoadingSavegame()
        {
            Debug.Log("StartLoadingSavegame");

            SaveableObjectsSceneManager saveManager = LocalSceneManagers.Get<SaveableObjectsSceneManager>();
            saveManager.OnLoadingFinished += OnLoadingSavegameFinished;
            saveManager.LoadFromSaveData(savegame.GetSavedObjectsFromSave());
            stage = Stage.LoadingSaveFile;
        }

        void OnLoadingSavegameFinished()
        {
            Debug.Log("OnLoadingSavegameFinished");

            LocalSceneManagers.Get<SaveableObjectsSceneManager>().OnLoadingFinished -= OnLoadingSavegameFinished;

            if (exitTransitionSceneFade)
                GameObject.Destroy(exitTransitionSceneFade.gameObject);


            StartEnterNextSceneFade();
        }

        void StartEnterNextSceneFade()
        {
            Debug.Log("StartEnterNextSceneFade:enterNextSceneFadePrefab " + enterNextSceneFadePrefab);

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
            Debug.Log("OnEnterNextSceneFadeFinished ");


            if (enterNextSceneFade)
                GameObject.Destroy(exitTransitionSceneFade.gameObject);

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
    }
}
