using UnityEngine;

namespace Benito.ScriptingFoundations.CameraHelpers
{
    // Todo move this to utilities just where the simple camera controller is ?
    public class PanTiltCameraController : MonoBehaviour
    {
        public bool autoUpdate;

        public float sensitivity = 1f;
        public bool invertY = false;

        [Tooltip("looking up")]
        public float minTiltAngle = -70f;
        [Tooltip("looking down")]
        public float maxTiltAngle = 70f; 

        // Current rotation values
        public float currentTilt = 0f; 
        public float currentPan = 0f;

        public void ApplyRotationInputAndMove(float horizontalInput, float verticalInput)
        {
            if (!invertY) verticalInput = -verticalInput;

            currentPan += horizontalInput * sensitivity * Time.deltaTime;
            currentTilt += verticalInput * sensitivity * Time.deltaTime;
            currentTilt = Mathf.Clamp(currentTilt, minTiltAngle, maxTiltAngle);

            transform.localRotation = Quaternion.Euler(currentTilt, currentPan, 0f);
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
