using Benito.ScriptingFoundations.Utilities;
using System;
using UnityEngine;

namespace Benito.ScriptingFoundations.Fades
{
    public abstract class BFade : MonoBehaviour
    {
        public Action OnFadeFinished;

        public bool destroyOnFinish = false;

        public enum FadeState
        {
            Initialized,
            Running,
            Stopped,
            Finished
        }
        protected FadeState fadeState = FadeState.Initialized;

        public bool HasFinished => fadeState == FadeState.Finished;

        public enum FadeDirection
        {
            Forward,
            Backward
        }
        protected float currentFadeValue;
        protected float targetFrom;
        protected float targetTo;
        public FadeDirection fadeDirection { get; protected set; }

        public virtual void RestartFade(FadeDirection newDirection)
        {
            fadeState = FadeState.Running;
            fadeDirection = newDirection;

            if (fadeDirection == FadeDirection.Forward)
            {
                currentFadeValue = 0;
            }
            else if (fadeDirection == FadeDirection.Backward)
            {
                currentFadeValue = 1;
            }

            HandleDirectionChange();
        }

        public virtual void ChangeFadeDirection(FadeDirection newDirection)
        {
            if (fadeState != FadeState.Running) 
            {
                Debug.Log("[Fade] Cant change Fade direction as it isnt running");
            }

            fadeDirection = newDirection;

            HandleDirectionChange();
        }

        void HandleDirectionChange()
        {
            if (fadeDirection == FadeDirection.Forward)
            {
                targetFrom = 0;
                targetTo = 1;
            }
            else if (fadeDirection == FadeDirection.Backward)
            {
                targetFrom = 1;
                targetTo = 0;
            }
        }

        public virtual void StopFade()
        {
            if (fadeState == FadeState.Running)
            {
                fadeState = FadeState.Stopped;
            }
            else
            {
                Debug.Log("[Fade] Cant stop Fade as it wasnt running");
            }
        }

        public virtual void ResumeFade()
        {
            if (fadeState == FadeState.Stopped)
            {
                fadeState = FadeState.Running;
            }
            else
            {
                Debug.Log("[Fade] Cant continue Fade as it wasnt stopped");
            }
        }

        public virtual void FinishUp()
        {
            if (fadeState == FadeState.Running)
            {
                fadeState = FadeState.Finished;
                OnFadeFinished?.Invoke();
                if (destroyOnFinish) Destroy(gameObject);
            }
            else
            {
                Debug.Log("[Fade] Cant finish Fade as it wasnt running");
            }
        }

        /// <summary>
        /// Every fade should implement this update method where it updates the fade based on current progress and target
        /// </summary>
        public abstract void UpdateFade();

        private void Update()
        {
            if(fadeState == FadeState.Running)
            {
                UpdateFade();
            }       
        }

        public static BFade CreateFade(GameObject fadePrefab, Transform fadeParent, bool destroyOnFinish = false)
        {
            BFade fade =  GameObjectUtilities.SpawnPrefabWithComponent<BFade>(fadePrefab, fadeParent);
            fade.destroyOnFinish = destroyOnFinish;

            return fade;
        }
    }
}
