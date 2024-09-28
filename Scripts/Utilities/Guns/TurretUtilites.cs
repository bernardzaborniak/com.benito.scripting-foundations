using Benito.ScriptingFoundations.Utilities.Guns;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;

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
        public static void CalculateStateAnglesLinearSpeed(ref TurretState state, Vector3 targetPos,
            Transform turretAnchorTransform, Transform gunAnchorTransform, TurretSettings settings, float deltaTime)
        {
            Vector3 targetDirectionInTurretSpace = Quaternion.Inverse(turretAnchorTransform.rotation) * (targetPos - turretAnchorTransform.position);
            float targetTurretRot = Vector2.SignedAngle(targetDirectionInTurretSpace.ToVector2_xz(), Vector2.up);
            Vector3 targetDirectionInGunSpace = Quaternion.Inverse(gunAnchorTransform.rotation) * (targetPos - gunAnchorTransform.position);
            float xzLength = targetDirectionInGunSpace.ToVector2_xz().magnitude;
            Vector2 targetGunSpaceDirectionFlattened = new Vector2(xzLength, targetDirectionInGunSpace.y);
            float targetGunRot = Vector2.SignedAngle(targetGunSpaceDirectionFlattened.normalized, Vector2.right);

            // allow to go over back with turret
            if (!settings.hasTurretRotLimit)
            {
                if (state.turretRotation < 0 && targetTurretRot > 0 && targetTurretRot - state.turretRotation > 180)
                {
                    targetTurretRot = targetTurretRot - 360;
                }
                else if(state.turretRotation > 0 && targetTurretRot < 0 && targetTurretRot - state.turretRotation < -180)
                {
                    targetTurretRot = targetTurretRot + 360;
                }
            }
               

            state.turretRotation = FloatUtilities.BlendLinearly(state.turretRotation, targetTurretRot, settings.turretRotationSpeed, deltaTime);
            state.gunRotation = FloatUtilities.BlendLinearly(state.gunRotation, targetGunRot, settings.gunRotationSpeed, deltaTime);


            if(settings.hasTurretRotLimit)
                state.turretRotation = Mathf.Clamp(state.turretRotation, settings.turretRotLeftLimit, settings.turretRotRightLimit);

            if (settings.hasGunRotLimit)
                state.gunRotation = Mathf.Clamp(state.gunRotation, settings.gunRotLowerLimit, settings.gunRotUpperLimit);
        }

        public static void CalculateStateAnglesLinearSpeed(ref TurretState state, float targetTurretRot, float targetGunRot,
            Transform turretAnchorTransform, Transform gunAnchorTransform, TurretSettings settings, float deltaTime)
        {

            // allow to go over back with turret
            if (!settings.hasTurretRotLimit)
            {
                if (state.turretRotation < 0 && targetTurretRot > 0 && targetTurretRot - state.turretRotation > 180)
                {
                    targetTurretRot = targetTurretRot - 360;
                }
                else if (state.turretRotation > 0 && targetTurretRot < 0 && targetTurretRot - state.turretRotation < -180)
                {
                    targetTurretRot = targetTurretRot + 360;
                }
            }


            state.turretRotation = FloatUtilities.BlendLinearly(state.turretRotation, targetTurretRot, settings.turretRotationSpeed, deltaTime);
            state.gunRotation = FloatUtilities.BlendLinearly(state.gunRotation, targetGunRot, settings.gunRotationSpeed, deltaTime);


            if (settings.hasTurretRotLimit)
                state.turretRotation = Mathf.Clamp(state.turretRotation, settings.turretRotLeftLimit, settings.turretRotRightLimit);

            if (settings.hasGunRotLimit)
                state.gunRotation = Mathf.Clamp(state.gunRotation, settings.gunRotLowerLimit, settings.gunRotUpperLimit);
        }

        public static void CalculateStateAnglesAcceleration(ref TurretState state, Vector3 targetPos,
            Transform turretAnchorTransform, Transform gunAnchorTransform, TurretSettings settings, float deltaTime)
        {
            Vector3 targetDirectionInTurretSpace = Quaternion.Inverse(turretAnchorTransform.rotation) * (targetPos - turretAnchorTransform.position);
            float targetTurretRot = Vector2.SignedAngle(targetDirectionInTurretSpace.ToVector2_xz(), Vector2.up);
            Vector3 targetDirectionInGunSpace = Quaternion.Inverse(gunAnchorTransform.rotation) * (targetPos - gunAnchorTransform.position);
            float xzLength = targetDirectionInGunSpace.ToVector2_xz().magnitude;
            Vector2 targetGunSpaceDirectionFlattened = new Vector2(xzLength, targetDirectionInGunSpace.y);
            float targetGunRot = Vector2.SignedAngle(targetGunSpaceDirectionFlattened.normalized, Vector2.right);

            // allow to go over back with turret
            if (!settings.hasTurretRotLimit)
            {
                if (state.turretRotation < 0 && targetTurretRot > 0 && targetTurretRot - state.turretRotation > 180)
                {
                    targetTurretRot = targetTurretRot - 360;
                }
                else if (state.turretRotation > 0 && targetTurretRot < 0 && targetTurretRot - state.turretRotation < -180)
                {
                    targetTurretRot = targetTurretRot + 360;
                }
            }

            state.turretRotation = FloatUtilities.BlendWithAccelAndDecel(state.turretRotation, targetTurretRot, 
                settings.turretRotationSpeed, settings.turretAcceleration,ref state.turretVelocity, true, deltaTime);
            state.gunRotation = FloatUtilities.BlendWithAccelAndDecel(state.gunRotation, targetGunRot, 
                settings.gunRotationSpeed, settings.gunAcceleration, ref state.gunVelocity, true, deltaTime);

            if (settings.hasTurretRotLimit)
                state.turretRotation = Mathf.Clamp(state.turretRotation, settings.turretRotLeftLimit, settings.turretRotRightLimit);
            if (settings.hasGunRotLimit)
                state.gunRotation = Mathf.Clamp(state.gunRotation, settings.gunRotLowerLimit, settings.gunRotUpperLimit);
        }

        public static void CalculateStateAnglesAcceleration(ref TurretState state, float targetTurretRot, float targetGunRot,
            Transform turretAnchorTransform, Transform gunAnchorTransform, TurretSettings settings, float deltaTime)
        {
            // allow to go over back with turret
            if (!settings.hasTurretRotLimit)
            {
                if (state.turretRotation < 0 && targetTurretRot > 0 && targetTurretRot - state.turretRotation > 180)
                {
                    targetTurretRot = targetTurretRot - 360;
                }
                else if (state.turretRotation > 0 && targetTurretRot < 0 && targetTurretRot - state.turretRotation < -180)
                {
                    targetTurretRot = targetTurretRot + 360;
                }
            }

            state.turretRotation = FloatUtilities.BlendWithAccelAndDecel(state.turretRotation, targetTurretRot,
                settings.turretRotationSpeed, settings.turretAcceleration, ref state.turretVelocity, true, deltaTime);
            state.gunRotation = FloatUtilities.BlendWithAccelAndDecel(state.gunRotation, targetGunRot,
                settings.gunRotationSpeed, settings.gunAcceleration, ref state.gunVelocity, true, deltaTime);

            if (settings.hasTurretRotLimit)
                state.turretRotation = Mathf.Clamp(state.turretRotation, settings.turretRotLeftLimit, settings.turretRotRightLimit);
            if (settings.hasGunRotLimit)
                state.gunRotation = Mathf.Clamp(state.gunRotation, settings.gunRotLowerLimit, settings.gunRotUpperLimit);
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