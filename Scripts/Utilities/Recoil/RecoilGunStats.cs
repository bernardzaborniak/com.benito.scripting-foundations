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
        public RecoilReductionSettings recoilUp1Handed;
        public RecoilReductionSettings recoilUp2Handed;
     

        [Header("Side Recoil - Measured in Degrees")]

        public float recoilSideShootForce;
        public RecoilReductionSettings recoilSide1Handed;
        public RecoilReductionSettings recoilSide2Handed;

        [Range(0, 1)] [Tooltip("0.5 means side Recoil will be randomly added to the left or right. 0 means all recoil will go to the left. 1 all recoil will go to the right")]
        public float recoilSideDirectionThreshold1Handed = 0.5f;

        [Range(0, 1)][Tooltip("0.5 means side Recoil will be randomly added to the left or right. 0 means all recoil will go to the left. 1 all recoil will go to the right")]
        public float recoilSideDirectionThreshold2Handed = 0.5f;


        [Header("Side Recoil - Measured in Centimeters")]

        public float recoilBackShootForce;

        public RecoilReductionSettings recoilBack1Handed;
        public RecoilReductionSettings recoilBack2Handed;

    }
}