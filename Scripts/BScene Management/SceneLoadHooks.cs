using Benito.ScriptingFoundations.Managers;
using Benito.ScriptingFoundations.SceneInitializers;
using Benito.ScriptingFoundations.Utilities;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Benito.ScriptingFoundations.BSceneManagement
{
    /// <summary>
    /// Calls the hook methods for all objects implementing the ISceneLoadHooksMethods interface.
    /// </summary>

    [DefaultExecutionOrder(-3)]
    [AddComponentMenu("Benitos Scripting Foundations/Transitions/Scene Load Hooks")]
    public class SceneLoadHooks : MonoBehaviour
    {
        public static SceneLoadHooks Instance;

        List<ISceneLoadHooksListener> methodListeners;
        List<ISceneLoadHooksListenerOnly3> methodListenersOnly3;

        [SerializeField] bool debugHooks = false;

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

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            methodListeners = InterfaceUtilities.FindInterfacesInScene<ISceneLoadHooksListener>(gameObject.scene);
            methodListenersOnly3 = InterfaceUtilities.FindInterfacesInScene<ISceneLoadHooksListenerOnly3>(gameObject.scene);
            stopwatch.Stop();
            Debug.Log($"[Scene Load Hooks] Scanning Scene for objects implementing the ISceneLoadHooksListener interface took: {stopwatch.ElapsedMilliseconds} ms");
        }

        public void h1_OnFinishedLoadTargetScene()
        {
            if(debugHooks) Debug.Log("[Scene Load Hooks] h1_OnFinishedLoadTargetScene");
            for (int i = 0; i < methodListeners.Count; i++) 
            {
                methodListeners[i].Start_h1_OnFinishedLoadTargetScene();
            }

        }
        public void h2_OnFinishedLoadSceneLoadSaveAndInitialize()
        {
            if (debugHooks) Debug.Log("[Scene Load Hooks] h2_OnFinishedLoadSceneLoadSaveAndInitialize");
            for (int i = 0; i < methodListeners.Count; i++)
            {
                methodListeners[i].Start_h2_OnFinishedLoadSceneLoadSaveAndInitialize();
            }
        }
        public void h3_OnFinishedStillPlayingLastFadeIn()
        {
            if (debugHooks) Debug.Log("[Scene Load Hooks] h3_OnFinishedStillPlayingLastFadeIn");
            for (int i = 0; i < methodListeners.Count; i++)
            {
                methodListeners[i].Start_h3_OnFinishedStillPlayingLastFadeIn();
            }

            for (int i = 0;i < methodListenersOnly3.Count; i++)
            {
                methodListenersOnly3[i].Start_h3_OnFinishedStillPlayingLastFadeIn();
            }
        }
        public void h4_OnFinished()
        {
            if (debugHooks) Debug.Log("[Scene Load Hooks] h4_OnFinished");
            for (int i = 0; i < methodListeners.Count; i++)
            {
                methodListeners[i].Start_h4_OnFinished();
            }
        }


        /// <summary>
        /// If there is no transition to call the hooks, we have to call them ourselves.
        /// </summary>
        public void OnEnteredPlayModeViaEditor()
        {
            Debug.Log("[SceneLoadHooks] Initialized Scene Hooks via Editor Play Mode for this scene.");
            SceneInitializersManager sceneInitializersManager = SceneInitializersManager.Instance;

            h1_OnFinishedLoadTargetScene();

            if (sceneInitializersManager == null || sceneInitializersManager.IsFinished)
            {
                h2_OnFinishedLoadSceneLoadSaveAndInitialize();
                h3_OnFinishedStillPlayingLastFadeIn();
                h4_OnFinished();
                return;
            }
            else
            {
                sceneInitializersManager.OnFinished += () =>
                {
                    h2_OnFinishedLoadSceneLoadSaveAndInitialize();
                    h3_OnFinishedStillPlayingLastFadeIn();
                    h4_OnFinished();
                };
            }
        }
    }
}
