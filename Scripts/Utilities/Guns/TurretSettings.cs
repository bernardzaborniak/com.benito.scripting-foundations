using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Benito.ScriptingFoundations.Utilities.Guns
{
    [System.Serializable]
    public class TurretSettings
    {
        [Tooltip("in angles")]
        public float turretRotationSpeed;
        public float turretAcceleration;
        [Tooltip("in angles")]
        public float gunRotationSpeed;
        public float gunAcceleration;

        [Space]
        public bool hasTurretRotLimit;
        [Tooltip("Relative to body forward in angles, left is in -80 for example while right is +80")]
        public float turretRotLeftLimit;
        public float turretRotRightLimit;

        [Space]
        public bool hasGunRotLimit;
        [Tooltip("Relative to turret forward in angles -80 for upwards, but is used as lower limit")]
        public float gunRotLowerLimit;
        [Tooltip("Relative to turret forward in angles 80 for downwards but is used as upper limit")]
        public float gunRotUpperLimit;
    }
}