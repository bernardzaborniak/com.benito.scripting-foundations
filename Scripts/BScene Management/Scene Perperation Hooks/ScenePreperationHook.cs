
using Benito.ScriptingFoundations.SceneInitializers;
using System;
using UnityEngine;

namespace Benito.ScriptingFoundations.BSceneManagement.ScenePreperationHooks
{
    /// <summary>
    /// Manually assign this hook to the Scene Preperation Controller
    /// 
    /// This is used to generate procedural stuff in the level for example, stuff we wamt to hide before showing the level
    /// </summary>
    public abstract class ScenePreperationHook : MonoBehaviour
    {
        public bool PreperationHookFinished {  get; private set; }
        public Action OnPreperationHookFinished;

        public abstract void ExecutePreperationLogic();

        protected void FinishHook()
        {
            PreperationHookFinished = true;
            OnPreperationHookFinished?.Invoke();
        }

    }
}