using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Benito.ScriptingFoundations.BSceneManagement
{
    public class BSceneFadeColorOverlay : BSceneFade
    {
        // Maybe change it up to use curves etc later on

        [SerializeField] Image colorOverlayImage;

        [SerializeField] Color startColor;
        [SerializeField] Color endColor;
        [SerializeField] float transitionDuration;

        float transitionStartTime;
        float nextTransitionEndTime;
        bool playTransition;

        public override void StartTransition()
        {
            playTransition = true;
            transitionStartTime = Time.time;
            nextTransitionEndTime = Time.time + transitionDuration;
            colorOverlayImage.color = startColor;
        }


        void Update()
        {
            if (playTransition)
            {
                if(Time.time> nextTransitionEndTime)
                {
                    playTransition = false;
                    colorOverlayImage.color = endColor;
                }
                else
                {
                    float progress = Mathf.Lerp(0, 1, (Time.time-transitionStartTime)/ (nextTransitionEndTime - transitionStartTime));
                    colorOverlayImage.color = Color.Lerp(startColor, endColor, progress);
                }
            }
        }
    }
}
