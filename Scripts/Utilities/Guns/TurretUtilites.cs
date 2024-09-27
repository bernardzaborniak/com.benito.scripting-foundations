using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Benito.ScriptingFoundations.Utilities
{
    /// <summary>
    /// Helper functions to get turret rotation right in different scenarios
    /// Ideally make sure that turret and gun transform anchers have the same positions, let just their rotation differ?
    /// 
    /// Typically turret is rotated only on the local y axis and Gun is rotated oly on the local turrets x axis.
    /// </summary>
    public static class TurretUtilities
    {
        /// <summary>
        /// Yields good results when used with BlendLinearlyWithAccel to arrive at the current vector
        /// 
        /// turretBodyRotation is the rotation of the turret Parent
        /// </summary>
        /// <returns></returns>
        public static Vector3 GetTargetTurretForward(Quaternion turretBodyRotation, Vector3 aimDirection)
        {
            Vector3 aimDirectionInTurretBodySpace = Quaternion.Inverse(turretBodyRotation) * aimDirection;
            aimDirectionInTurretBodySpace.y = 0;

            return aimDirectionInTurretBodySpace;
        }

        /// <summary>
        /// Yields good results when used with BlendLinearlyWithAccel to arrive at the current vector
        /// 
        /// turretBodyRotation is the rotation of the turret Parent
        /// 
        /// This method respects the angle limits counting from bodyForward
        /// </summary>
        /// <returns></returns>
        public static Vector3 GetTargetTurretForwardWithLimits(Quaternion turretBodyRotation, float maxAnglesLeft, float maxAnglesRight)
        {
            return Vector3.zero;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="turretRotation"></param>
        /// <param name="bodyRotation">this rotation represents the parent of the turret, angle limits are relative to this rotation</param>
        /// <param name="aimDirection"></param>
        /// <param name="degrees"></param>
        /// <returns></returns>
        /*public static Quaternion RotateTurretTowardsDirection(this Quaternion turretRotation, Quaternion bodyRotation, Vector3 aimDirection, float rotationSpeed, float deltaTime)
        {
            Vector3 aimDirectionInTurretBodySpace = Quaternion.Inverse(bodyRotation) * aimDirection;
            aimDirectionInTurretBodySpace.y = 0;
            Vector3 aimDirectionInWorldSpace = bodyRotation * aimDirectionInTurretBodySpace;
            return QuaternionUtilities.RotateTowards(
                turretRotation,
                Quaternion.LookRotation(aimDirectionInWorldSpace, bodyRotation*Vector3.up),
                rotationSpeed,
                deltaTime
                );
        }*/

        
       /* public static Quaternion RotateGunTowardsDirection(this Quaternion turretRotation, Quaternion bodyRotation, Vector3 aimDirection, float degrees)
        {

        }*/
    }
}