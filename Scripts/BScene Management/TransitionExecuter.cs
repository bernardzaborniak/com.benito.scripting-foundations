using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Benito.ScriptingFoundations.BSceneManagement
{
    /// <summary>
    /// Classes deriving from this class take complete care of scene transitions.
    /// </summary>
    public abstract class TransitionExecuter
    {
        /// <summary>
        /// ::HasFinishedLoadingTargetLevel::: level was loaded, now the last fade is still playing.   
        /// ::HasFinished::: target level is loaded and graphical part of transition has also finished.   
        /// </summary>
        public Action OnFinished;

        /// <summary>
        /// ::HasFinishedLoadingTargetLevel::: level was loaded, now the last fade is still playing.   
        /// ::HasFinished::: target level is loaded and graphical part of transition has also finished.   
        /// </summary>
        public Action OnFinishedLoadingTargetScene;

        public abstract void StartTransition();

        public abstract void UpdateTransition();

        /// <summary>
        /// Return progress between 0 and 1
        /// </summary>
        /// <returns></returns>
        public abstract float GetProgress();

        public abstract string GetCurrentStageDebugString();

        protected BSceneFade CreateFade(GameObject fadePrefab, Transform fadeParent)
        {
            BSceneFade fade = GameObject.Instantiate(fadePrefab, fadeParent).GetComponent<BSceneFade>();

            if (fade == null)
            {
                Debug.LogError("[BSceneTransition] fadePrefab needs to have a BSceneFade Component at its root");
            }
            return fade;
        }
    }
}
