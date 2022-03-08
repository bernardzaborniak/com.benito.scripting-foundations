using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Benito.ScriptingFoundations.Managers;
using UnityEngine.SceneManagement;
using System;

namespace Benito.ScriptingFoundations.BSceneManagement
{
    public class BSceneManager : Singleton
    {
        string preloadedScene;
        
        BSceneTransition currentTransition;

        public Action OnPreloadingSceneFinished;

        public enum Status
        {
            Idle,
            PreloadingScene,
            IdleWithPreloadedScene,
            Transitioning
        }


        Status status;

        AsyncOperation preloadSceneOperation;
        public float PreloadingSceneProgress { get => preloadSceneOperation.progress; }
        public float NormalizedPreloadingSceneProgress { get => preloadSceneOperation.progress/0.9f; }


        public override void InitialiseSingleton()
        {
            status = Status.Idle;
        }

        public override void UpdateSingleton()
        {
            switch (status)
            {
                case Status.PreloadingScene:
                    {
                        if(preloadSceneOperation.progress >= 0.9f)
                        {
                            OnPreloadingSceneFinished?.Invoke();
                            status = Status.IdleWithPreloadedScene;
                        }
                            

                        break;
                    }
                case Status.Transitioning:
                    {
                        currentTransition.UpdateTransition();

                        if (currentTransition.Finished)
                        {
                            currentTransition = null;
                            status = Status.Idle;
                        }
                            

                        break;
                    }
            }

        }

        public void SwitchSceneInstantly(string newScene)
        {
            SceneManager.LoadScene(newScene);
        }

        public AsyncOperation PreloadScene(string sceneName)
        {
            status = Status.PreloadingScene;
            preloadSceneOperation =  SceneManager.LoadSceneAsync(sceneName);
            preloadSceneOperation.allowSceneActivation = false;
            preloadedScene = sceneName;
            
            return preloadSceneOperation;
        }

        // Needs to be called by the transition scene - every transition scene can do it differently?
        public void ExitTransitionScene()
        {
            BSceneTransitionWithTransitionScene transition = currentTransition as BSceneTransitionWithTransitionScene;
            if (transition != null)
            {
                Debug.Log("yep 2");

                transition.OnTransitionSceneAllowsContinuation();
            }
            else
            {
                Debug.LogError("ExitTransitionScene failed, as current transition is not a BSceneTransitionWithTransitionScene");
            }
            //SceneManager.LoadScene
        }

        public void SwitchToPreloadedScene(GameObject exitCurrentSceneFadePrefab = null, GameObject enterNextSceneFadePrefab = null)
        {
            currentTransition = new BSceneTransitionDefault(transform, preloadSceneOperation, exitCurrentSceneFadePrefab, enterNextSceneFadePrefab);
            currentTransition.StartTransition();
            status = Status.Transitioning;
        }

        public void SwitchThroughPreloadedTransitionScene(string targetScene,
            GameObject exitCurrentSceneFadePrefab = null, GameObject enterTransitionSceneFadePrefab = null,
            GameObject exitTransitiontSceneFadePrefab = null, GameObject enterNextSceneFadePrefab = null)
        {
            currentTransition = new BSceneTransitionWithTransitionScene(targetScene, transform, preloadSceneOperation,
                exitCurrentSceneFadePrefab, enterTransitionSceneFadePrefab,
                exitTransitiontSceneFadePrefab, enterNextSceneFadePrefab);
            currentTransition.StartTransition();
            status = Status.Transitioning;

        }
    }

}
