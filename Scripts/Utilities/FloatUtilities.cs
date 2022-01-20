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

        public static void BlendLinearly(ref float currentValue, float targetValue, float blendSpeed, float deltaTime)
        {
            blendSpeed *= deltaTime;
            currentValue += Mathf.Clamp(targetValue - currentValue, -blendSpeed, blendSpeed);
        }


        public static float BlendWithAcceleration(this float currentValue, float targetValue, float maxBlendSpeed, float maxBlednAcceleration, ref float currentSpeed,float deltaTime)
        {
            throw new System.NotImplementedException();
        }

        public static void BlendWithAcceleration(ref float currentValue, float targetValue, float maxBlendSpeed, float maxBlednAcceleration, ref float currentSpeed, float deltaTime)
        {
            throw new System.NotImplementedException();
        }
    }

}


