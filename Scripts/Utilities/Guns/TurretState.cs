using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Benito.ScriptingFoundations.Utilities;

namespace Benito.ScriptingFoundations.Utilities.Guns
{
    //[System.Serializable]
    public class TurretState
    {
        /// <summary>
        /// Measured between -180 and 180
        /// </summary>
        public float turretRotation;

        /// <summary>
        /// Between -90 and 90 or -89 and 89
        /// </summary>
        public float gunRotation;

        public float turretVelocity;
        public float gunVelocity;

        //public (float targetTurretRotation, float targetGunRotation) GetTargetRotationInAngles(Vector3 targetDirection, Transform parentTransform)
        public (float targetTurretRotation, float targetGunRotation) CalculateTargetRotationInAngles(Transform targetTransform, Transform bodyTransform, Transform turretAnchorTransform, Transform gunAnchorTransform)
        {
            // convert target direction into local space ... but it will be different depending on which local spece we convert it inot - turret or gun or body

            Vector3 targetDirectionInTurretSpace = Quaternion.Inverse(turretAnchorTransform.rotation) * (targetTransform.position- turretAnchorTransform.position);
            
            //Debug.Log($"targetDirectionInTurretSpace: {targetDirectionInTurretSpace}");

            float turretAngles = Vector2.SignedAngle(targetDirectionInTurretSpace.ToVector2_xz(), Vector2.up);
            //Debug.Log($"turret angles: {turretAngles}");

            Vector3 targetDirectionInGunSpace = Quaternion.Inverse(gunAnchorTransform.rotation) * (targetTransform.position - gunAnchorTransform.position);

            //Debug.Log($"targetDirectionInGunSpace: {targetDirectionInGunSpace}");

            float xzLength = targetDirectionInGunSpace.ToVector2_xz().magnitude;
            Vector2 targetGunSpaceDirectionFlattened = new Vector2(xzLength, targetDirectionInGunSpace.y);
            //Debug.DrawRay(gunAnchorTransform.position, gunAnchorTransform.rotation * targetGunSpaceDirectionFlattened, Color.cyan);
            //Debug.Log($"targetGunSpaceDirectionFlattened: {targetGunSpaceDirectionFlattened}");
            float gunAngles = Vector2.SignedAngle(targetGunSpaceDirectionFlattened.normalized, Vector2.right);
            //Debug.Log($"gun angles: {gunAngles}");


            turretRotation = turretAngles;
            gunRotation = gunAngles;

            return (0,0);
        }

        public Quaternion GetTargetTurretRotation(Quaternion turretAnchorRotation, Vector3 turretAnchorUp)
        {
            return QuaternionUtilities.RotateAlongAxis(turretAnchorRotation, turretAnchorUp, turretRotation);
        }

        public Quaternion GetTargetGunRotation(Quaternion gunAnchorRotation, Vector3 gunAnchorRight)
        {
            return QuaternionUtilities.RotateAlongAxis(gunAnchorRotation, gunAnchorRight, gunRotation);
        }

        public void UpdateValuesAccordingToTransforms(Transform parentTransform, Transform turretTransform, Transform gunTransform)
        {

        }
    }
}