using UnityEngine;
using System;
using Benito.ScriptingFoundations.BSceneManagement;

namespace Benito.ScriptingFoundations.BSceneManagement.TransitionScene
{
    /// <summary>
    /// Require one of those components in every transition Scene
    /// 
    /// Transition Scenes dont have initializers like normal scenes, as this would be a problem when loading scenes additively.
    /// </summary>
    public class TransitionSceneController : MonoBehaviour
    {
        [Tooltip("If this is set to true, the transition system will wait until the player action is triggered before transitioning")]
        [field: SerializeField]
        public bool TransitionWaitsForPlayerInteractionToFinish { get; protected set; }

        public Action OnPlayerTriggeredTransitionCompletion;


        protected TransitionExecuter transitionExecutor;

        // Set up so the transition get get its progress from the eecutor
        public void SetUp(TransitionExecuter transitionExecutor)
        {
            this.transitionExecutor = transitionExecutor;
        }
    }
}