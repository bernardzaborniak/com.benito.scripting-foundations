using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

namespace Benito.ScriptingFoundations.BSceneManagement
{
    public class BSceneTransitionDefault : BSceneTransition
    {
        GameObject exitCurrentSceneFadePrefab;
        GameObject enterNextSceneFadePrefab;
        BSceneFade exitCurrentSceneFade;
        BSceneFade enterNextSceneFade;
        AsyncOperation preloadSceneOperation;

        Transform sceneManagerTransform;

        string nextScene;
        string currentScene;

        enum Stage
        {
            Idle,
            PlayingExitCurrentSceneFade,
            PlayingEnterNextSceneFade,
            WaitingForNewSceneToFullyLoad,
            Finished
        }

        Stage stage;

        public BSceneTransitionDefault(Transform sceneManagerTransform, string currentScene, string nextScene, AsyncOperation preloadSceneOperation, GameObject exitCurrentSceneFadePrefab = null, GameObject enterNextSceneFadePrefab = null)
        {
            this.sceneManagerTransform = sceneManagerTransform;
            this.currentScene = currentScene;
            this.nextScene = nextScene;
            this.preloadSceneOperation = preloadSceneOperation;
            this.exitCurrentSceneFadePrefab = exitCurrentSceneFadePrefab;
            this.enterNextSceneFadePrefab = enterNextSceneFadePrefab;
            Debug.Log("enterNextSceneFadePrefab 1: " + enterNextSceneFadePrefab);

            stage = Stage.Idle;

            Debug.Log("cerated BSceneTransitionDefault");
        }

        public override void UpdateTransition()
        {
            if (stage == Stage.Idle)
            {
                if (exitCurrentSceneFadePrefab != null)
                {
                    Debug.Log("Start Exit current Scene Fade");

                    StartExitCurrentSceneFade();
                }
                else
                {
                    OnExitCurrentSceneFadeFinished();
                }
            }
        }

        void StartExitCurrentSceneFade()
        {
            Debug.Log("enterNextSceneFadePrefab 2: " + enterNextSceneFadePrefab);

            exitCurrentSceneFade = GameObject.Instantiate(exitCurrentSceneFadePrefab, sceneManagerTransform).GetComponent<BSceneFade>();

            if (exitCurrentSceneFade == null)
            {
                Debug.LogError("exitCurrentSceneFadePrefab needs to have a BSceneFade Component at its root");
                return;
            }
            

            exitCurrentSceneFade.OnTransitionFinished += OnExitCurrentSceneFadeFinished;
            exitCurrentSceneFade.StartTransition();
            stage = Stage.PlayingExitCurrentSceneFade;
        }

        public void OnExitCurrentSceneFadeFinished()
        {
            if (exitCurrentSceneFade)
            {
                GameObject.Destroy(exitCurrentSceneFade.gameObject);
            }

            if (enterNextSceneFadePrefab != null)
            {
                Debug.Log("Start OnEnterNextSceneFade");

                StartEnterNextSceneFade();        
            }
            else
            {
                OnEnterNextSceneFadeFinished();
            }

            preloadSceneOperation.allowSceneActivation = true;
        }

        void StartEnterNextSceneFade()
        {
            Debug.Log("enterNextSceneFadePrefab 3: " + enterNextSceneFadePrefab);

            enterNextSceneFade = GameObject.Instantiate(enterNextSceneFadePrefab, sceneManagerTransform).GetComponent<BSceneFade>();

            if (enterNextSceneFade == null)
            {
                Debug.LogError("enterNextSceneFadePrefab needs to have a BSceneFade Component at its root");
                return;
            }

            enterNextSceneFade.OnTransitionFinished += OnEnterNextSceneFadeFinished;
            enterNextSceneFade.StartTransition();
            stage = Stage.PlayingEnterNextSceneFade;
        }

        public void OnEnterNextSceneFadeFinished()
        {
            if (enterNextSceneFade)
            {
                GameObject.Destroy(enterNextSceneFade.gameObject);
            }


            stage = Stage.WaitingForNewSceneToFullyLoad;
            preloadSceneOperation.completed += OnNewSceneFullyLoads;

            Debug.Log("OnEnterNextSceneFadeFinished");

        }

        public void OnNewSceneFullyLoads(AsyncOperation operation)
        {
            // Debug.Log("preloadSceneOperation.allowSceneActivation = true;");
            //Debug.Log("stage: " + stage);
            stage = Stage.Finished;

            //SceneManager.SetActiveScene(SceneManager.GetSceneByName(nextScene));
            //SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName(currentScene));
        }

    }
}
