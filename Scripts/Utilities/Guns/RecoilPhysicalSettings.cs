using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Benito.ScriptingFoundations.Utilities.Guns
{
    [System.Serializable]
    public class RecoilPhysicalSettings
    {       
        [Header("Up Recoil - Measured in Degrees")]

        public float recoilUpShootForce;
        public RecoilDirectionSettings recoilUp;
     
        [Header("Side Recoil - Measured in Degrees")]

        public float recoilSideShootForce;
        public RecoilDirectionSettings recoilSide;

        [Range(0, 1)] [Tooltip("0.5 means side Recoil will be randomly added to the left or right. 0 means all recoil will go to the left. 1 all recoil will go to the right")]
        public float recoilSideDirectionBalance = 0.5f;

        [Tooltip("Determines if and how much the up recoil scales the side recoil. \n X-Axis 0 is up recoil 0, 1 is up recoil max. \n Y-Axis 0 to 1 is the 0-1 multiplier applied to the side recoil")]
        public AnimationCurve upToSideRecoilMultiplierCurve;

        [Header("Back Recoil - Measured in Centimeters")]
        public float recoilBackShootForce;

        public RecoilDirectionSettings recoilBack;
    }
}