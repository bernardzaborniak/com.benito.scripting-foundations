using Benito.ScriptingFoundations.Utilities.Guns;
using UnityEngine;

namespace Benito.ScriptingFoundations.CameraHelpers
{
    public class PanTiltAndRecoilCameraController : MonoBehaviour
    {
        public bool autoUpdate;

        [Header("Camera Movement")]
        public float sensitivity = 1f;
        public bool invertY = false;

        [Tooltip("looking up")]
        public float minTiltAngle = -30f;
        [Tooltip("looking down")]
        public float maxTiltAngle = 60f;

        // Current rotation values
        public float currentTilt = 0f;
        public float currentPan = 0f;

        [Header("Recoil")]
        public RecoilSimpleSettings recoilSettings;
        public Vector3 targetRecoilRotation;
        public Vector3 currentRecoilRotation;

        public void ApplyRotationInputAndMove(float horizontalInput, float verticalInput)
        {
            // 1. apply input and move
            if (!invertY) verticalInput = -verticalInput;

            currentPan += horizontalInput * sensitivity * Time.deltaTime;
            currentTilt += verticalInput * sensitivity * Time.deltaTime;
            currentTilt = Mathf.Clamp(currentTilt, minTiltAngle, maxTiltAngle);


            // 2 apply recoil and move

            Vector3 rotationAfterRecoil = Vector3.zero;
            targetRecoilRotation = Vector3.Lerp(targetRecoilRotation, Vector3.zero, recoilSettings.returnForce * Time.deltaTime);
            currentRecoilRotation = Vector3.Slerp(currentRecoilRotation, targetRecoilRotation, recoilSettings.snappiness * Time.deltaTime);

            if (!recoilSettings.autoReturnXY)
            {
                // transform the recoil rotations into pan and tilt to stay ther
                float tiltToTransform = currentRecoilRotation.x;
                float panToTransform = currentRecoilRotation.y;

                currentTilt = Mathf.Clamp(currentTilt + tiltToTransform, minTiltAngle, maxTiltAngle);
                currentPan += panToTransform;

                currentRecoilRotation.x -= tiltToTransform;
                currentRecoilRotation.y -= panToTransform;

                targetRecoilRotation.x -= tiltToTransform;
                targetRecoilRotation.y -= panToTransform;
            }
                

            rotationAfterRecoil = new Vector3
            (
                Mathf.Clamp(currentTilt + currentRecoilRotation.x, minTiltAngle, maxTiltAngle),
                currentPan + currentRecoilRotation.y,
                currentRecoilRotation.z
            );

            transform.localRotation = Quaternion.Euler(rotationAfterRecoil);
        }

        public void ApplyRecoil()
        {
            targetRecoilRotation += new Vector3
                (
                    -recoilSettings.recoilX,
                    Random.Range(-recoilSettings.recoilY, recoilSettings.recoilY),
                    Random.Range(-recoilSettings.recoilZ, recoilSettings.recoilZ)
                );
        }

        public void SetRecoilSettings(RecoilSimpleSettings settings)
        {
            recoilSettings = settings;
        }

        private void Update()
        {
            if (autoUpdate)
            {
                ApplyRotationInputAndMove
                (
                    Input.GetAxis("Horizontal"),
                    Input.GetAxis("Vertical")
                );
            }
        }
    }
}
