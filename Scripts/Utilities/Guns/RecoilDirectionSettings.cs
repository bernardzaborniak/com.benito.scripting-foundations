using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Benito.ScriptingFoundations.Utilities.Guns
{
    [System.Serializable]
    public struct RecoilDirectionSettings 
    {
        public float maxSpeed;
        public float maxReduceRecoilSpeed;
        public float maxReduceRecoilAcceleration;
        public float maxValue;
    }
}
