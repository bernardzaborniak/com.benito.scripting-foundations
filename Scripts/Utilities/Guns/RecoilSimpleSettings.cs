using System;
using UnityEngine;

namespace Benito.ScriptingFoundations.Utilities.Guns
{
    [Serializable]
    public class RecoilSimpleSettings
    {
        public float recoilX = 5f;
        public float recoilY = 2f;
        public float recoilZ = 5f;

        [Tooltip("Lower snapiness will make the movement slow and heavy feeling, higher will make it snappier :)")]
        public float snappiness = 5f;

        [Tooltip("How fast should the recoil go back on z and if autoReturnXY=true , also on y and z?")]
        public float returnForce = 5f;

        [Tooltip("Wheter the recoil on x and y should automatically go back")]
        public bool autoReturnXY = true;


    }

}
