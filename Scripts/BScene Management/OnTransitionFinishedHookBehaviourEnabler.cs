using UnityEngine;

namespace Benito.ScriptingFoundations.BSceneManagement
{
    /// <summary>
    /// Example hook, that just enables objects when hooked
    /// </summary>
    public class OnTransitionFinishedHookBehaviourEnabler : OnTransitionFinishedHook
    {
        [SerializeField] Behaviour[] behavioursToActivate;

        public override void StartAfterTransitionFinished()
        {
            for (int i = 0; i < behavioursToActivate.Length; ++i)
            {
                behavioursToActivate[i].enabled = true;
            }

        }
    }
}
