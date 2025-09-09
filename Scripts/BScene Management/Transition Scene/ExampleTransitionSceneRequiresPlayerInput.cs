using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Benito.ScriptingFoundations.BSceneManagement.TransitionScene
{
    public class ExampleTransitionSceneRequiresPlayerInput : TransitionSceneController
    {
        [SerializeField] TextMeshProUGUI loadingProgressNumber;
        [SerializeField] TextMeshProUGUI loadingProgressString;
        [SerializeField] TextMeshProUGUI loadingFinishedTriggerPlayerInputText;
        [SerializeField] Image progressBarFillable;

        private void Start()
        {
            TransitionWaitsForPlayerInteractionToFinish = true;
            loadingFinishedTriggerPlayerInputText.gameObject.SetActive(false);
        }

        void Update()
        {
            float progress = transitionExecutor.GetProgress();

            loadingProgressNumber.text = (progress * 100).ToString("F0");
            loadingProgressString.text = transitionExecutor.GetProgressString();

            progressBarFillable.fillAmount = progress;


            if (progress == 1)
            {
                loadingFinishedTriggerPlayerInputText.gameObject.SetActive(true);

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    OnPlayerTriggeredTransitionCompletion?.Invoke();
                }
            }            
        }
    }
}
