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
        [Tooltip("Relative to body forward in angles")]
        public float turretRotLeftLimit;
        public float turretRotRightLimit;

        [Space]
        public bool hasGunRotLimit;
        [Tooltip("Relative to turret forward in angles")]
        public float gunRotUpperLimit;
        [Tooltip("Relative to turret forward in angles")]
        public float gunRotLowerLimit;
    }
}