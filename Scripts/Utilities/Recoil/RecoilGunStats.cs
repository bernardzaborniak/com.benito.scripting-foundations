using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Benito.ScriptingFoundations.Utilities.Recoil
{
    [System.Serializable]
    public class RecoilGunStats
    {       
        [Header("Up Recoil - Measured in Degrees")]

        public float recoilUpShootForce;
        public RecoilDirectionSettings recoilUp1Handed;
        public RecoilDirectionSettings recoilUp2Handed;
     

        [Header("Side Recoil - Measured in Degrees")]

        public float recoilSideShootForce;
        public RecoilDirectionSettings recoilSide1Handed;
        public RecoilDirectionSettings recoilSide2Handed;

        [Range(0, 1)] [Tooltip("0.5 means side Recoil will be randomly added to the left or right. 0 means all recoil will go to the left. 1 all recoil will go to the right")]
        public float recoilSideDirectionThreshold1Handed = 0.5f;

        [Tooltip("Determines if and how much the up recoil scales the side recoil. \n X-Axis 0 is up recoil 0, 1 is up recoil max. \n Y-Axis 0 to 1 is the 0-1 multiplier applied to the side recoil")]
        public AnimationCurve upToSideRecoilMultiplyer1Handed;

        [Range(0, 1)][Tooltip("0.5 means side Recoil will be randomly added to the left or right. 0 means all recoil will go to the left. 1 all recoil will go to the right")]
        public float recoilSideDirectionThreshold2Handed = 0.5f;

        [Tooltip("Determines if and how much the up recoil scales the side recoil. \n X-Axis 0 is up recoil 0, 1 is up recoil max. \n Y-Axis 0 to 1 is the 0-1 multiplier applied to the side recoil")]
        public AnimationCurve upToSideRecoilMultiplyer2Handed;





        [Header("Back Recoil - Measured in Centimeters")]

        public float recoilBackShootForce;

        public RecoilDirectionSettings recoilBack1Handed;
        public RecoilDirectionSettings recoilBack2Handed;

    }
}