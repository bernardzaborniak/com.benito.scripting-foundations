using Benito.ScriptingFoundations.Managers;
using UnityEngine;

namespace Benito.ScriptingFoundations.BSceneManagement
{
    /// <summary>
    /// This will be called when transitions initializers saving etc is finished.
    /// If scene is entered via play button inside editor and not via the transition, it will be called after the initializers have finished.
    /// 
    /// Dreive from this class for more complex behaviours.
    /// </summary>
    public abstract class OnTransitionFinishedHook : MonoBehaviour
    {
        public enum HookMode
        {
            OnTransitionFinishedLoadingTargetScene,
            OnTransitionFinished
        }
        [SerializeField] HookMode hookMode;

        BSceneTransitionManager transitionManager;

        public abstract void StartAfterTransitionFinished();

        private void OnEnable()
        {
            // Register yourself to the transition manager if present, if not, just execute
            transitionManager = GlobalManagers.Get<BSceneTransitionManager>();

            if (transitionManager == null)
            {
                StartAfterTransitionFinished();
                return;
            }

            // As there is no transition going on, it was probably finished already or we started from Unitys playmode
            if (transitionManager.ManagerState == BSceneTransitionManager.State.Idle)
            {
                StartAfterTransitionFinished();
                return;
            }

            if (hookMode == HookMode.OnTransitionFinishedLoadingTargetScene)
            {
                transitionManager.OnTransitionFinishedLoadingTargetScene += StartAfterTransitionFinished;
            }
            else if (hookMode == HookMode.OnTransitionFinished)
            {
                transitionManager.OnTransitionFinished += StartAfterTransitionFinished;
            }

        }

        private void OnDisable()
        {
            if (transitionManager != null)
            {
                if (hookMode == HookMode.OnTransitionFinishedLoadingTargetScene)
                {
                    transitionManager.OnTransitionFinishedLoadingTargetScene -= StartAfterTransitionFinished;
                }
                else if (hookMode == HookMode.OnTransitionFinished)
                {
                    transitionManager.OnTransitionFinished -= StartAfterTransitionFinished;
                }
            }
        }
    }
}
