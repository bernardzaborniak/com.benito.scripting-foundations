using UnityEngine;
using System;
using Benito.ScriptingFoundations.BSceneManagement;

namespace Benito.ScriptingFoundations.BSceneManagement
{
    /// <summary>
    /// Require one of those components in every transition Scene
    /// </summary>
    public class BTransitionSceneController : MonoBehaviour
    {
        [Tooltip("If this is set to true, the transition system will wait until the player action is triggered before transitioning")]
        [field: SerializeField]
        public bool TransitionWaitsForPlayerInteractionToFinish { get; private set; }

        public Action OnPlayerTriggeredTransitionCompletion;


        protected BTransitionExecuter transitionExecutor;

        // Set up so the transition get get its progress from the eecutor
        public void SetUp(BTransitionExecuter transitionExecutor)
        {
            this.transitionExecutor = transitionExecutor;
        }
    }
}