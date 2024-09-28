using Benito.ScriptingFoundations.Utilities.Guns;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Benito.ScriptingFoundations.Utilities
{
    /// <summary>
    /// Helper functions to get turret rotation right in different scenarios
    /// 
    /// Typically turret is rotated only on the local y axis and Gun is rotated oly on the local turrets x axis.
    /// Its best to have an empty anchor transform as parent of both gun and turret to make angle calculation simpler
    /// </summary>
    public static class TurretUtilities
    {
        public static (float targetTurretRotation, float targetGunRotation) CalculateTargetRotationInAnglesLinearSpeed(
            Vector3 targetPos, Transform turretAnchorTransform, Transform gunAnchorTransform, TurretSettings settings)
        {
            Vector3 targetDirectionInTurretSpace = Quaternion.Inverse(turretAnchorTransform.rotation) * (targetPos - turretAnchorTransform.position);
            float turretAngles = Vector2.SignedAngle(targetDirectionInTurretSpace.ToVector2_xz(), Vector2.up);
            Vector3 targetDirectionInGunSpace = Quaternion.Inverse(gunAnchorTransform.rotation) * (targetPos - gunAnchorTransform.position);
            float xzLength = targetDirectionInGunSpace.ToVector2_xz().magnitude;
            Vector2 targetGunSpaceDirectionFlattened = new Vector2(xzLength, targetDirectionInGunSpace.y);
            float gunAngles = Vector2.SignedAngle(targetGunSpaceDirectionFlattened.normalized, Vector2.right);

            if(settings.hasTurretRotLimit)
                turretAngles = Mathf.Clamp(turretAngles, settings.turretRotLeftLimit, settings.turretRotRightLimit);
            if (settings.hasGunRotLimit)
                gunAngles = Mathf.Clamp(gunAngles, settings.gunRotLowerLimit, settings.gunRotUpperLimit);

            return (turretAngles, gunAngles);
        }

        public static (float targetTurretRotation, float targetGunRotation) CalculateTargetRotationInAnglesAcceleration(
            Vector3 targetPos, Transform turretAnchorTransform, Transform gunAnchorTransform, TurretSettings settings)
        {
            return (0, 0);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="turretAnchorRotation"></param>
        /// <param name="turretAnchorUp"></param>
        /// <param name="localTurretRotation">Measured between -180 and 180</param>
        /// <returns></returns>
        public static Quaternion GetTargetTurretRotation(Quaternion turretAnchorRotation, Vector3 turretAnchorUp, float localTurretRotation)
        {
            return QuaternionUtilities.RotateAlongAxis(turretAnchorRotation, turretAnchorUp, localTurretRotation);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gunAnchorRotation"></param>
        /// <param name="gunAnchorRight"></param>
        /// <param name="localGunRotation">Between -90 and 90 or -89 and 89</param>
        /// <returns></returns>
        public static Quaternion GetTargetGunRotation(Quaternion gunAnchorRotation, Vector3 gunAnchorRight, float localGunRotation)
        {
            return QuaternionUtilities.RotateAlongAxis(gunAnchorRotation, gunAnchorRight, localGunRotation);
        }
    }
}