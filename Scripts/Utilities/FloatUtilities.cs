using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace Benito.ScriptingFoundations.Utilities
{
    public static class FloatUtilities
    {
        public static float Remap(this float value, float from1, float to1, float from2, float to2, bool clamp = false)
        {
            if (clamp)
            {
                return Mathf.Clamp((value - from1) / (to1 - from1) * (to2 - from2) + from2, from2, to2);
            }
            else
            {
                return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
            }
        }


        /// <summary>
        /// Wasnt thoroughy tested yet.
        /// </summary>
        public static float CalculateBrakeDistance(float currentVelocity, float maxDeceleration)
        {
            return currentVelocity * currentVelocity / (2 * maxDeceleration);
        } 
        public static float CalculateBrakeDistance2(float currentVelocity, float maxDeceleration)
        {
            float timeToReachV0 = Mathf.Abs(currentVelocity) / maxDeceleration;
            return currentVelocity * timeToReachV0 + 0.5f * maxDeceleration * timeToReachV0 * timeToReachV0;

        }
        



        public static float ClampVelocityToPreventOvershoot(this float currentVelocity, float remainingDistanceToTarget, float deltaTime)
        {
            return Mathf.Clamp(currentVelocity * deltaTime, -Mathf.Abs(remainingDistanceToTarget), Mathf.Abs(remainingDistanceToTarget)) / deltaTime;
        }

        public static bool WouldVelocityOvershoot(float currentVelocity, float remainingDistanceToTarget, float deltaTime)
        {
            return (remainingDistanceToTarget < currentVelocity * deltaTime);
        }

        public static float Clamp(this float value, float min, float max)
        {
            return Mathf.Clamp(value, min, max);
        }



        public static float BlendLinearly(this float currentValue, float targetValue, float speed, float deltaTime)
        {
            speed *= deltaTime;
            return currentValue + Mathf.Clamp(targetValue - currentValue, -speed, speed);
        }

        public static float BlendWithAccel(this float currentValue, float targetValue, float maxSpeed, float maxAcceleration, ref float currentVelocity, float deltaTime)
        {
            float differenceTowardsTarget = targetValue - currentValue;

            float acceleration = differenceTowardsTarget < 0 ? acceleration = -maxAcceleration : acceleration = maxAcceleration;
            currentVelocity += acceleration * deltaTime;
            currentVelocity = Mathf.Clamp(currentVelocity, -maxSpeed, maxSpeed);

            currentVelocity = currentVelocity.ClampVelocityToPreventOvershoot(differenceTowardsTarget, deltaTime);
            currentValue += currentVelocity * deltaTime;

            return currentValue;
        }

        public static float BlendWithAccelAndDecel(this float currentValue, float targetValue, float maxSpeed, float maxAcceleration, ref float currentVelocity, bool overshoot, float deltaTime)
        {
            Profiler.BeginSample("Float Blend");
            if (currentValue == targetValue && currentVelocity == 0) // performance optimisation
                return currentValue;

            // Only used if overshoot is true. Should be a bit higher than the normal acceleration to allow full deceleration at high timesteps.
            float limitToFakeDeceleration = maxAcceleration * 1.1f; 
            // Adjust how this margins are calculated if needed, this setup seems to be good for now.
            float overshootVelocityErrorMargin = 0.1f * maxAcceleration; 
            float positionErrorMargin = 0.0005f * maxAcceleration;
            bool brake = false;
            float differenceTowardsTarget = targetValue - currentValue;

            // Snap
            if (ShouldSnapToTarget(currentVelocity))
            {
                currentVelocity = 0f;
                differenceTowardsTarget = 0f;
                currentValue = targetValue;
                return currentValue;
            }

            // Check for Brake
            if (GoingIntoDirectionOfTarget(currentVelocity))
            {
                brake = ShouldBrake(currentVelocity);
            }

            // Calculate Acceleration
            if (brake)
            {
                float decelerationToBrakeCorrectly = -(currentVelocity * currentVelocity) / (2 * differenceTowardsTarget);
                if (overshoot) decelerationToBrakeCorrectly = Mathf.Clamp(decelerationToBrakeCorrectly, -limitToFakeDeceleration, limitToFakeDeceleration);
                currentVelocity += decelerationToBrakeCorrectly * deltaTime;
            }
            else
            {
                float acceleration = differenceTowardsTarget < 0? acceleration = -maxAcceleration : acceleration = maxAcceleration;
                currentVelocity += acceleration * deltaTime;
            }

            // Clamp
            currentVelocity = Mathf.Clamp(currentVelocity, -maxSpeed, maxSpeed);
            if (ShouldClampResultingVelocityToPreventOvershoot(currentVelocity))
            {
                currentVelocity = currentVelocity.ClampVelocityToPreventOvershoot(differenceTowardsTarget, deltaTime);
            }

            // Apply Velocity
            currentValue += currentVelocity * deltaTime;

            return currentValue;

            #region Local Functions

            bool ShouldSnapToTarget(float currentVelocity)
            {
                return Mathf.Abs(differenceTowardsTarget) < positionErrorMargin && (!overshoot || overshoot && Mathf.Abs(currentVelocity) < overshootVelocityErrorMargin);
            }

            bool GoingIntoDirectionOfTarget(float currentVelocity)
            {
                return differenceTowardsTarget > 0 && currentVelocity > 0 || differenceTowardsTarget < 0 && currentVelocity < 0;
            }

            bool ShouldBrake(float currentVelocity)
            {
                float timeToReachV0 = Mathf.Abs(currentVelocity) / maxAcceleration;
                float brakeDeceleration = currentVelocity > 0 ? -maxAcceleration : maxAcceleration;
                float valueAfterBrakingNow = currentValue + currentVelocity * timeToReachV0 + 0.5f * brakeDeceleration * timeToReachV0 * timeToReachV0;
                
                if (Mathf.Abs(valueAfterBrakingNow - currentValue) >= Mathf.Abs(differenceTowardsTarget))
                    return true;
                
                return false;
            }

            bool ShouldClampResultingVelocityToPreventOvershoot(float currentVelocity)
            {
                return !overshoot || overshoot && Mathf.Abs(currentVelocity) < overshootVelocityErrorMargin;
            }

            #endregion
            Profiler.EndSample();
        }


        public static float CalculatePhysicalRecoil(this float currentValue, float maxValue, float maxRecoilSpeed, float maxReduceRecoilSpeed, float reduceRecoilAcceleration, ref float currentVelocity, float deltaTime)
        {
            if (currentValue == 0 && currentVelocity == 0) // performance optimisation
                return 0;

            float valueDifference = 0-currentValue;
            float valueDifferenceAbsolute = Mathf.Abs(valueDifference);
            bool isMovingAgainstRecoil = valueDifference > 0 && currentVelocity > 0 || valueDifference < 0 && currentVelocity < 0;
            bool brake = false;

            // Snap
            if(Mathf.Abs(valueDifference) < 0.1f && Mathf.Abs(currentVelocity) < 0.1f)
            {
                currentVelocity = 0;
                currentValue = 0;
                return currentValue;
            }

            // Check for Brake
            if (isMovingAgainstRecoil)
            {
                brake = valueDifferenceAbsolute <= CalculateBrakeDistance(currentVelocity, reduceRecoilAcceleration);
            }

            // Add Acceleration
            if (brake)
            {
                if (valueDifference > 0)
                {
                    currentVelocity -= reduceRecoilAcceleration * deltaTime;
                    if (currentVelocity < 0) currentVelocity = 0; // prevent from "overbraking"
                }
                else
                {
                    currentVelocity += reduceRecoilAcceleration * deltaTime;
                    if (currentVelocity > 0) currentVelocity = 0;
                }
            }
            else
            {
                float acceleration = valueDifference > 0 ? reduceRecoilAcceleration : -reduceRecoilAcceleration;
                currentVelocity += acceleration * deltaTime;
            }


            //Define again, after adjusting the velocity
            isMovingAgainstRecoil = valueDifference > 0 && currentVelocity > 0 || valueDifference < 0 && currentVelocity < 0;

            // Clamp Velocity
            if (isMovingAgainstRecoil)
            {
                currentVelocity = Mathf.Clamp(currentVelocity, -maxReduceRecoilSpeed, maxReduceRecoilSpeed);
                if(WouldVelocityOvershoot(Mathf.Abs(currentVelocity), valueDifferenceAbsolute, deltaTime))
                {
                    currentVelocity = 0;
                    currentValue = 0;
                    return currentValue;
                }
            }
            else
            {
                currentVelocity = Mathf.Clamp(currentVelocity, -maxRecoilSpeed, maxRecoilSpeed);
            }


            // Apply Velocity
            currentValue += currentVelocity * deltaTime;
            currentValue = Mathf.Clamp(currentValue ,- maxValue, maxValue);
            return currentValue;
        }
    }

}


