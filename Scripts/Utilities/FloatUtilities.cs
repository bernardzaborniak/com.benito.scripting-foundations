using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

            // Clamp to prevent overshoot.
            currentVelocity = Mathf.Clamp(currentVelocity * deltaTime, -Mathf.Abs(differenceTowardsTarget), Mathf.Abs(differenceTowardsTarget)) / deltaTime;
            currentValue += currentVelocity * deltaTime;

            return currentValue;
        }

        public static float BlendWithAccelAndDecel(this float currentValue, float targetValue, float maxSpeed, float maxAcceleration, ref float currentVelocity, bool overshoot, float deltaTime)
        {
            // Only used if overshoot is true. Should be a bit higher than the normal acceleration to allow full deceleration at high timesteps.
            float limitToFakeDeceleration = maxAcceleration * 1.1f; 
            // Adjust how this margins are calculated if needed, this setup seems to be good for now.
            float overshootVelocityErrorMargin = 0.05f * maxAcceleration; 
            float positionErrorMargin = 0.0005f * maxAcceleration;
            bool brake = false;

            float differenceTowardsTarget = targetValue - currentValue;

            if (ShouldSnapToTarget(currentVelocity))
            {
                currentVelocity = 0f;
                differenceTowardsTarget = 0f;
                currentValue = targetValue;
                return currentValue;
            }

            if (GoingIntoDirectionOfTarget(currentVelocity))
            {
                brake = ShouldBrake(currentVelocity);
            }

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

            currentVelocity = Mathf.Clamp(currentVelocity, -maxSpeed, maxSpeed);

            if (ShouldClampResultingVelocityToPreventOvershoot(currentVelocity))
                currentVelocity = Mathf.Clamp(currentVelocity * deltaTime, -Mathf.Abs(differenceTowardsTarget), Mathf.Abs(differenceTowardsTarget)) / deltaTime;

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
        }
    }

}


