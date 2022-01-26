using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Benito.ScriptingFoundations.Utilities
{
    /// <summary>
    /// Holds formulas containing launch angle and time of flight calculations .
    /// Formulas are from  https://gamedev.stackexchange.com/questions/53552/how-can-i-find-a-projectiles-launch-angle.
    /// </summary>
    public static class ProjectileUtilities
    {
        /// <summary>
        /// There will almost always be 2 angles at which we can shoot (except at 45 degrees).
        /// If direct shot is true, it means we take the lower angle of the 2.
        /// </summary>
        public static float CalculateProjectileLaunchAngle(float launchVelocity, Vector3 startPosition, Vector3 targetPosition, bool directShot = true)
        {
            Vector3 distDelta = targetPosition - startPosition;

            return CalculateProjectileLaunchAngle(launchVelocity, new Vector3(distDelta.x, 0f, distDelta.z).magnitude, distDelta.y, directShot);
        }

        /// <summary>
        /// There will almost always be 2 angles at which we can shoot (except at 45 degrees).
        /// If direct shot is true, it means we take the lower angle of the 2.
        /// </summary>
        public static float CalculateProjectileLaunchAngle(float speed, float horizontalDistance, float heightDifference, bool directShoot = true, float gravity = 9.71f)
        {
            float theta = 0f;

            if (directShoot)
            {
                theta = Mathf.Atan((Mathf.Pow(speed, 2) - Mathf.Sqrt(Mathf.Pow(speed, 4) - gravity * (gravity * Mathf.Pow(horizontalDistance, 2) + 2 * heightDifference * Mathf.Pow(speed, 2)))) / (gravity * horizontalDistance));
            }
            else
            {
                theta = Mathf.Atan((Mathf.Pow(speed, 2) + Mathf.Sqrt(Mathf.Pow(speed, 4) - gravity * (gravity * Mathf.Pow(horizontalDistance, 2) + 2 * heightDifference * Mathf.Pow(speed, 2)))) / (gravity * horizontalDistance));
            }

            return (theta * (180 / Mathf.PI));  //change into degrees
        }

        public static float CalculateTimeOfFlightOfProjectileLaunchedAtAnAngle(float projectileLaunchVelocity, float launchAngleInDegrees, Vector3 projectileLaunchPosition, Vector3 targetPosition, float gravity = 9.71f)
        {
            float timeInAir;
            float vY = projectileLaunchVelocity * Mathf.Sin(launchAngleInDegrees * (Mathf.PI / 180));

            float startH = projectileLaunchPosition.y;
            float finalH = targetPosition.y;

            if (finalH < startH)
            {
                timeInAir = (vY + Mathf.Sqrt((float)(Mathf.Pow(vY, 2) - 4 * (0.5 * gravity) * (-(startH - finalH))))) / gravity;
            }
            else
            {
                float vX = projectileLaunchVelocity * Mathf.Cos(launchAngleInDegrees * (Mathf.PI / 180));
                float distanceX = Vector3.Distance(projectileLaunchPosition, targetPosition);
                timeInAir = distanceX / vX;
            }

            return timeInAir;
        }

        public static Vector3 CalculateRandomSpreadInConeShapeAroundTransformForwardV(Transform relativeTransform, float bloomAngle)
        {
            // todo make this use this transform matrix  - so it works without a transform aswell

            // Relative transform would be the shoot point transform if we are calculating this for a gun.
            // Imagine a circle one unit in front of the 0/0/0 point - the radius of the circle is depending on the desired bloom/spread angle. Now in this circle we do the randomInsideCircle.

            //tan(alpha) = b/a  -> tan(alpha) * a = b
            //a = 1, b varies

            float unitSphereRadius = Mathf.Tan(bloomAngle * Mathf.Deg2Rad);

            // To make the points appear more often in the middle of the circle, we add a random scaler, which maz reduce the radius
            Vector2 insideUnitCircle = Random.insideUnitCircle * unitSphereRadius * Random.Range(0f, 1f);

            return relativeTransform.TransformDirection(new Vector3(insideUnitCircle.x, insideUnitCircle.y, 1f));
        }

        public static Quaternion CalculateRandomSpreadInConeShapeAroundTransformForwardQ(Transform relativeTransform, float bloomAngle)
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
    }
}
