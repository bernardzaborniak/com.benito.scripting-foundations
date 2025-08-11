using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Benito.ScriptingFoundations.Managers;
using Benito.ScriptingFoundations.Saving;
using Benito.ScriptingFoundations.NaughtyAttributes;
using UnityEngine.SceneManagement;
using System;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

namespace Benito.ScriptingFoundations.BSceneManagement
{
    /// <summary>
    /// Takes care of asynchronous preloading and unloading of scenes
    /// 
    /// Use Methods of BSceneTransitionManager whenever possible
    /// </summary>
    public class BSceneLoader : SingletonManagerGlobal
    {
        // State
        public enum State
        {
            Idle,
            PreloadingScene,
            IdleWithPreloadedScene
        }

        [SerializeField][ReadOnly] State state;

        // Loading Progress
        [SerializeField][ReadOnly][Label("Preloaded Scene Name")] string preloadedScene;
        AsyncOperation preloadSceneOperation;

        public float PreloadingSceneProgress { get => preloadSceneOperation != null ? preloadSceneOperation.progress : -1; }
        public float NormalizedPreloadingSceneProgress { get => preloadSceneOperation != null ? preloadSceneOperation.progress / 0.9f : -1; }

        // Hooks
        public Action OnPreloadingSceneFinished;
        public Action OnSwitchedToPreloadedScene;

        public bool IsPreloadingComplete()
        {
            return state is State.IdleWithPreloadedScene;
        }

        public bool IsCurrentlyPreloading()
        {
            return state is not State.Idle;
        }


        public override void InitialiseManager()
        {
            ResetPreloadOperation();
        }

        void ResetPreloadOperation()
        {
            Debug.Log($" [{Time.frameCount}] ResetPreloadOperation called");


            state = State.Idle;
            preloadedScene = null;
            preloadSceneOperation = null;
        }

        public override void UpdateManager()
        {
            if (state == State.PreloadingScene)
            {
                // why 0.9? https://docs.unity3d.com/6000.1/Documentation/ScriptReference/AsyncOperation-progress.html
                // If you set allowSceneActivation to false, progress is halted at 0.9 until it is set to true. This is extremely useful for creating loading bars.

                if (preloadSceneOperation.progress >= 0.9f)
                {
                    state = State.IdleWithPreloadedScene;
                    OnPreloadingSceneFinished?.Invoke();
                }

            }
        }

        /// <summary>
        /// Switches scene with default unity functionality, might casue load stutter if the target scene is big.
        /// </summary>
        public void SwitchSceneInstantly(string newScene)
        {
            SceneManager.LoadScene(newScene);
        }

        public void SwitchToPreloadedScene()
        {
            if (preloadSceneOperation == null)
            {
                Debug.LogError("[BSceneLoader] You're trying to switch to a preloaded scene but there is none preloaded.");
                return;
            }

            preloadSceneOperation.allowSceneActivation = true;
            preloadSceneOperation.completed += (preloadSceneOperation) =>
            {
                ResetPreloadOperation(); // reset before invoking
                OnSwitchedToPreloadedScene?.Invoke();
            };
        }

        /// <summary>
        /// Preloads a scene asynchronously with SceneManager.LoadSceneAsync.
        /// </summary>
        public void PreloadScene(string sceneName)
        {
            if (state is State.PreloadingScene or State.IdleWithPreloadedScene)
            {
                Debug.LogError("[BSceneLoader] You're alreading preloading a scene. Only one can be preloaded");
                return;
            }

            state = State.PreloadingScene;
            preloadSceneOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            preloadSceneOperation.allowSceneActivation = false;
            preloadedScene = sceneName;
        }

      

        // UNLOADING a preloaded scene is not possible rn in Unity :(

    }
}
