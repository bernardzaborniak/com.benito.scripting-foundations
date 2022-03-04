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
        //bool hasPreloadedScene { get => }

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
                        break;
                    }
            }

        }

        public void SwitchSceneInstantly(string newScene)
        {
            SceneManager.LoadScene(newScene);
        }

        public Action PreloadScene(string sceneName)
        {
            status = Status.PreloadingScene;
            preloadSceneOperation =  SceneManager.LoadSceneAsync(sceneName);
            preloadSceneOperation.allowSceneActivation = false;
            preloadedScene = sceneName;
            
            return OnPreloadingSceneFinished;
        }

        // Needs to be called by the transition scene - every transition scene can do it differently?
        public void ExitTransitionScene()
        {
            //SceneManager.LoadScene
        }

        public void SwitchToPreloadedSceneAsync(BSceneFade exitCurrentSceneFade = null, BSceneFade enterNextSceneFade = null)
        {
            currentTransition = new BSceneTransitionDefault(SceneManager.GetActiveScene().name, preloadedScene, preloadSceneOperation, exitCurrentSceneFade, enterNextSceneFade);
            status = Status.Transitioning;

        }

        public void SwitchThroughPreloadedTransitionScene(string nextScene, string transitionScene, 
            BSceneFade exitCurrentSceneFade = null, BSceneFade enterTransitionSceneFade = null,
            BSceneFade exitTransitiontSceneFade = null, BSceneFade enterNextSceneFade = null)
        {
            currentTransition = new BSceneTransitionWithTransitionScene();


        }      
    }

}
