using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Benitos.ScriptingFoundations.Utilities
{
    public static class VectorUtilities
    {
        #region Vector3
        public static Vector3 ToVector3_x0z(this Vector3 vector)
        {
            vector.y = 0;
            return vector;
        }
        public static Vector3 ToVector3_0y0(this Vector3 vector)
        {
            vector.x = vector.z = 0;
            return vector;
        }

        public static Vector2 ToVector2_xz(this Vector3 vector)
        {
            return new Vector2(vector.x, vector.z);
        }

        public static Vector3 BlendLinearly(this Vector3 currentValue, Vector3 targetValue, float blendSpeed, float deltaTime)
        {
            throw new System.NotImplementedException();
        }

        public static Vector3 BlendLinearlyWithAcceleration(this Vector3 currentValue, Vector3 targetValue, float maxBlendSpeed, float maxBlendSpeedAcceleration, ref float currentVelocity, float deltaTime)
        {
            throw new System.NotImplementedException();
        }

        public static Vector3 RotateAlongAxis(this Vector3 vectorToRotate, Vector3 axis, float degrees)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Rotation Speed is in degrees per second.
        /// </summary>
        public static Vector3 RotateAlongAxis(this Vector3 vectorToRotate, Vector3 axis, float rotationSpeed, float deltaTime)
        {
            throw new System.NotImplementedException();
        }

        public static Vector3 CalculateRandomSpreadInConeShapeAroundTransformForward(Transform relativeTransform, float bloomAngle)
        {
            // Relative transform would be the shoot point transform if we are calculating this for a gun.
            // Imagine a circle one unit in front of the 0/0/0 point - the radius of the circle is depending on the desired bloom/spread angle. Now in this circle we do the randomInsideCircle.

            //tan(alpha) = b/a  -> tan(alpha) * a = b
            //a = 1, b varies

            float unitSphereRadius = Mathf.Tan(bloomAngle * Mathf.Deg2Rad);

            // To make the points appear more often in the middle of the circle, we add a random scaler, which maz reduce the radius
            Vector2 insideUnitCircle = Random.insideUnitCircle * unitSphereRadius * Random.Range(0f, 1f);

            return relativeTransform.TransformDirection(new Vector3(insideUnitCircle.x, insideUnitCircle.y, 1f));
        }

        #endregion

        #region Vector2

        public static Vector3 ToVector3_x0z(this Vector2 vector)
        {
            return new Vector3(vector.x, 0, vector.y);
        }

        public static Vector2 Rotate(this Vector2 vector, float degrees)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Rotation Speed is in degrees per second.
        /// </summary>
        public static Vector2 Rotate(this Vector2 vector, float rotationSpeed, float deltaTime)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}
