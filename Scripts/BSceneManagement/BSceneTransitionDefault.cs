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

        enum Stage
        {
            Idle,
            PlayingExitCurrentSceneFade,
            PlayingEnterNextSceneFade,
            Finished
        }

        Stage stage;


        public BSceneTransitionDefault(Transform sceneManagerTransform, AsyncOperation preloadSceneOperation, GameObject exitCurrentSceneFadePrefab = null, GameObject enterNextSceneFadePrefab = null)
        {
            this.sceneManagerTransform = sceneManagerTransform;
            this.preloadSceneOperation = preloadSceneOperation;
            this.exitCurrentSceneFadePrefab = exitCurrentSceneFadePrefab;
            this.enterNextSceneFadePrefab = enterNextSceneFadePrefab;

            stage = Stage.Idle;
        }

        public override void StartTransition()
        {
             StartExitCurrentSceneFade();
        }

        public override void UpdateTransition()
        {
        }

        void StartExitCurrentSceneFade()
        {
            if (exitCurrentSceneFadePrefab)
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
            preloadSceneOperation.completed += OnLoadingNextSceneComplete;
        }

        void OnLoadingNextSceneComplete(AsyncOperation asyncOperation)
        {
            preloadSceneOperation.completed -= OnLoadingNextSceneComplete;

            if (exitCurrentSceneFade)
                GameObject.Destroy(exitCurrentSceneFade.gameObject);

            StartEnterNextSceneFade();
        }

        void StartEnterNextSceneFade()
        {
           if(enterNextSceneFadePrefab != null)
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
        }

      
    }
}
