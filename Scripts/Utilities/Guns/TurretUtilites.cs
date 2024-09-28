using Benito.ScriptingFoundations.Utilities.Guns;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;
using static UnityEngine.GraphicsBuffer;

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
        #region Helper Methods

        private static void CalculateTargetRotations(Vector3 targetPos, Transform turretAnchorTransform, Transform gunAnchorTransform,
            out float targetTurretRot, out float targetGunRot)
        {
            Vector3 targetDirectionInTurretSpace = Quaternion.Inverse(turretAnchorTransform.rotation) * (targetPos - turretAnchorTransform.position);
            targetTurretRot = Vector2.SignedAngle(targetDirectionInTurretSpace.ToVector2_xz(), Vector2.up);

            Vector3 targetDirectionInGunSpace = Quaternion.Inverse(gunAnchorTransform.rotation) * (targetPos - gunAnchorTransform.position);
            float xzLength = targetDirectionInGunSpace.ToVector2_xz().magnitude;
            Vector2 targetGunSpaceDirectionFlattened = new Vector2(xzLength, targetDirectionInGunSpace.y);
            targetGunRot = Vector2.SignedAngle(targetGunSpaceDirectionFlattened.normalized, Vector2.right);
        }

        private static float HandleTurretOverwrap(float currentTurretRotation, float targetTurretRotation, TurretSettings settings)
        {
            if (!settings.hasTurretRotLimit)
            {
                if (currentTurretRotation < 0 && targetTurretRotation > 0 && targetTurretRotation - currentTurretRotation > 180)
                {
                    return targetTurretRotation - 360;
                }
                else if (currentTurretRotation > 0 && targetTurretRotation < 0 && targetTurretRotation - currentTurretRotation < -180)
                {
                    return targetTurretRotation + 360;
                }
            }
            return targetTurretRotation;
        }

        private static void ClampRotations(ref TurretState state, TurretSettings settings)
        {
            if (settings.hasTurretRotLimit)
            {
                state.turretRotation = Mathf.Clamp(state.turretRotation, settings.turretRotLeftLimit, settings.turretRotRightLimit);
            }

            if (settings.hasGunRotLimit)
            {
                state.gunRotation = Mathf.Clamp(state.gunRotation, settings.gunRotLowerLimit, settings.gunRotUpperLimit);
            }
        }

        #endregion


        public static void CalculateStateAnglesLinearSpeed(ref TurretState state, Vector3 targetPos,
            Transform turretAnchorTransform, Transform gunAnchorTransform, TurretSettings settings, float deltaTime)
        {
            float targetTurretRot, targetGunRot;
            CalculateTargetRotations(targetPos, turretAnchorTransform, gunAnchorTransform, out targetTurretRot, out targetGunRot);

            // allow to go over back with turret
            targetTurretRot = HandleTurretOverwrap(state.turretRotation, targetTurretRot, settings);

            state.turretRotation = FloatUtilities.BlendLinearly(state.turretRotation, targetTurretRot, settings.turretRotationSpeed, deltaTime);
            state.gunRotation = FloatUtilities.BlendLinearly(state.gunRotation, targetGunRot, settings.gunRotationSpeed, deltaTime);

            ClampRotations(ref state, settings);
        }

        public static void CalculateStateAnglesLinearSpeed(ref TurretState state, float targetTurretRot, float targetGunRot,
            Transform turretAnchorTransform, Transform gunAnchorTransform, TurretSettings settings, float deltaTime)
        {
            // allow to go over back with turret
            targetTurretRot = HandleTurretOverwrap(state.turretRotation, targetTurretRot, settings);

            state.turretRotation = FloatUtilities.BlendLinearly(state.turretRotation, targetTurretRot, settings.turretRotationSpeed, deltaTime);
            state.gunRotation = FloatUtilities.BlendLinearly(state.gunRotation, targetGunRot, settings.gunRotationSpeed, deltaTime);

            ClampRotations(ref state, settings);
        }

        public static void CalculateStateAnglesAcceleration(ref TurretState state, Vector3 targetPos,
            Transform turretAnchorTransform, Transform gunAnchorTransform, TurretSettings settings, float deltaTime)
        {
            float targetTurretRot, targetGunRot;
            CalculateTargetRotations(targetPos, turretAnchorTransform, gunAnchorTransform, out targetTurretRot, out targetGunRot);

            // allow to go over back with turret
            targetTurretRot = HandleTurretOverwrap(state.turretRotation, targetTurretRot, settings);

            state.turretRotation = FloatUtilities.BlendWithAccelAndDecel(state.turretRotation, targetTurretRot, 
                settings.turretRotationSpeed, settings.turretAcceleration,ref state.turretVelocity, true, deltaTime);
            state.gunRotation = FloatUtilities.BlendWithAccelAndDecel(state.gunRotation, targetGunRot, 
                settings.gunRotationSpeed, settings.gunAcceleration, ref state.gunVelocity, true, deltaTime);

            ClampRotations(ref state, settings);
        }

        public static void CalculateStateAnglesAcceleration(ref TurretState state, float targetTurretRot, float targetGunRot,
            Transform turretAnchorTransform, Transform gunAnchorTransform, TurretSettings settings, float deltaTime)
        {
            // allow to go over back with turret
            targetTurretRot = HandleTurretOverwrap(state.turretRotation, targetTurretRot, settings);

            state.turretRotation = FloatUtilities.BlendWithAccelAndDecel(state.turretRotation, targetTurretRot,
                settings.turretRotationSpeed, settings.turretAcceleration, ref state.turretVelocity, true, deltaTime);
            state.gunRotation = FloatUtilities.BlendWithAccelAndDecel(state.gunRotation, targetGunRot,
                settings.gunRotationSpeed, settings.gunAcceleration, ref state.gunVelocity, true, deltaTime);

            ClampRotations(ref state, settings);
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