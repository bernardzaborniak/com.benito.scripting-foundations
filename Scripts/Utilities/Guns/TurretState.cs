using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Benito.ScriptingFoundations.Utilities;

namespace Benito.ScriptingFoundations.Utilities.Guns
{
    //[System.Serializable]
    public class TurretState
    {
        /// <summary>
        /// Measured between -180 and 180
        /// </summary>
        public float turretRotation;

        /// <summary>
        /// Between -90 and 90 or -89 and 89
        /// </summary>
        public float gunRotation;

        public float turretVelocity;
        public float gunVelocity; 
    }
}