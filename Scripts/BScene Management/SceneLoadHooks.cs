using Benito.ScriptingFoundations.Managers;
using Benito.ScriptingFoundations.SceneInitializers;
using System;
using UnityEngine;

namespace Benito.ScriptingFoundations.BSceneManagement
{
    /// <summary>
    /// Objects wishing to use the proper start after transition finished may use those hookes
    /// They should register to the singleton instance during awake.
    /// </summary>

    [DefaultExecutionOrder(-3)]
    [AddComponentMenu("Benitos Scripting Foundations/Transitions/Scene Load Hooks")]
    public class SceneLoadHooks : MonoBehaviour
    {
        public static SceneLoadHooks Instance;

        public Action h1_OnFinishedLoadTargetScene; 
        public Action h2_OnFinishedLoadSceneLoadSaveAndInitialize;
        public Action h3_OnFinishedStillPlayingLastFadeIn;
        public Action h4_OnFinished;

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

        /// <summary>
        /// If there is no transition to call the hooks, we have to call them ourselves.
        /// </summary>
        public void OnEnteredPlayModeViaEditor()
        {
            Debug.Log("[SceneLoadHooks] Initialized Scene Hooks via Editor Play Mode for this scene.");
            sceneInitializersManager = SceneInitializersManager.Instance;

            h1_OnFinishedLoadTargetScene?.Invoke();

            if (sceneInitializersManager == null || sceneInitializersManager.IsFinished)
            {
                h2_OnFinishedLoadSceneLoadSaveAndInitialize?.Invoke();
                h3_OnFinishedStillPlayingLastFadeIn?.Invoke();
                h4_OnFinished?.Invoke();
                return;
            }
            else
            {
                sceneInitializersManager.OnFinished += () =>
                {
                    h2_OnFinishedLoadSceneLoadSaveAndInitialize?.Invoke();
                    h3_OnFinishedStillPlayingLastFadeIn?.Invoke();
                    h4_OnFinished?.Invoke();
                };
            }
        }
    }
}
