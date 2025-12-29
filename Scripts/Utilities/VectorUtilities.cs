using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Benito.ScriptingFoundations.Utilities
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

        public static Vector3 TransformInputToCameraSpace(Vector2 input, Transform camTransform, bool clampDiagonalInput = true)
        {
            // Get the camera's forward and right vectors, ignoring the Y component, flatten them to make sure we only use the horizontal (X-Z) components
            Vector3 cameraForward = camTransform.forward.ToVector3_x0z().normalized;
            Vector3 cameraRight = camTransform.right.ToVector3_x0z().normalized;

            Vector3 inputInWorldSpace = cameraForward * input.x + cameraRight * input.y;

            if (clampDiagonalInput)
            {
                inputInWorldSpace = Vector3.ClampMagnitude(inputInWorldSpace, 1);
            }

            return inputInWorldSpace;
        }


        public static Vector3 ClampVelocityToPreventOvershoot(this Vector3 currentVelocity, float remainingDistanceToTarget, float deltaTime)
        {
            return Vector3.ClampMagnitude(currentVelocity * deltaTime, remainingDistanceToTarget) / deltaTime;
        }

        public static Vector3 BlendLinearly(this Vector3 currentVector, Vector3 targetVector, float speed, float deltaTime)
        {
            Vector3 vectorTowardsTarget = targetVector - currentVector;
            return currentVector += Vector3.ClampMagnitude(
                vectorTowardsTarget.normalized * speed * deltaTime,
                vectorTowardsTarget.magnitude
                );
        }

        public static Vector3 BlendLinearlyWithAccel(this Vector3 currentVector, Vector3 targetVector, float maxSpeed, float maxAcceleration, ref Vector3 currentVelocity, float deltaTime)
        {
            Vector3 vectorTowardsTarget = targetVector - currentVector;

            Vector3 acceleration = vectorTowardsTarget.normalized * maxAcceleration;
            currentVelocity += acceleration * deltaTime;
            currentVelocity = Vector3.ClampMagnitude(currentVelocity, maxSpeed);

            currentVelocity = currentVelocity.ClampVelocityToPreventOvershoot(vectorTowardsTarget.magnitude, deltaTime);
            currentVector += currentVelocity * deltaTime;

            return currentVector;
        }

        public static Vector3 BlendLinearlyWithAccelAndDecel(this Vector3 currentVector, Vector3 targetVector, float maxSpeed, float maxAcceleration, ref Vector3 currentVelocity, bool overshoot, float deltaTime)
        {
            // Only used if overshoot is true. Should be a bit higher than the normal acceleration to allow full deceleration at high timesteps.
            float limitToFakeDeceleration = maxAcceleration * 1.1f;
            // Adjust how this margins are calculated if needed, this setup seems to be good for now.
            float overshootVelocityErrorMargin = 0.1f * maxAcceleration;
            float positionErrorMargin = 0.0005f * maxAcceleration;
            bool brake = false;

            Vector3 vectorTowardsTarget = targetVector - currentVector;
            float distanceTowardsTarget = vectorTowardsTarget.magnitude;

            float currentVelocityMagnitude = currentVelocity.magnitude;

            if (ShouldSnapToTarget())
            {
                currentVelocity = Vector3.zero;
                vectorTowardsTarget = Vector3.zero;
                currentVector = targetVector;
                return currentVector;
            }

            if (GoingIntoDirectionOfTarget(currentVelocity))
            {
                brake = ShouldBrake(currentVelocity);
            }

            if (brake)
            {
                float decelerationToBrakeCorrectly = (currentVelocityMagnitude * currentVelocityMagnitude) / (2 * distanceTowardsTarget); // Calculate Acceleration needed to change from initial to final velocity over distance https://docs.google.com/document/d/1_q4Nphvuas84DPDBshBWDuZy5VwznjSqWg7Let9vTIE/edit#
                if (overshoot) decelerationToBrakeCorrectly = Mathf.Clamp(decelerationToBrakeCorrectly, -limitToFakeDeceleration, limitToFakeDeceleration);
                Vector3 deceleration = -vectorTowardsTarget.normalized * decelerationToBrakeCorrectly;
                currentVelocity += deceleration * deltaTime;
            }
            else
            {
                Vector3 acceleration = vectorTowardsTarget.normalized * maxAcceleration;
                currentVelocity += acceleration * deltaTime;
            }

            currentVelocity = Vector3.ClampMagnitude(currentVelocity, maxSpeed);

            if (ShouldClampResultingVelocityToPreventOvershoot())
                currentVelocity = currentVelocity.ClampVelocityToPreventOvershoot(distanceTowardsTarget, deltaTime);

            currentVector += currentVelocity * deltaTime;

            return currentVector;

            #region Local Functions

            bool ShouldSnapToTarget()
            {
                return distanceTowardsTarget < positionErrorMargin && (!overshoot || overshoot && currentVelocityMagnitude < overshootVelocityErrorMargin);
            }

            bool GoingIntoDirectionOfTarget(Vector3 currentVelocity)
            {
                // that is difficult - just check the angle?
                return Vector3.Angle(currentVelocity,vectorTowardsTarget)<90;
            }

            bool ShouldBrake(Vector3 currentVelocity)
            {
                float timeToReachV0 = currentVelocityMagnitude / maxAcceleration; // Time to reach vel https://docs.google.com/document/d/1_q4Nphvuas84DPDBshBWDuZy5VwznjSqWg7Let9vTIE/edit#heading=h.aykw6apzg0to
                Vector3 brakeDeceleration = -vectorTowardsTarget.normalized * maxAcceleration;
                Vector3 valueAfterBrakingNow = currentVector + currentVelocity * timeToReachV0 + 0.5f * brakeDeceleration * timeToReachV0 * timeToReachV0;

                if (Vector3.Distance(valueAfterBrakingNow, currentVector) >= distanceTowardsTarget)
                    return true;

                return false;
            }

            bool ShouldClampResultingVelocityToPreventOvershoot()
            {
                return !overshoot || overshoot && currentVelocityMagnitude < overshootVelocityErrorMargin;
            }

            #endregion
        }



        /// <summary>
        /// Calculates angle between 2 vectors ONLY on the specified axis. Contrary to Unitys angle axis ignoring the axis in angle calculation and only using it to determine if signed or not.
        /// Source: https://forum.unity.com/threads/is-vector3-signedangle-working-as-intended.694105/
        /// </summary>
        public static float AngleOffAroundAxis(Vector3 from, Vector3 to, Vector3 axis, bool clockwise = false)
        {
            Vector3 right;
            if (clockwise)
            {
                right = Vector3.Cross(from, axis);
                from = Vector3.Cross(axis, right);
            }
            else
            {
                right = Vector3.Cross(axis, from);
                from = Vector3.Cross(right, axis);
            }
            return Mathf.Atan2(Vector3.Dot(to, right), Vector3.Dot(to, from)) * Mathf.Rad2Deg;
        }

        /// <summary>
        /// The Return Vector is normalized, the input vector doesnt have to be
        /// </summary>
        public static Vector3 RotateAlongAxisNormalized(this Vector3 currentVector, Vector3 axis, float degrees)
        {
            return Quaternion.LookRotation(currentVector, axis).RotateAlongAxis(axis, degrees) * Vector3.forward;
        }

        public static Vector3 RotateTowardsAlongAxisNormalized(this Vector3 currentVector, Vector3 targetVector, Vector3 axis, float degrees)
        {
            float differenceAngle = VectorUtilities.AngleOffAroundAxis(currentVector, targetVector, axis);
            differenceAngle = Mathf.Clamp(differenceAngle, -degrees, degrees);

            return Quaternion.LookRotation(currentVector, axis).RotateAlongAxis(axis, differenceAngle) * Vector3.forward;
        }

        public static Vector3 RotateTowardsAlongAxisWithAccelNormalized(this Vector3 currentVector, Vector3 targetVector, Vector3 axis, float maxSpeed,float maxAcceleration, ref float currentVelocity, float deltaTime)
        {
            float angleErrorTolerance = 0.001f; // AngleOffAroundAxis will never return exactly 0;

            float differenceAngle = VectorUtilities.AngleOffAroundAxis(currentVector, targetVector, axis);

            if (Mathf.Abs(differenceAngle) < angleErrorTolerance)
            {
                currentVelocity = 0;
                return Quaternion.LookRotation(currentVector, axis).RotateAlongAxis(axis, 0) * Vector3.forward;
            }

            float blendedAngle = FloatUtilities.BlendWithAccel(0, differenceAngle, maxSpeed, maxAcceleration, ref currentVelocity, deltaTime);
            return Quaternion.LookRotation(currentVector, axis).RotateAlongAxis(axis, blendedAngle) * Vector3.forward;
        }

        public static Vector3 RotateTowardsAlongAxisWithAccelAndDecelNormalized(this Vector3 currentVector, Vector3 targetVector, Vector3 axis, float maxSpeed, float maxAcceleration, ref float currentVelocity, bool overshoot, float deltaTime)
        {
            float angleErrorTolerance = 0.001f; // AngleOffAroundAxis will never return exactly 0;

            float differenceAngle = VectorUtilities.AngleOffAroundAxis(currentVector, targetVector, axis);

            if (Mathf.Abs(differenceAngle) < angleErrorTolerance)
            {
                currentVelocity = 0;
                return Quaternion.LookRotation(currentVector, axis).RotateAlongAxis(axis, 0) * Vector3.forward;
            }

            float blendedAngle = FloatUtilities.BlendWithAccelAndDecel(0, differenceAngle, maxSpeed, maxAcceleration, ref currentVelocity, overshoot, deltaTime);
            return Quaternion.LookRotation(currentVector, axis).RotateAlongAxis(axis, blendedAngle) * Vector3.forward;
        }

        #endregion

        #region Vector2

        public static Vector3 ToVector3_x0z(this Vector2 vector)
        {
            return new Vector3(vector.x, 0, vector.y);
        }

        /// <summary>
        /// Rotates the vector clockwise.
        /// </summary>
        public static Vector2 Rotate(this Vector2 vector, float degrees)
        {
            degrees *= -Mathf.Deg2Rad;

            return new Vector2(
                vector.x * Mathf.Cos(degrees) - vector.y * Mathf.Sin(degrees),
                vector.x * Mathf.Sin(degrees) + vector.y * Mathf.Cos(degrees)
                );
        }


        public static Vector2 RotateTowards(this Vector2 from, Vector2 to, float degrees)
        {
            //https://github.com/lordofduct/spacepuppy-unity-framework-3.0/blob/master/SpacepuppyUnityFramework/Utils/VectorUtil.cs#L171

            degrees *= Mathf.Deg2Rad;

            float a1 = Mathf.Atan2(from.y, from.x);
            float a2 = Mathf.Atan2(to.y, to.x);
            a2 = ShortenAngleToAnother(a2, a1, true);

            var da = a2 - a1;
            var ra = a1 + Mathf.Clamp(Mathf.Abs(degrees), 0f, Mathf.Abs(da)) * Mathf.Sign(da);

            var l = from.magnitude;
            return new Vector2(Mathf.Cos(ra) * l, Mathf.Sin(ra) * l);
        }

        #region V2 Rotate Towards Helper Mehtods

        // from https://github.com/lordofduct/spacepuppy-unity-framework-3.0/blob/master/SpacepuppyUnityFramework/Utils/VectorUtil.cs#L171

        /// <summary>
        /// closest angle from a1 to a2
        /// absolute value the return for exact angle
        static float NearestAngleBetween(float a1, float a2, bool useRadians)
        {
            float rd = useRadians ? Mathf.PI : 180f;
            float ra = Wrap(a2 - a1, rd * 2f,0);
            if (ra > rd) ra -= (rd * 2f);
            return ra;
        }

        /// <summary>
        /// Returns a value for dependant that is a value that is the shortest angle between dep and ind from ind.
        /// 
        /// 
        /// for instance if dep=-170 degrees and ind=170 degrees then 190 degrees will be returned as an alternative to -170 degrees
        /// note: angle is passed in radians, this written example is in degrees for ease of reading
        static float ShortenAngleToAnother(float dep, float ind, bool useRadians)
        {
            return ind + NearestAngleBetween(ind, dep, useRadians);
        }

        /// <summary>
        /// Wraps a value around some significant range.
        /// 
        /// Similar to modulo, but works in a unary direction over any range (including negative values).
        /// 
        /// ex:
        /// Wrap(8,6,2) == 4
        /// Wrap(4,2,0) == 0
        /// Wrap(4,2,-2) == 0
        static float Wrap(float value, float max, float min)
        {
            max -= min;
            if (max == 0)
                return min;

            return value - max * (int)Mathf.Floor((value - min) / max);
        }



        #endregion

        #endregion
    }
}
