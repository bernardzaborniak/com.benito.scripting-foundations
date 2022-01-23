using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Benitos.ScriptingFoundations.Utilities
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

        public static void Remap(ref float value, float from1, float to1, float from2, float to2, bool clamp = false)
        {
            if (clamp)
            {
                value = Mathf.Clamp((value - from1) / (to1 - from1) * (to2 - from2) + from2, from2, to2);
            }
            else
            {
                value = (value - from1) / (to1 - from1) * (to2 - from2) + from2;
            }
        }


        public static float BlendLinearly(this float currentValue, float targetValue, float blendSpeed, float deltaTime)
        {
            blendSpeed *= deltaTime;
            return currentValue + Mathf.Clamp(targetValue - currentValue, -blendSpeed, blendSpeed);
        }

        /*public static void BlendLinearly(ref float currentValue, float targetValue, float blendSpeed, float deltaTime)
        {
            blendSpeed *= deltaTime;
            currentValue += Mathf.Clamp(targetValue - currentValue, -blendSpeed, blendSpeed);
        }*/


        public static float BlendWithAcceleration(this float currentValue, float targetValue, float maxBlendSpeed, float maxBlendSpeedAcceleration, ref float currentVelocity, float deltaTime)
        {
            float differenceToTarget = targetValue - currentValue;
            currentVelocity += Mathf.Clamp(differenceToTarget, -maxBlendSpeedAcceleration, maxBlendSpeedAcceleration) * deltaTime;
            currentValue += currentVelocity * Time.deltaTime;

            return currentValue;
        }

        public static float BlendWithAccelerationAndDeceleration(this float currentValue, float targetValue, float maxBlendSpeed, float maxBlendSpeedAcceleration, ref float currentVelocity, float deltaTime, ref bool brake, bool overshoot)
        {
         

            float limitToFakeDeceleration = maxBlendSpeedAcceleration * 1.1f; // Only used if overshoot is true should be a bit higher than the normal acceleration to allow full deceleration at high timesteps
            float overshootVelocityErrorMargin = 0.1f;
            float positionErrorMargin = 0.01f;

            float differenceToTarget = targetValue - currentValue;
            Debug.Log("differenceToTarget: " + differenceToTarget);


            if (ShouldSnapToTarget(currentVelocity))
            {
                currentVelocity = 0f;
                differenceToTarget = 0f;
                currentValue = targetValue;
                return currentValue;
            }

            //bool brake = false;

            if (GoingIntoDirectionOfTarget(currentVelocity))
            {
                brake = ShouldBrake(currentVelocity);
            }

            if (brake)
            {
                Debug.Log("Brake: " + brake + "---------------------------------");

                float accelerationToBrakeCorrectly = -(currentVelocity * currentVelocity) / (2 * differenceToTarget);
                if (overshoot) accelerationToBrakeCorrectly = Mathf.Clamp(accelerationToBrakeCorrectly, -limitToFakeDeceleration, limitToFakeDeceleration);
                currentVelocity += accelerationToBrakeCorrectly * deltaTime;
            }
            else
            {
                float acceleration = differenceToTarget < 0? acceleration = -maxBlendSpeedAcceleration : acceleration = maxBlendSpeedAcceleration;
                currentVelocity += acceleration * deltaTime;
            }

            currentVelocity = Mathf.Clamp(currentVelocity, -maxBlendSpeed, maxBlendSpeed);

            if (ShouldClampResultingVelocityToPreventOvershoot(currentVelocity));
                currentVelocity = Mathf.Clamp(currentVelocity * deltaTime, -Mathf.Abs(differenceToTarget), Mathf.Abs(differenceToTarget)) / deltaTime;

            Debug.Log("vel " + currentVelocity);
            currentValue += currentVelocity * deltaTime;

            return currentValue;


            bool ShouldSnapToTarget(float currentVelocity)
            {
                return Mathf.Abs(differenceToTarget) < positionErrorMargin && (!overshoot || overshoot && Mathf.Abs(currentVelocity) < overshootVelocityErrorMargin);
            }

            bool GoingIntoDirectionOfTarget(float currentVelocity)
            {
                return differenceToTarget > 0 && currentVelocity > 0 || differenceToTarget < 0 && currentVelocity < 0;
            }

            bool ShouldBrake(float currentVelocity)
            {
                float timeToReachV0 = Mathf.Abs(currentVelocity) / maxBlendSpeedAcceleration;
                float brakeDeceleration = currentVelocity > 0 ? -maxBlendSpeedAcceleration : maxBlendSpeedAcceleration;
                float valueAfterBrakingNow = currentValue + currentVelocity * timeToReachV0 + 0.5f * brakeDeceleration * timeToReachV0 * timeToReachV0;
                
                if (Mathf.Abs(valueAfterBrakingNow - currentValue) >= Mathf.Abs(differenceToTarget))
                    return true;
                
                return false;
            }

            bool ShouldClampResultingVelocityToPreventOvershoot(float currentVelocity)
            {
                return !overshoot || overshoot && Mathf.Abs(currentVelocity) < overshootVelocityErrorMargin;
            }
        }

        /*public static void BlendWithAcceleration(ref float currentValue, float targetValue, float maxBlendSpeed, float maxBlendSpeedAcceleration, ref float currentVelocity, float deltaTime)
        {
            throw new System.NotImplementedException();
        }*/
    }

}


