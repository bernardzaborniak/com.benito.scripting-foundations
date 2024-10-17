
using UnityEngine;
using Benito.ScriptingFoundations.Validator;

///Helps with commonly found turret controls a bit
namespace Benito.ScriptingFoundations.Utilities.Guns
{
    [System.Serializable]
    public class TurretController
    {
        //Refs
        public Transform turretAnchor;
        [Tooltip("must be child of turretAnchor and have 0 pos, 0 rot and 1 scale")]
        public Transform turret;
        public Transform gunAnchor;
        [Tooltip("must be child of gunAnchor and have 0 pos, 0 rot and 1 scale")]
        public Transform gun;

        [Tooltip("Used to calculate how good our current aim is for CurrentAnglesToTargetDelta(). Most of the time this will be the same as the projectileLaunchPoint")]
        public Transform aimReferencePoint;

        //public Transform projectileLaunchPoint;

        TurretState turretState;
        TurretSettings turretSettings;

        #region Validation
        /// <summary>
        /// Helper method called on initialize to check weather the turrent is set up correctly
        /// </summary>
        public void ValidateTurretReferences()
        {
            if (turret.localPosition != Vector3.zero || turret.localRotation != Quaternion.identity)
            {
                Debug.Log("Turret Controller Validation failed, make sure turret.localPosition == Vector3.zero && turret.localRotation == Quaternion.identity");
            }
            if (gun.localPosition != Vector3.zero || gun.localRotation != Quaternion.identity)
            {
                Debug.Log("Turret Controller Validation failed, make sure gun.localPosition == Vector3.zero && gun.localRotation == Quaternion.identity");
            }
        }
        #endregion

        public void Initialize(TurretSettings turretSettings)
        {
            this.turretState = new TurretState();
            this.turretSettings = turretSettings;
            ValidateTurretReferences();
        }

        /// <summary>
        /// Returns the difference in angles, between current launch direction and desired to reach the target
        /// </summary>
        /// <returns></returns>
        public float CurrentAnglesToTargetDelta(Vector3 targetAim)
        {
            Vector3 directionGunTipToEnemy = targetAim - aimReferencePoint.position;
            return Vector3.Angle(directionGunTipToEnemy, gun.forward);
        }

        #region Control Methods

        // Linear Variants (more performant, looks more arcadey though, less realistic)
        public void AimTargetSimple(Vector3 aimTarget)
        {
            TurretUtilities.CalculateStateAnglesLinearSpeed(ref turretState, aimTarget, turretAnchor, gunAnchor, turretSettings, Time.deltaTime);

            turret.rotation = TurretUtilities.GetTargetTurretRotation(turretAnchor.rotation, turretAnchor.up, turretState.turretRotation);
            gun.rotation = TurretUtilities.GetTargetGunRotation(gunAnchor.rotation, gunAnchor.right, turretState.gunRotation);
        }
        public void AimAnglesSimple(float turretRotation, float gunRotation)
        {
            TurretUtilities.CalculateStateAnglesLinearSpeed(ref turretState, turretRotation, gunRotation, turretAnchor, gunAnchor, turretSettings, Time.deltaTime);

            turret.rotation = TurretUtilities.GetTargetTurretRotation(turretAnchor.rotation, turretAnchor.up, turretState.turretRotation);
            gun.rotation = TurretUtilities.GetTargetGunRotation(gunAnchor.rotation, gunAnchor.right, turretState.gunRotation);
        }


        // Acceleration Variants
        public void AimTargetAcceleration(Vector3 aimTarget)
        {
            TurretUtilities.CalculateStateAnglesAcceleration(ref turretState, aimTarget, turretAnchor, gunAnchor, turretSettings, Time.deltaTime);

            turret.rotation = TurretUtilities.GetTargetTurretRotation(turretAnchor.rotation, turretAnchor.up, turretState.turretRotation);
            gun.rotation = TurretUtilities.GetTargetGunRotation(gunAnchor.rotation, gunAnchor.right, turretState.gunRotation);
        }

        public void AimAnglesAcceleration(float turretRotation, float gunRotation)
        {
            TurretUtilities.CalculateStateAnglesAcceleration(ref turretState, turretRotation, gunRotation, turretAnchor, gunAnchor, turretSettings, Time.deltaTime);

            turret.rotation = TurretUtilities.GetTargetTurretRotation(turretAnchor.rotation, turretAnchor.up, turretState.turretRotation);
            gun.rotation = TurretUtilities.GetTargetGunRotation(gunAnchor.rotation, gunAnchor.right, turretState.gunRotation);
        }

        #endregion
    }
}