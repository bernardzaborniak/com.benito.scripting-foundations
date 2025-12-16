using Benito.ScriptingFoundations.NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace Benito.ScriptingFoundations.Fades
{
    public class BFadeColorOverlay : BFade
    {
        public float fadeDuration;
        float fadeSpeed; // internally just works with speed;


        [SerializeField] Image colorOverlayImage;

        public Color startColor;
        public Color endColor;

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
            // Temp

            Debug.Log($"fade status {fadeState} fade fadeDuration: {currentFadeValue} FadeProgress: {FadeProgress}"); // TODO check if this works

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
