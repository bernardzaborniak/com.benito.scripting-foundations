using Benito.ScriptingFoundations.Managers;
using Benito.ScriptingFoundations.SceneInitializers;
using System;
using UnityEngine;

namespace Benito.ScriptingFoundations.BSceneManagement
{
    /// <summary>
    /// Objects wishing to use the proper start after transition finished may use either once 
    /// of the 2 Action hooks provided. They should register during awake
    /// </summary>

    [DefaultExecutionOrder(-3)]
    [AddComponentMenu("Benitos Scripting Foundations/Transitions/Scene Load Hooks")]
    public class SceneLoadHooks : MonoBehaviour
    {
        public static SceneLoadHooks Instance;

        public Action OnTransitionFinishedInitializingTargetScene; // Initializers finished
        public Action OnTransitionFinished;

        SceneInitializersManager sceneInitializersManager;
        BSceneTransitionManager transitionManager;

        void Awake()
        {
            if (Instance != null)
            {
                DestroyImmediate(Instance.gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        private void Start()
        {
            // Register yourself to the transition manager if present, if not, just execute
            sceneInitializersManager = SceneInitializersManager.Instance;
            transitionManager = GlobalManagers.Get<BSceneTransitionManager>();

            if (transitionManager == null || transitionManager.ManagerState == BSceneTransitionManager.State.Idle)
            {
                if (sceneInitializersManager == null || sceneInitializersManager.IsFinished)
                {
                    OnTransitionFinishedInitializingTargetScene?.Invoke();
                    OnTransitionFinished?.Invoke();
                    return;
                }
                else
                {
                    sceneInitializersManager.OnFinished += () =>
                    {
                        OnTransitionFinishedInitializingTargetScene?.Invoke();
                        OnTransitionFinished?.Invoke();
                    };
                }
            }
            else
            {
                transitionManager.OnTransitionFinishedLoadingTargetScene += () => OnTransitionFinishedInitializingTargetScene?.Invoke();
                transitionManager.OnTransitionFinished += () => OnTransitionFinished?.Invoke();
            }
        }
    }
}
