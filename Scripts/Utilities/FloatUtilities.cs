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

        public static float BlendWithAccelerationAndDeceleration(this float currentValue, float targetValue, float maxBlendSpeed, float maxBlendSpeedAcceleration, ref float currentVelocity, float deltaTime)
        {
            float errorMargin = 0.01f;
            Debug.Log("-------------------------");
            float differenceToTarget = targetValue - currentValue;
            Debug.Log("differenceToTarget: " + differenceToTarget);

            if (Mathf.Abs(differenceToTarget) < errorMargin)
            {
                currentVelocity = 0f;
                differenceToTarget = 0f;
                currentValue = targetValue;
                return currentValue;
            }

            bool brake = false;
            float timeToReachV0 = (0 -currentVelocity) / maxBlendSpeedAcceleration;
            Debug.Log("timeToReachV0: " + timeToReachV0);

            float valueAfterBrakingNow = currentValue + currentVelocity * timeToReachV0 + 0.5f * maxBlendSpeedAcceleration * timeToReachV0 * timeToReachV0;
            Debug.Log("valueAfterBrakingNow: " + valueAfterBrakingNow);
            Debug.Log("currentValue: " + currentValue);
            Debug.Log("valueAfterBrakingNow - currentValue: " + (valueAfterBrakingNow - currentValue));

            if (Mathf.Abs(valueAfterBrakingNow - currentValue) >= Mathf.Abs(differenceToTarget))
                brake = true;

            if (brake)
            {
                Debug.Log("Brake: " + brake);

                //float velocityDelta = 0 - currentVelocity;
                //float distanceDelta = differenceToTarget;
                //float accelerationToBrakeCorrectly = -(currentVelocity * currentVelocity) / (2 *Mathf.Abs(differenceToTarget));
                float accelerationToBrakeCorrectly = -(currentVelocity * currentVelocity) / (2 *differenceToTarget);
                currentVelocity += accelerationToBrakeCorrectly * deltaTime;
                //deltaV / deltaDistance
                //currentValue = +velocityDelta / distanceDelta * velocityDelta * deltaTime;
            }
            else
            {
                currentVelocity += Mathf.Clamp(differenceToTarget, -maxBlendSpeedAcceleration, maxBlendSpeedAcceleration) * deltaTime;
                //currentVelocity = Mathf.Clamp(currentVelocity, -differenceToTarget, differenceToTarget);

            }

            currentValue += currentVelocity * deltaTime;




            return currentValue;
        }

        /*public static void BlendWithAcceleration(ref float currentValue, float targetValue, float maxBlendSpeed, float maxBlendSpeedAcceleration, ref float currentVelocity, float deltaTime)
        {
            throw new System.NotImplementedException();
        }*/
    }

}


