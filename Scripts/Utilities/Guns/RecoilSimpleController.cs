using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Benito.ScriptingFoundations.Utilities.Guns
{
    //Inspired by  https://www.youtube.com/watch?v=geieixA4Mqc - gilbert youtube channel
    public class RecoilSimpleController : MonoBehaviour
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

        //Rotations
        private Vector3 currentRotation;
        private Vector3 targetRotation;

        [SerializeField] RecoilSimpleSettings settings;

        private void Start() { targetRotation = Vector3.zero; }


        void Update()
        {
            if (settings.autoReturnXY) targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, settings.returnForce * Time.deltaTime);
            else targetRotation.z = Mathf.Lerp(targetRotation.z, 0f, settings.returnForce * Time.deltaTime);

            currentRotation = Vector3.Slerp(currentRotation, targetRotation, settings.snappiness * Time.deltaTime);
            transform.localRotation = Quaternion.Euler(currentRotation);
        }

        public void ApplyRecoil()
        {
            targetRotation += new Vector3(-settings.recoilX, Random.Range(-settings.recoilY, settings.recoilY), Random.Range(-settings.recoilZ, settings.recoilZ));
        }
        public void SetSettings(RecoilSimpleSettings settings)
        {
            this.settings = settings;
        }
    }
}
