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
    }
}
