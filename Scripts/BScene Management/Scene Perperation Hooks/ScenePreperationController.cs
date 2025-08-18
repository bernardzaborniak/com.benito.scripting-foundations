using UnityEngine;
using Benito.ScriptingFoundations.Managers;
using Benito.ScriptingFoundations.SceneInitializers;
using System;
using System.Collections;

namespace Benito.ScriptingFoundations.BSceneManagement.ScenePreperationHooks
{
    /// <summary>
    /// Have thi in your scene, this will be taken into consideration if you need to preapre some stuff
    /// in your scene before showing it to the player, like generating a randomly generated level etc...
    /// </summary>
    public class ScenePreperationController : AbstractSceneInitializer
    {
        [SerializeField] ScenePreperationHook hooks;

        public bool InitializationFinished { get; private set; }
        public float InitializationProcess {  get; private set; } // just calculate be counting currentHook/allHooks

        public Action OnInitializationFinished;

        public override void Initialize()
        {
            // run the initialization here, but make it run async so it doesnt stutter, with budgeted operation ideally?

        }

        IEnumerator InitializationCoroutine()
        {
            // todod 

            yield return null;
        }
    }
}