using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Benitos.ScriptingFoundations.Utilities
{
    public static class QuaternionUtilities
    {
        public static Quaternion CalculateRandomSpreadInConeShapeAroundTransformForward(Transform relativeTransform, float bloomAngle)         
        {
            // Relative transform would be the shoot point transform if we are calculating this for a gun.
            // Imagine a circle one unit in front of the 0/0/0 point - the radius of the circle is depending on the desired bloom/spread angle. Now in this circle we do the randomInsideCircle.

            //tan(alpha) = b/a  -> tan(alpha) * a = b
            //a = 1, b varies

            float unitSphereRadius = Mathf.Tan(bloomAngle * Mathf.Deg2Rad);

            // To make the points appear more often in the middle of the circle, we add a random scaler, which maz reduce the radius
            Vector2 insideUnitCircle = Random.insideUnitCircle * unitSphereRadius * Random.Range(0f, 1f);

            return Quaternion.LookRotation(relativeTransform.TransformDirection(new Vector3(insideUnitCircle.x, insideUnitCircle.y, 1f)));
        }

        public static Quaternion RotateAlongAxis(this Quaternion quaternion, Vector3 axis, float degrees)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Rotation Speed is in degrees per second.
        /// </summary>
        public static Quaternion RotateAlongAxis(this Quaternion quaternion, Vector3 axis, float rotationSpeed, float deltaTime)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Rotate towards target point, but only on specified axis, usefull for a tank turret for example, which rotates the turret along the tank y axis and the brrel along the turrets x axis.
        /// </summary>
        public static Quaternion RotateTowardsAlongAxis(this Quaternion quaternion, Quaternion targetRotation, float degrees)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Rotate towards target point, but only on specified axis, usefull for a tank turret for example, which rotates the turret along the tank y axis and the brrel along the turrets x axis.
        /// Rotation Speed is in degrees per second. Acceleration is in m/s2.
        /// </summary>
        public static Quaternion RotateTowardsAlongAxis(this Quaternion quaternion, Quaternion targetRotation, float rotationSpeed, float deltaTime)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// RotationSpeed & CurrentAngularVelocity are in degrees per second. Acceleration is in m/s2.
        /// </summary>
        public static Quaternion RotateTowardsWithAcceleration(this Quaternion quaternion, Quaternion targetRotation, float maxRotationSpeed, float maxAcceleration, ref Vector3 currentAngularVelocity, float deltaTime)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Rotate towards target point, but only on specified axis, usefull for a tank turret for example, which rotates the turret along the tank y axis and the brrel along the turrets x axis.
        /// RotationSpeed & CurrentAngularVelocity are in degrees per second. Acceleration is in m/s2.
        /// </summary>
        public static Quaternion RotateTowardsAlongAxisWithAcceleration(this Quaternion quaternion, Quaternion targetRotation, float maxRotationSpeed, float maxAcceleration, ref Vector3 currentAngularVelocity, float deltaTime)
        {
            throw new System.NotImplementedException();
        }
    }
}
