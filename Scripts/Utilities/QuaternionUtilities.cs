using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Benito.ScriptingFoundations.Utilities
{
    public static class QuaternionUtilities
    {
        /// <summary>
        /// Returns a Quaternion that rotates Quaternion A towards quaternion B.
        /// </summary>
        public static Quaternion GetDifferenceQuaternion(Quaternion a, Quaternion b)
        {
            return b * Quaternion.Inverse(a);
            // Is it the same as Quaternion.DifferenceQuaternion?
        }

        public static Quaternion RotateAlongAxis(this Quaternion currentRotation, Vector3 axis, float degrees)
        {
            return Quaternion.AngleAxis(degrees, axis) * currentRotation;
        }

        public static Quaternion RotateTowards(this Quaternion currentRotation, Quaternion targetRotation, float rotationSpeed, float deltaTime)
        {
            return Quaternion.RotateTowards(currentRotation, targetRotation, rotationSpeed * deltaTime);
            
        }

        /// <summary>
        /// RotationSpeed & CurrentAngularVelocity are in degrees per second. Acceleration is in degrees/s2.
        /// The algorythm works good enough but I would not consider it final yet.
        /// </summary>
        [System.Obsolete("It works ok but has issues")]
        public static Quaternion RotateTowardsWithAccel(this Quaternion currentRotation, Quaternion targetRotation, float maxRotationSpeed, float maxAcceleration, ref Vector3 currentAngularVelocity, float deltaTime)
        {
            float snapTolerance = 0.5f;

            //https://answers.unity.com/questions/49082/rotation-quaternion-to-angular-velocity.html

            Quaternion quaternionTowardsTarget = GetDifferenceQuaternion(currentRotation, targetRotation);

            float angleInDegrees;
            Vector3 rotationAxis;

            quaternionTowardsTarget.ToAngleAxis(out angleInDegrees, out rotationAxis);

            if(angleInDegrees < snapTolerance)
            {
                currentRotation = targetRotation;
                currentAngularVelocity = Vector3.zero;
                return currentRotation;
            }

            Vector3 angularDisplacement = rotationAxis * angleInDegrees;
            Vector3 desiredAngularVelocity = angularDisplacement;
            desiredAngularVelocity = desiredAngularVelocity.normalized * maxRotationSpeed;

            Vector3 desiredCurrentDelta = desiredAngularVelocity - currentAngularVelocity;

            currentAngularVelocity += Vector3.ClampMagnitude(desiredCurrentDelta, maxAcceleration) * deltaTime;
            currentAngularVelocity = Vector3.ClampMagnitude(currentAngularVelocity * deltaTime, angleInDegrees) / deltaTime;
            currentRotation = Quaternion.Euler(currentAngularVelocity * deltaTime) * currentRotation;

            return currentRotation;   
        }

        /// <summary> Isolate the y-Component of a rotation </summary>
        private static Quaternion YRotation(Quaternion q)
        {
            float theta = Mathf.Atan2(q.y, q.w);

            // quaternion representing rotation about the y axis
            return new Quaternion(0, Mathf.Sin(theta), 0, Mathf.Cos(theta));
        }
    }
}
