using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Benito.ScriptingFoundations.Utilities;
using Benito.ScriptingFoundations.NaughtyAttributes;
using Benito.ScriptingFoundations.Utilities.Guns;

namespace Benito.ScriptingFoundations.Utilities.Guns
{
    /// <summary>
    /// Represents a tank that is being rotated along its foward axis and still keeps aiming at the target at the same time
    /// </summary>
    public class ExampleTurretControllerMono : MonoBehaviour
    {
        [SerializeField] bool useAcceleration;

        [SerializeField] Transform targetTransform;
        [Space]
        [SerializeField] Transform bodyTransform;
        [SerializeField] Transform turretAnchorTransform;
        [SerializeField] Transform turretTransform;

        [SerializeField] Transform gunAnchorTransform;
        [SerializeField] Transform gunTransform;

        // Todo change this into seperate serializable class 
        [Header("Turret Stats")]
        [SerializeField] TurretSettings turretSettings;
        [SerializeField] TurretState turretState;


        private void Start()
        {
            turretState = new TurretState();
        }

        void Update()
        {
            if (useAcceleration)
                TurretUtilities.CalculateStateAnglesAcceleration(ref turretState, targetTransform.position, turretAnchorTransform, gunAnchorTransform, turretSettings, Time.deltaTime);
            else
                TurretUtilities.CalculateStateAnglesLinearSpeed(ref turretState, targetTransform.position, turretAnchorTransform, gunAnchorTransform, turretSettings, Time.deltaTime);

            turretTransform.rotation = TurretUtilities.GetTargetTurretRotation(turretAnchorTransform.rotation, turretAnchorTransform.up, turretState.turretRotation);
            gunTransform.rotation = TurretUtilities.GetTargetGunRotation(gunAnchorTransform.rotation, gunAnchorTransform.right, turretState.gunRotation);

            // Debug
            //Debug.DrawLine(gunTransform.position, gunTransform.position + gunTransform.forward * 25, Color.blue);
        }
    }
}
