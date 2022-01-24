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
            Vector3 vectorTowardsTarget = targetValue - currentValue;
            return currentValue += Vector3.ClampMagnitude(
                vectorTowardsTarget.normalized * blendSpeed * deltaTime,
                vectorTowardsTarget.magnitude
                );
        }

        public static Vector3 BlendLinearlyWithAccel(this Vector3 currentValue, Vector3 targetValue, float maxBlendSpeed, float maxBlendSpeedAcceleration, ref Vector3 currentVelocity, float deltaTime)
        {
            Vector3 vectorTowardsTarget = targetValue - currentValue;

            Vector3 acceleration = vectorTowardsTarget.normalized * maxBlendSpeedAcceleration;
            currentVelocity += acceleration * deltaTime;
            currentVelocity = Vector3.ClampMagnitude(currentVelocity, maxBlendSpeed);

            // Clamp to prevent overshoot.
            currentVelocity = Vector3.ClampMagnitude(currentVelocity * deltaTime, vectorTowardsTarget.magnitude) / deltaTime;
            currentValue += currentVelocity * deltaTime;

            return currentValue;
        }

        public static Vector3 BlendLinearlyWithAccelAndDecel(this Vector3 currentValue, Vector3 targetValue, float maxBlendSpeed, float maxBlendSpeedAcceleration, ref Vector3 currentVelocity, bool overshoot, float deltaTime)
        {
            // Only used if overshoot is true. Should be a bit higher than the normal acceleration to allow full deceleration at high timesteps.
            float limitToFakeDeceleration = maxBlendSpeedAcceleration * 1.1f;
            // Adjust how this margins are calculated if needed, this setup seems to be good for now.
            float overshootVelocityErrorMargin = 0.05f * maxBlendSpeedAcceleration;
            float positionErrorMargin = 0.0005f * maxBlendSpeedAcceleration;
            bool brake = false;

            Vector3 vectorTowardsTarget = targetValue - currentValue;
            float distanceTowardsTarget = vectorTowardsTarget.magnitude;
            Debug.Log("vectorTowardsTarget: " + vectorTowardsTarget);

            float currentVelocityMangitude = currentVelocity.magnitude;

            if (ShouldSnapToTarget())
            {
                currentVelocity = Vector3.zero;
                vectorTowardsTarget = Vector3.zero;
                currentValue = targetValue;
                return currentValue;
            }

            if (GoingIntoDirectionOfTarget(currentVelocity))
            {
                brake = ShouldBrake(currentVelocity);
            }

            if (brake)
            {
                Debug.Log("Brake: " + brake + "---------------------------------");

                float accelerationToBrakeCorrectly = -(currentVelocity * currentVelocity) / (2 * differenceTowardsTarget);
                if (overshoot) accelerationToBrakeCorrectly = Mathf.Clamp(accelerationToBrakeCorrectly, -limitToFakeDeceleration, limitToFakeDeceleration);
                currentVelocity += accelerationToBrakeCorrectly * deltaTime;
            }
            else
            {
                float acceleration = differenceTowardsTarget < 0 ? acceleration = -maxBlendSpeedAcceleration : acceleration = maxBlendSpeedAcceleration;
                currentVelocity += acceleration * deltaTime;
            }

            currentVelocity = Mathf.Clamp(currentVelocity, -maxBlendSpeed, maxBlendSpeed);

            if (ShouldClampResultingVelocityToPreventOvershoot(currentVelocity))
                currentVelocity = Mathf.Clamp(currentVelocity * deltaTime, -Mathf.Abs(differenceTowardsTarget), Mathf.Abs(differenceTowardsTarget)) / deltaTime;

            Debug.Log("vel " + currentVelocity);
            currentValue += currentVelocity * deltaTime;

            return currentValue;

            #region Local Functions

            bool ShouldSnapToTarget()
            {
                return distanceTowardsTarget < positionErrorMargin && (!overshoot || overshoot && currentVelocityMangitude < overshootVelocityErrorMargin);
            }

            bool GoingIntoDirectionOfTarget(Vector3 currentVelocity)
            {
                // that is difficult - just check the angle?
                return Vector3.Angle(currentVelocity,vectorTowardsTarget)<90;
            }

            bool ShouldBrake(Vector3 currentVelocity)
            {
                float timeToReachV0 = Mathf.Abs(currentVelocity) / maxBlendSpeedAcceleration;
                float brakeDeceleration = currentVelocity > 0 ? -maxBlendSpeedAcceleration : maxBlendSpeedAcceleration;
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
