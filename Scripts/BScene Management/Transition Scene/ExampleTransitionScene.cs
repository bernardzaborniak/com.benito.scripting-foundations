using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Benito.ScriptingFoundations.BSceneManagement.TransitionScene
{
    public class ExampleTransitionScene : TransitionSceneController
    {
        [SerializeField] TextMeshProUGUI loadingProgressNumber;
        [SerializeField] TextMeshProUGUI loadingProgressString;
        [SerializeField] Image progressBarFillable;

        private void Start()
        {
            TransitionWaitsForPlayerInteractionToFinish = false;
        }

        void Update()
        {
            float progress = transitionExecutor.GetProgress();

            loadingProgressNumber.text = (progress * 100).ToString("F0");
            loadingProgressString.text = transitionExecutor.GetProgressString();

            progressBarFillable.fillAmount = progress;
        }
    }
}
