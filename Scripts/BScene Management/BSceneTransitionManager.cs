using System;
using UnityEngine;
using Benito.ScriptingFoundations.Managers;

namespace Benito.ScriptingFoundations.BSceneManagement
{
    /// <summary>
    /// Takes care of switching between scenes with transitions, 
    /// transitions are custom object that allow for various handling of those scene switches.
    /// 
    /// Use BSceneLoader Preload scene first, before transitioning into it
    /// 
    /// Requires BSCeneLoader inside the same hierarchy
    /// 
    /// Creates new BSceneTransition objects for each transition
    /// </summary>
    public class BSceneTransitionManager : SingletonManagerGlobal
    {
        // Refs
        BSceneLoader sceneLoader;

        // Transitions 
        BTransitionExecuter currentTransition;
        public float TransitionProgress { get => currentTransition != null ? currentTransition.GetProgress() : -1; }

        public Action OnTransitionFinishedLoadingTargetScene;
        public Action OnTransitionFinished;


        public override void InitialiseManager()
        {
            sceneLoader = GlobalManagers.Get<BSceneLoader>();
        }

        public override void UpdateManager()
        {
            if (currentTransition != null)
            {
                currentTransition.UpdateTransition();
            }
        }

        /// <summary>
        /// Needs to be called by the transition scene - every transition scene can do it differently?
        /// </summary>
        /*public void ExitTransitionScene()
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
        }*/

       

        /// <summary>
        /// Transitions into the currently preloaded scene with optional fades if you leave them empty.
        /// Requires an already preloaded scene
        /// </summary>
        public void TransitionIntoPreloadedScene(GameObject exitCurrentSceneFadePrefab = null, GameObject enterNextSceneFadePrefab = null)
        {
            if (!sceneLoader.IsPreloadingComplete())
            {
                Debug.LogError("[BSceneTransitionManager] Can't transition into preloaded scene as loading hasn't finished yet or there i no preloaded scene.");
                return;
            }
            if (currentTransition != null) 
            {
                Debug.LogError("[BSceneTransitionManager] Can't start a new transition as the previous one hasnt finished yet");
                return;
            }

            currentTransition = new BTransitionExecutorDefaultRequiresPreloadedScene(transform, sceneLoader, exitCurrentSceneFadePrefab, enterNextSceneFadePrefab);

            AddTransitionFinishingHooks();
            currentTransition.StartTransition();  // Calling start is enough, the transition will handle all further setps asynchronously on its own

        }
        
        /// <summary>
        /// Transitions into target scene with fades. Preloads target scene automatically.
        /// </summary>
        public void TransitionDefault(string targetScene, GameObject exitCurrentSceneFadePrefab = null, GameObject enterNextSceneFadePrefab = null)
        {
            if (sceneLoader.IsCurrentlyPreloading())
            {
                Debug.LogError("[BSceneTransitionManager] Can't start Transition as there already is another preloaded scene.");
                return;
            }
            if (currentTransition != null)
            {
                Debug.LogError("[BSceneTransitionManager] Can't start a new transition as the previous one hasnt finished yet");
                return;
            }

            currentTransition = new BTransitionExecutorDefault(targetScene, transform, sceneLoader,
               exitCurrentSceneFadePrefab, enterNextSceneFadePrefab);

            AddTransitionFinishingHooks();
            currentTransition.StartTransition();
        }


        /// <summary>
        /// Transitions into target scene with a specified transition. Preloads all scenes automatically.
        /// </summary>
        public void TransitionThroughTransitionScene(string targetScene, string transitionScene,
           GameObject exitCurrentSceneFadePrefab = null, GameObject enterTransitionSceneFadePrefab = null,
           GameObject exitTransitiontSceneFadePrefab = null, GameObject enterNextSceneFadePrefab = null)
       {
            if (sceneLoader.IsCurrentlyPreloading())
            {
                Debug.LogError("[BSceneTransitionManager] Can't start Transition as there already is another preloaded scene.");
                return;
            }
            if (currentTransition != null)
            {
                Debug.LogError("[BSceneTransitionManager] Can't start a new transition as the previous one hasnt finished yet");
                return;
            }

            currentTransition = new BTransitionExecutorThroughTransitionScene(targetScene, transitionScene, transform, sceneLoader,
               exitCurrentSceneFadePrefab, enterTransitionSceneFadePrefab,
               exitTransitiontSceneFadePrefab, enterNextSceneFadePrefab);

            AddTransitionFinishingHooks();
            currentTransition.StartTransition();
       }


        /// <summary>
        /// Transitions into target scene with a specified transition. Preloads all scenes automatically. Loads the save while being inside the transition scene.
        /// </summary>
        /*
       public void LoadSceneSaveThroughTransitionScene(string targetScene, string transitionScene, string savegamePathInSavesFolder,
           GameObject exitCurrentSceneFadePrefab = null, GameObject enterTransitionSceneFadePrefab = null,
           GameObject exitTransitiontSceneFadePrefab = null, GameObject enterNextSceneFadePrefab = null)
       {
            if (sceneLoader.IsCurrentlyPreloading())
            {
                Debug.LogError("[BSceneTransitionManager] Can't start Transition as there already is another preloaded scene.");
                return;
            }

           currentTransition = new BSceneTransitionLoadSceneSave(targetScene, transitionSceneName, savegamePathInSavesFolder, transform, preloadSceneOperation,
           exitCurrentSceneFadePrefab, enterTransitionSceneFadePrefab,
               exitTransitiontSceneFadePrefab, enterNextSceneFadePrefab);
           currentTransition.StartTransition();
           //state = State.TransitioningToTargetScene;
       }
       */

        void AddTransitionFinishingHooks()
        {
            // we dont need to reassign the hooks, as every transition object will be destroyed after use

            currentTransition.OnFinishedLoadingTargetScene += () =>
            {
                OnTransitionFinishedLoadingTargetScene?.Invoke();
            };

            currentTransition.OnFinished += () =>
            {
                OnTransitionFinished?.Invoke();
                currentTransition = null;
            };
        }

        #region Inspector Debug methods
#if UNITY_EDITOR

        public string GetCurrentTransitionName()
        {
            if (currentTransition != null)
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
