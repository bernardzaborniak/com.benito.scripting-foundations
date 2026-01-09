using UnityEngine;

namespace Benito.ScriptingFoundations.CameraHelpers
{
    [System.Serializable]
    public class HeadBobbingSettings
    {
        [Tooltip("Local position bob amount per axis")]
        public Vector3 positionAmount;
        [Tooltip("Local rotation (Euler) bob amount per axis")]
        public Vector3 rotationAmount;
        [Tooltip("Base frequency for positional bobbing")]
        public float positionFrequency = 6f;

        [Tooltip("Base frequency for rotational bobbing")]
        public float rotationFrequency = 6f;

        public static HeadBobbingSettings Empty => new HeadBobbingSettings();
    }
}
