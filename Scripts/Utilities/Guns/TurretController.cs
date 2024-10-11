
using UnityEngine;

///Helps with commonly found turret controls a bit
namespace Benito.ScriptingFoundations.Utilities.Guns
{
    [System.Serializable]
    public class TurretController
    {
        //Refs
        public Transform turretAnchor;
        public Transform turret;
        public Transform gunAnchor;
        public Transform gun;

        //public Transform projectileLaunchPoint;

        TurretState turretState;
        TurretSettings turretSettings;

        public void Initialize(TurretSettings turretSettings)
        {
            this.turretState = new TurretState();
            this.turretSettings = turretSettings;
        }

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
    }
}