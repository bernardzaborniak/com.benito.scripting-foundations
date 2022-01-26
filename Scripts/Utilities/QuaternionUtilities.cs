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
        }

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

        public static Quaternion RotateAlongAxis(this Quaternion currentRotation, Vector3 axis, float degrees)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Rotation Speed is in degrees per second.
        /// </summary>
        public static Quaternion RotateAlongAxis(this Quaternion currentRotation, Vector3 axis, float rotationSpeed, float deltaTime)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Rotate towards target point, but only on specified axis, usefull for a tank turret for example, which rotates the turret along the tank y axis and the brrel along the turrets x axis.
        /// </summary>
        public static Quaternion RotateTowardsAlongAxis(this Quaternion currentRotation, Quaternion targetRotation, float degrees)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Rotate towards target point, but only on specified axis, usefull for a tank turret for example, which rotates the turret along the tank y axis and the brrel along the turrets x axis.
        /// Rotation Speed is in degrees per second. Acceleration is in m/s2.
        /// </summary>
        public static Quaternion RotateTowardsAlongAxis(this Quaternion currentRotation, Quaternion targetRotation, float rotationSpeed, float deltaTime)
        {
            throw new System.NotImplementedException();
        }

        public static Quaternion RotateTowards(this Quaternion currentRotation, Quaternion targetRotation, float rotationSpeed, float deltaTime)
        {
            return Quaternion.RotateTowards(currentRotation, targetRotation, rotationSpeed * deltaTime);
            
        }

        static Quaternion DeltaRotationTaylorApprox(Vector3 angularVelocity, float deltaTime)
        {
            Vector3 ha = angularVelocity * deltaTime * 0.5f;
            return new Quaternion(1.0f, ha.x, ha.y, ha.z);
        }

        public static Quaternion AngVelToDeriv(Quaternion Current, Vector3 AngVel)
        {
            var Spin = new Quaternion(AngVel.x, AngVel.y, AngVel.z, 0f);
            var Result = Spin * Current;
            return new Quaternion(0.5f * Result.x, 0.5f * Result.y, 0.5f * Result.z, 0.5f * Result.w);
        }

        public static Vector3 DerivToAngVel(Quaternion Current, Quaternion Deriv)
        {
            var Result = Deriv * Quaternion.Inverse(Current);
            return new Vector3(2f * Result.x, 2f * Result.y, 2f * Result.z);
        }

        public static Quaternion IntegrateRotation(Quaternion Rotation, Vector3 AngularVelocity, float DeltaTime)
        {
            if (DeltaTime < Mathf.Epsilon) return Rotation;
            var Deriv = AngVelToDeriv(Rotation, AngularVelocity);
            var Pred = new Vector4(
                    Rotation.x + Deriv.x * DeltaTime,
                    Rotation.y + Deriv.y * DeltaTime,
                    Rotation.z + Deriv.z * DeltaTime,
                    Rotation.w + Deriv.w * DeltaTime
            ).normalized;
            return new Quaternion(Pred.x, Pred.y, Pred.z, Pred.w);
        }

        /// <summary>
        /// RotationSpeed & CurrentAngularVelocity are in degrees per second. Acceleration is in degrees/s2.
        /// </summary>
        public static Quaternion RotateTowardsWithAccel(this Quaternion currentRotation, Quaternion targetRotation, float maxRotationSpeed, float maxAcceleration, ref Vector3 currentAngularVelocity, float deltaTime)
        {
            //https://answers.unity.com/questions/49082/rotation-quaternion-to-angular-velocity.html

            Quaternion quaternionTowardsTarget = GetDifferenceQuaternion(currentRotation, targetRotation);

            float angleInDegrees;
            Vector3 rotationAxis;

            quaternionTowardsTarget.ToAngleAxis(out angleInDegrees, out rotationAxis);

            Vector3 angularDisplacement = rotationAxis * angleInDegrees;
            //Vector3 angularSpeed = angularDisplacement deltaTime;
            Vector3 angularSpeed = angularDisplacement;
            angularSpeed = angularSpeed.normalized * maxRotationSpeed;
            angularSpeed = Vector3.ClampMagnitude(angularSpeed * deltaTime, angleInDegrees) / deltaTime;

            //Debug.Log("vel: " + angularSpeed.ToString("F4"));
           // Debug.Log("vel in deg: " + (angularSpeed).ToString("F4"));

            currentRotation = Quaternion.Euler(angularSpeed* deltaTime) * currentRotation;


            return currentRotation;
            //return IntegrateRotation(currentRotation, angularSpeed, 1);


            float anglesTowardsTarget = Quaternion.Angle(currentRotation, targetRotation);
            Quaternion quaternionTowardsTarget2 = GetDifferenceQuaternion(currentRotation, targetRotation);

            //Vector3 velocity = quaternionTowardsTarget.eulerAngles.normalized * maxRotationSpeed;

            //Vector3 velocity =  DerivToAngVel(currentRotation, targetRotation).normalized * maxRotationSpeed;
            //velocity = Vector3.ClampMagnitude(velocity*deltaTime, anglesTowardsTarget)/deltaTime;

            
            Vector3 velocity = DerivToAngVel(currentRotation, targetRotation);
            Debug.Log("vel: " + velocity.ToString("F4"));
            return IntegrateRotation(currentRotation, velocity, deltaTime);

            //return quaternionTowardsTarget * currentRotation;
            //return DeltaRotationTaylorApprox(velocity,deltaTime) * currentRotation;

            Vector3 acceleration = quaternionTowardsTarget2.eulerAngles.normalized * maxAcceleration;
            currentAngularVelocity += acceleration * deltaTime;
            currentAngularVelocity = Vector3.ClampMagnitude(currentAngularVelocity, maxRotationSpeed);

            // Clamp to prevent overshoot.
            currentAngularVelocity = Vector3.ClampMagnitude(currentAngularVelocity * deltaTime, Quaternion.Angle(currentRotation, targetRotation)) /deltaTime;

            currentRotation = Quaternion.Euler(currentAngularVelocity*deltaTime) * currentRotation;

            return currentRotation;

            // Try with difference Vector instead


            /*float distanceInDegrees = Quaternion.Angle(currentRotation, targetRotation);
            Debug.Log("distanceInDegrees: " + distanceInDegrees);
            float angularVelocityMagnitude = currentAngularVelocity.magnitude;
            Debug.Log("angularVelocityMagnitude before: " + angularVelocityMagnitude);

            float floatBlend = FloatUtilities.BlendWithAccel(0, distanceInDegrees, maxRotationSpeed, maxAcceleration, ref angularVelocityMagnitude, deltaTime);
            Debug.Log("angularVelocityMagnitude after: " + angularVelocityMagnitude);

            Debug.Log("float blend: " + floatBlend);
            Quaternion newRotation = Quaternion.RotateTowards(currentRotation, targetRotation, floatBlend);
            Debug.Log("currentAngularVelocity before: " + currentAngularVelocity.ToString("F4"));

            currentAngularVelocity = (newRotation * Quaternion.Inverse(currentRotation)).eulerAngles.normalized * angularVelocityMagnitude;
            Debug.Log("currentAngularVelocity now: " + currentAngularVelocity.ToString("F4"));
            return newRotation;*/
        }

        public static Quaternion RotateTowardsWithAccelAndDecel(this Quaternion currentRotation, Quaternion targetRotation, float maxRotationSpeed, float maxAcceleration, ref Vector3 currentAngularVelocity, bool overshoot, float deltaTime)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Rotate towards target point, but only on specified axis, usefull for a tank turret for example, which rotates the turret along the tank y axis and the brrel along the turrets x axis.
        /// RotationSpeed & CurrentAngularVelocity are in degrees per second. Acceleration is in m/s2.
        /// </summary>
        public static Quaternion RotateTowardsAlongAxisWithAcceleration(this Quaternion currentRotation, Quaternion targetRotation, float maxRotationSpeed, float maxAcceleration, ref Vector3 currentAngularVelocity, float deltaTime)
        {
            throw new System.NotImplementedException();
        }
    }
}
