using UnityEngine;

namespace Benito.ScriptingFoundations.BSceneManagement
{
    /// <summary>
    /// Example hook, that just enables objects when hooked
    /// </summary>
    [AddComponentMenu("Benitos Scripting Foundations/Transitions/OnTransitionFinishedHookGameObjectEnabler")]
    public class OnTransitionFinishedHookGameObjectEnabler : OnTransitionFinishedHook
    {
        [SerializeField] GameObject[] objectsToActivate;

        public override void StartAfterTransitionFinished()
        {
            for (int i = 0; i < objectsToActivate.Length; ++i)
            {
                objectsToActivate[i].SetActive(true);
            }
        }
    }
}