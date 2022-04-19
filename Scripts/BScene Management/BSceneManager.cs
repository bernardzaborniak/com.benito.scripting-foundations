using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Benito.ScriptingFoundations.Managers;
using UnityEngine.SceneManagement;
using System;

namespace Benito.ScriptingFoundations.BSceneManagement
{
    public class BSceneManager : SingletonManagerGlobal
    {
        string preloadedScene;
        
        BSceneTransition currentTransition;

        public Action OnPreloadingSceneFinished;
        public Action OnTransitionFinishes; 

        public enum Status
        {
            Idle,
            PreloadingScene,
            IdleWithPreloadedScene,
            Transitioning
        }

        Status status;

        AsyncOperation preloadSceneOperation;
        public float PreloadingSceneProgress { get => preloadSceneOperation != null? preloadSceneOperation.progress : -1; }
        public float NormalizedPreloadingSceneProgress { get => preloadSceneOperation != null ? preloadSceneOperation.progress/0.9f : -1; }
        public float TransitionProgress { get => currentTransition != null ? currentTransition.GetProgress(): -1; }


        public override void InitialiseManager()
        {
            status = Status.Idle;
        }

        public override void UpdateManager()
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

                        Debug.Log("2: transitioning current trans: " + currentTransition );

                        if (currentTransition is BSceneTransitionWithTransitionScene)
                        {
                            Debug.Log("2: trans stage string: " + (currentTransition as BSceneTransitionWithTransitionScene).GetCurrentStageDebugString());
                        }

                        if (currentTransition.IsFinished())
                        {
                            Debug.Log("2: manager, is finished");
                            OnTransitionFinishes?.Invoke();
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
            Debug.Log("2: preload scene called on manager");
            status = Status.PreloadingScene;
            preloadSceneOperation =  SceneManager.LoadSceneAsync(sceneName);
            preloadSceneOperation.allowSceneActivation = false;
            preloadedScene = sceneName;
            
            return preloadSceneOperation;
        }

        public void UnloadPreloadedScene()
        {
            //TODO
        }

        // Needs to be called by the transition scene - every transition scene can do it differently?
        public void ExitTransitionScene()
        {
            if (currentTransition is BSceneTransitionWithTransitionScene)
            {
                (currentTransition as BSceneTransitionWithTransitionScene).OnTransitionSceneAllowsContinuation();
            }
            else if (currentTransition is BSceneTransitionLoadSceneSave)
            {
                (currentTransition as BSceneTransitionLoadSceneSave).OnTransitionSceneAllowsContinuation();
            }
            else
            {
                Debug.LogError("ExitTransitionScene failed, as current transition is not a BSceneTransitionWithTransitionScene or BSceneTransitionLoadSceneSave");
            }
        }

        public void SwitchToPreloadedScene(GameObject exitCurrentSceneFadePrefab = null, GameObject enterNextSceneFadePrefab = null)
        {
            currentTransition = new BSceneTransitionDefault(transform, preloadSceneOperation, exitCurrentSceneFadePrefab, enterNextSceneFadePrefab);
            currentTransition.StartTransition();
            status = Status.Transitioning;

            Debug.Log("2: switched status to transitioning");
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
            Debug.Log("2: switched status to transitioning");

        }

        public void LoadSceneSaveThroughTransitionScene(string targetScene, string transitionSceneName, string savegamePathInSavesFolder,
            GameObject exitCurrentSceneFadePrefab = null, GameObject enterTransitionSceneFadePrefab = null,
            GameObject exitTransitiontSceneFadePrefab = null, GameObject enterNextSceneFadePrefab = null)
        {
            currentTransition = new BSceneTransitionLoadSceneSave(targetScene, transitionSceneName, savegamePathInSavesFolder, transform, preloadSceneOperation,
            exitCurrentSceneFadePrefab, enterTransitionSceneFadePrefab,
                exitTransitiontSceneFadePrefab, enterNextSceneFadePrefab);
            currentTransition.StartTransition();
            status = Status.Transitioning;
            Debug.Log("2: switched status to transitioning");

        }

        #region Inspector Debug methods
#if UNITY_EDITOR

        public string GetCurrentState()
        {
            return status.ToString();
        }

        public string GetCurrentTransitionName()
        {
            if(currentTransition != null)
            {
                return currentTransition.GetType().Name;
            }

            return "";
        }

        public string GetCurrentTransitionStage()
        {
            if (currentTransition != null)
            {
                return currentTransition.GetCurrentStageDebugString();
            }
            return "";

        }
#endif

        #endregion
    }

}
