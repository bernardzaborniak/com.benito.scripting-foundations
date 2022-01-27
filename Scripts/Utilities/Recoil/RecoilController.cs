using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Benito.ScriptingFoundations.Utilities.Recoil
{
    public class RecoilController : MonoBehaviour
    {
        public class CurrentRecoilControllerSettings
        {
            [Header("Up Recoil")]
            public RecoilReductionSettings recoilUp = new RecoilReductionSettings();

            [Header("Side Recoil")]
            public RecoilReductionSettings recoilSide = new RecoilReductionSettings();
            public float recoilSideDirectionThreshold;

            [Header("Back Recoil")]
            public RecoilReductionSettings recoilBack = new RecoilReductionSettings();

            public void UpdateRecoilSettings(RecoilGunStats gunStats, bool useTwoHanded)
            {
                if (useTwoHanded)
                {
                    recoilUp = gunStats.recoilUp2Handed;
                    recoilSide = gunStats.recoilSide2Handed;
                    recoilBack = gunStats.recoilBack2Handed;
                    recoilSideDirectionThreshold = gunStats.recoilSideDirectionThreshold2Handed;
                }
                else
                {
                    recoilUp = gunStats.recoilUp1Handed;
                    recoilSide = gunStats.recoilSide1Handed;
                    recoilBack = gunStats.recoilBack1Handed;
                    recoilSideDirectionThreshold = gunStats.recoilSideDirectionThreshold1Handed;
                }
            }
        }

        class CurrentRecoilValues
        {
            public float value;
            public float velocity;

            public bool applyCounterForce;
            public bool brake;
            public float currentBrakeDistance;

            public void AddForce(float force)
            {
                velocity += force;
                Debug.Log("new vel: " + velocity);
            }

            public void Update(float maxSpeed, float maxCounterAcceleration, float deltaTime)
            {
                value = FloatUtilities.BlendWithAccelAndDecel(value, 0, maxSpeed, maxCounterAcceleration, ref velocity, false, deltaTime);
               // Debug.Log("update value: " + value);
                //Debug.Log(" vel after update: " + velocity);

            }
        }

        [SerializeField] Transform transformToApplyRecoilTo;

        CurrentRecoilControllerSettings currentRecoilSettings = new CurrentRecoilControllerSettings();
        CurrentRecoilValues currentRotUp = new CurrentRecoilValues();
        CurrentRecoilValues currentRotSide = new CurrentRecoilValues();
        CurrentRecoilValues currentPosBack = new CurrentRecoilValues();


        /// <summary>
        /// Called when switching the weapon or switching from 1 handed to 2 handed.
        /// </summary>
        public void UpdateRecoilSettings(RecoilGunStats gunStats, bool useTwoHanded)
        {
            currentRecoilSettings.UpdateRecoilSettings(gunStats, useTwoHanded);  
        }

        public void AddRecoil(float upForce, float sideForce, float backForce)
        {
            currentRotUp.AddForce(upForce);
            currentRotSide.AddForce(AddRandomScaledSideVelocity(sideForce));
            currentPosBack.AddForce(backForce);
        }

        float AddRandomScaledSideVelocity(float maxSideForce)
        {
            if (currentRotUp.value <= 0)
                return 0;

            float sideForceToApply = maxSideForce * (currentRotUp.value / currentRecoilSettings.recoilUp.maxValue);

            // Rotate left or right partly random.
            if (Random.value < currentRecoilSettings.recoilSideDirectionThreshold)
                sideForceToApply = -sideForceToApply;

            return sideForceToApply;
        }

        private void Update()
        {
            currentRotUp.Update(currentRecoilSettings.recoilUp.maxSpeed, currentRecoilSettings.recoilUp.maxReduceRecoilAcceleration, Time.deltaTime);
            currentRotSide.Update(currentRecoilSettings.recoilSide.maxSpeed, currentRecoilSettings.recoilSide.maxReduceRecoilAcceleration, Time.deltaTime);
            currentPosBack.Update(currentRecoilSettings.recoilBack.maxSpeed, currentRecoilSettings.recoilBack.maxReduceRecoilAcceleration, Time.deltaTime);

            transformToApplyRecoilTo.localPosition = new Vector3(0, 0, -currentPosBack.value*0.01f); // We are using centimeters instead of meters as speed.
            transformToApplyRecoilTo.localRotation = Quaternion.Euler(-currentRotUp.value, currentRotSide.value, 0);
        }
    }
}


