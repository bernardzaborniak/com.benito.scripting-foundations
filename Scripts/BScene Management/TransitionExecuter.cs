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
        /// ::HasFinishedLoadingTargetLevel::: level was loaded, initializers and saves are ready,
        /// but transition is still playing.   
        /// </summary>
        public Action h1_OnFinishedLoadTargetScene;

        /// <summary>
        /// ::HasFinishedLoadingTargetLevel::: level was loaded, initializers and saves are ready,
        /// but transition is still playing.   
        /// </summary>
        public Action h2_OnFinishedLoadSceneLoadSaveAndInitialize;

        /// <summary>
        /// ::HasFinished::: level was loaded, now the last fade is still playing.   
        /// </summary>
        public Action h3_OnFinishedStillPlayingLastFadeIn;

        /// <summary>
        /// ::HasFinished::: level was loaded, now also the last fade in stopped playing. 
        /// </summary>
        public Action h4_OnFinished;

        public abstract void StartTransition();

        public abstract void UpdateTransition();

        /// <summary>
        /// Return progress between 0 and 1
        /// </summary>
        /// <returns></returns>
        public abstract float GetProgress();

        public abstract string GetProgressString();

        protected BSceneFade CreateFade(GameObject fadePrefab, Transform fadeParent)
        {
            BSceneFade fade = GameObject.Instantiate(fadePrefab, fadeParent).GetComponent<BSceneFade>();

            if (fade == null)
            {
                Debug.LogError("[BSceneTransition] fadePrefab needs to have a BSceneFade Component at its root");
            }
            return fade;
        }

        /// <summary>
        /// Mostly returns true if the transition is still active but only like the last enter scene fade is still playing
        /// </summary>
        public abstract bool CanBeFinishedPrematurely();

        /// <summary>
        /// Call this if you want to instantly finish teh transition if the last scene fade is still playing
        /// </summary>
        public abstract void FinishUpPrematurely();
    }
}
