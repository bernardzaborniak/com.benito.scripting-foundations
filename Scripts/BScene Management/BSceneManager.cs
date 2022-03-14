using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Benito.ScriptingFoundations.Managers;
using Benito.ScriptingFoundations.Saving;
using Benito.ScriptingFoundations.InspectorAttributes;
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

                        if (currentTransition.Finished)
                        {
                            currentTransition = null;
                            OnTransitionFinishes?.Invoke();
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

        public void LoadSceneSaveThroughTransitionScene(string targetScene, string transitionSceneName, string savegamePath,
            GameObject exitCurrentSceneFadePrefab = null, GameObject enterTransitionSceneFadePrefab = null,
            GameObject exitTransitiontSceneFadePrefab = null, GameObject enterNextSceneFadePrefab = null)
        {
            currentTransition = new BSceneTransitionLoadSceneSave(targetScene, transitionSceneName, savegamePath, transform, preloadSceneOperation,
            exitCurrentSceneFadePrefab, enterTransitionSceneFadePrefab,
                exitTransitiontSceneFadePrefab, enterNextSceneFadePrefab);
            currentTransition.StartTransition();
            status = Status.Transitioning;
        }

        public void DisableAllObjectExceptSceneManagers(string targetSceneName)
        {
            LocalSceneManagers sceneManagers = null;


            foreach (var gameObject in UnityEngine.SceneManagement.SceneManager.GetSceneByName(targetSceneName).GetRootGameObjects())
            { 
                gameObject.SetActive(false);

                if(sceneManagers == null)
                {
                    sceneManagers = gameObject.GetComponentInChildren<LocalSceneManagers>();
                    if(sceneManagers != null)
                    {
                        sceneManagers.gameObject.SetActive(true);
                        if(sceneManagers.gameObject.transform.parent != null)
                        {
                            Debug.LogError("DisableAllObjectExceptSceneManagers - LocalSceneManagers Should not have a parent!!!");
                        }
                    }
                }
            }

            
        }

        public void EnableAllObjects(string targetSceneName)
        {
            foreach (var gameObject in UnityEngine.SceneManagement.SceneManager.GetSceneByName(targetSceneName).GetRootGameObjects())
            {
                gameObject.SetActive(true);
            }
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
