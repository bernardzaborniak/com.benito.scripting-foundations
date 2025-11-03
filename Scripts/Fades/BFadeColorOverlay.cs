using Benito.ScriptingFoundations.NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace Benito.ScriptingFoundations.Fades
{
    public class BFadeColorOverlay : BFade
    {
        [SerializeField] float fadeDuration;
        float fadeSpeed; // internally just works with speed;


        [SerializeField] Image colorOverlayImage;

        [SerializeField] Color startColor;
        [SerializeField] Color endColor;

        Color currentStartColor;
        Color currentEndColor;

        public override void RestartFade(FadeDirection newDirection)
        {
            base.RestartFade(newDirection);

            fadeSpeed = 1.0f / fadeDuration;

            SetImageColorWithPogress();
        }

        void SetImageColorWithPogress()
        {
            colorOverlayImage.color = Color.Lerp(startColor, endColor, currentFadeValue);
        }

        public override void UpdateFade()
        {
            if (Mathf.Abs(targetTo - currentFadeValue) < 0.01f)
            {
                currentFadeValue = targetTo;
                SetImageColorWithPogress();
                FinishUp();
            }
            else
            {
                currentFadeValue = Mathf.MoveTowards(currentFadeValue, targetTo, fadeSpeed * Time.deltaTime);   
                SetImageColorWithPogress();
            }
        }
    }
}
