using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

namespace Benito.ScriptingFoundations.BSceneManagement
{
    public class BSceneTransitionDefault : BSceneTransition
    {
        BSceneFade exitCurrentSceneFade;
        BSceneFade enterNextSceneFade;
        AsyncOperation preloadSceneOperation;

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

        public BSceneTransitionDefault(string currentScene, string nextScene, AsyncOperation preloadSceneOperation, BSceneFade exitCurrentSceneFade = null, BSceneFade enterNextSceneFade = null)
        {
            this.currentScene = currentScene;
            this.nextScene = nextScene;
            this.preloadSceneOperation = preloadSceneOperation;
            this.exitCurrentSceneFade = exitCurrentSceneFade;
            this.enterNextSceneFade = enterNextSceneFade;

            stage = Stage.Idle;
        }

        public override void UpdateTransition()
        {
            if (stage == Stage.Idle)
            {
                if (exitCurrentSceneFade != null)
                {
                    exitCurrentSceneFade.OnTransitionFinished += OnExitCurrentSceneFadeFinished;
                    exitCurrentSceneFade.StartTransition();
                    stage = Stage.PlayingExitCurrentSceneFade;
                }
                else
                {
                    OnExitCurrentSceneFadeFinished();
                }
            }
        }

        public void OnExitCurrentSceneFadeFinished()
        {
            if (enterNextSceneFade != null)
            {
                enterNextSceneFade.OnTransitionFinished += OnEnterNextSceneFadeFinished;
                enterNextSceneFade.StartTransition();
                stage = Stage.PlayingEnterNextSceneFade;
            }
            else
            {
                OnEnterNextSceneFadeFinished();
            }

            preloadSceneOperation.allowSceneActivation = true;
        }
        public void OnEnterNextSceneFadeFinished()
        {
            stage = Stage.WaitingForNewSceneToFullyLoad;
            preloadSceneOperation.completed += OnNewSceneFullyLoads;
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
