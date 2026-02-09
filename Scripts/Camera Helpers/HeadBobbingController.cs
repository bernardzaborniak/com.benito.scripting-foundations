using UnityEngine;


namespace Benito.ScriptingFoundations.CameraHelpers
{
    // Made in collab with brother chat
    public class HeadBobbingController : MonoBehaviour
    {
        [Header("References")]
        public Transform transformToMove;

        [Header("Smoothing")]
        public float transitionSpeed = 4f;

        [Header("Movement")]
        public float stopThreshold = 0.1f;

        [Tooltip("Set by movement speed")]
        [SerializeField] float frequencyMultiplier = 1f;

        [Tooltip("Set by settings of bobbing strength")]
        [SerializeField] float amplitudeMultiplier = 1f;


        Vector3 originalPos;
        Quaternion originalRot;

        HeadBobbingSettings targetSettings;

        Vector3 currentPosAmount;
        Vector3 currentRotAmount;

        float currentPosFrequency;
        float currentRotFrequency;

        float posPhase;
        float rotPhase;


        void Awake()
        {
            if (transformToMove == null) transformToMove = transform;

            originalPos = transformToMove.localPosition;
            originalRot = transformToMove.localRotation;

            targetSettings = new HeadBobbingSettings();
        }

        void Update()
        {
            // Frame-rate independent smoothing
            float lerpFactor = 1f - Mathf.Exp(-transitionSpeed * Time.deltaTime);

            currentPosAmount = Vector3.Lerp(currentPosAmount, targetSettings.positionAmount, lerpFactor);
            currentRotAmount = Vector3.Lerp(currentRotAmount, targetSettings.rotationAmount, lerpFactor);

            currentPosFrequency = Mathf.Lerp(currentPosFrequency, targetSettings.positionFrequency, lerpFactor);
            currentRotFrequency = Mathf.Lerp(currentRotFrequency, targetSettings.rotationFrequency, lerpFactor);

            // Integrate phase
            if (frequencyMultiplier > stopThreshold)
            {
                posPhase += Time.deltaTime * currentPosFrequency * frequencyMultiplier;
                rotPhase += Time.deltaTime * currentRotFrequency * frequencyMultiplier;
            }

            // Position bobbing
            Vector3 posOffset = new Vector3(
                Mathf.Sin(posPhase) * currentPosAmount.x,
                Mathf.Sin(posPhase) * currentPosAmount.y,
                Mathf.Sin(posPhase) * currentPosAmount.z
            ) * amplitudeMultiplier;

            // Rotation bobbing
            Vector3 rotOffset = new Vector3(
                Mathf.Sin(rotPhase) * currentRotAmount.x,
                Mathf.Sin(rotPhase) * currentRotAmount.y,
                Mathf.Sin(rotPhase) * currentRotAmount.z
            ) * amplitudeMultiplier;

            transformToMove.localPosition = originalPos + posOffset;
            transformToMove.localRotation = originalRot * Quaternion.Euler(rotOffset);
        }

        public void SetSettings(HeadBobbingSettings newSettings) { targetSettings = newSettings; }

        /// <summary>
        /// Scales bobbing frequency (not amplitude) based on normalized movement speed (0–1)
        /// </summary>
        public void SetFrequencyByMovementSpeed(float normalizedSpeed) { frequencyMultiplier = normalizedSpeed; }

        /// <summary>
        /// Scales bobbing amplitude (not amplitude) based on a setting (0–3?)
        /// </summary>
        public void SetAmplitudeBySettings(float multiplier) { amplitudeMultiplier = multiplier; }

    }

}