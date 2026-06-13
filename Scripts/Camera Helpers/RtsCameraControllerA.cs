using System;
using UnityEngine;

namespace Benito.ScriptingFoundations.CameraHelpers
{
    /// <summary>
    /// RTS camera controller based on SimpleCameraController.
    /// RMB drag → rotates camera. RMB quick tap → fires OnRightClick.
    /// LMB quick tap → fires OnLeftClick.
    /// </summary>
    public class RtsCameraControllerA : MonoBehaviour
    {
        class CameraState
        {
            public float yaw;
            public float pitch;
            public float roll;
            public float x;
            public float y;
            public float z;

            public void SetFromTransform(Transform t)
            {
                pitch = t.eulerAngles.x;
                yaw = t.eulerAngles.y;
                roll = t.eulerAngles.z;
                x = t.position.x;
                y = t.position.y;
                z = t.position.z;
            }

            public void Translate(Vector3 translation)
            {
                Vector3 rotated = Quaternion.Euler(pitch, yaw, roll) * translation;
                x += rotated.x;
                y += rotated.y;
                z += rotated.z;
            }

            public void LerpTowards(CameraState target, float posLerpPct, float rotLerpPct)
            {
                yaw = Mathf.Lerp(yaw, target.yaw, rotLerpPct);
                pitch = Mathf.Lerp(pitch, target.pitch, rotLerpPct);
                roll = Mathf.Lerp(roll, target.roll, rotLerpPct);
                x = Mathf.Lerp(x, target.x, posLerpPct);
                y = Mathf.Lerp(y, target.y, posLerpPct);
                z = Mathf.Lerp(z, target.z, posLerpPct);
            }

            public void UpdateTransform(Transform t)
            {
                t.eulerAngles = new Vector3(pitch, yaw, roll);
                t.position = new Vector3(x, y, z);
            }
        }

        CameraState targetState = new CameraState();
        CameraState interpolatingState = new CameraState();

        [Header("Movement")]
        public float boost = 3.5f;
        [Range(0.001f, 1f)] public float positionLerpTime = 0.2f;

        [Header("Rotation")]
        public AnimationCurve mouseSensitivityCurve = new AnimationCurve(
            new Keyframe(0f, 0.5f, 0f, 5f),
            new Keyframe(1f, 2.5f, 0f, 0f));
        [Range(0.001f, 1f)] public float rotationLerpTime = 0.01f;
        public bool invertY = false;

        [Header("Click Detection")]
        [Tooltip("Accumulated mouse axis units below which RMB/LMB counts as a click, not a drag.")]
        [SerializeField] float clickDragThreshold = 0.5f;

        // Fired when a button is released as a click (not a drag).
        public Action OnRightClick;
        public Action OnLeftClick;

        float rmbAccumulatedDelta;
        float lmbAccumulatedDelta;
        Vector2 lmbDownScreenPos;
        bool rmbDragging;

        void OnEnable()
        {
            targetState.SetFromTransform(transform);
            interpolatingState.SetFromTransform(transform);
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                rmbAccumulatedDelta = 0f;
                rmbDragging = false;
            }

            if (Input.GetMouseButtonDown(0))
            {
                lmbAccumulatedDelta = 0f;
                lmbDownScreenPos = Input.mousePosition;
            }

            Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

            if (Input.GetMouseButton(1))
            {
                rmbAccumulatedDelta += mouseDelta.magnitude;

                if (!rmbDragging && rmbAccumulatedDelta >= clickDragThreshold)
                {
                    rmbDragging = true;
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }

                float sensitivity = mouseSensitivityCurve.Evaluate(mouseDelta.magnitude);
                targetState.yaw += mouseDelta.x * sensitivity;
                targetState.pitch += mouseDelta.y * sensitivity * (invertY ? 1f : -1f);
            }

            if (Input.GetMouseButton(0))
            {
                lmbAccumulatedDelta += Vector2.Distance(Input.mousePosition, lmbDownScreenPos);
                lmbDownScreenPos = Input.mousePosition;
            }

            if (Input.GetMouseButtonUp(1))
            {
                if (rmbDragging)
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
                else
                {
                    OnRightClick?.Invoke();
                }
                rmbDragging = false;
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (lmbAccumulatedDelta < clickDragThreshold)
                    OnLeftClick?.Invoke();
            }

            // WASD translation
            Vector3 translation = Vector3.zero;
            if (Input.GetKey(KeyCode.W)) translation += Vector3.forward;
            if (Input.GetKey(KeyCode.S)) translation += Vector3.back;
            if (Input.GetKey(KeyCode.A)) translation += Vector3.left;
            if (Input.GetKey(KeyCode.D)) translation += Vector3.right;
            if (Input.GetKey(KeyCode.Q)) translation += Vector3.down;
            if (Input.GetKey(KeyCode.E)) translation += Vector3.up;

            if (Input.GetKey(KeyCode.LeftShift))
                translation *= 10f;

            boost += Input.mouseScrollDelta.y * 0.2f;
            translation *= Mathf.Pow(2f, boost) * Time.deltaTime;

            targetState.Translate(translation);

            float posLerpPct = 1f - Mathf.Exp(Mathf.Log(1f - 0.99f) / positionLerpTime * Time.deltaTime);
            float rotLerpPct = 1f - Mathf.Exp(Mathf.Log(1f - 0.99f) / rotationLerpTime * Time.deltaTime);
            interpolatingState.LerpTowards(targetState, posLerpPct, rotLerpPct);
            interpolatingState.UpdateTransform(transform);
        }
    }
}
