using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Benito.ScriptingFoundations.Utilities.Guns
{
    public class RecoilController : MonoBehaviour
    {
        public class CurrentRecoilControllerSettings
        {
            [Header("Up Recoil")]
            public RecoilDirectionSettings recoilUp = new RecoilDirectionSettings();

            [Header("Side Recoil")]
            public RecoilDirectionSettings recoilSide = new RecoilDirectionSettings();
            public float recoilSideDirectionThreshold;
            public AnimationCurve recoilSideMultiplyerSurve;

            [Header("Back Recoil")]
            public RecoilDirectionSettings recoilBack = new RecoilDirectionSettings();

            public void UpdateRecoilSettings(RecoilGunSettings gunStats, bool useTwoHanded)
            {
                if (useTwoHanded)
                {
                    recoilUp = gunStats.recoilUp2Handed;
                    recoilSide = gunStats.recoilSide2Handed;
                    recoilBack = gunStats.recoilBack2Handed;
                    recoilSideDirectionThreshold = gunStats.recoilSideDirectionThreshold2Handed;
                    recoilSideMultiplyerSurve = gunStats.upToSideRecoilMultiplyer2Handed;

                }
                else
                {
                    recoilUp = gunStats.recoilUp1Handed;
                    recoilSide = gunStats.recoilSide1Handed;
                    recoilBack = gunStats.recoilBack1Handed;
                    recoilSideDirectionThreshold = gunStats.recoilSideDirectionThreshold1Handed;
                    recoilSideMultiplyerSurve = gunStats.upToSideRecoilMultiplyer1Handed;
                }
            }
        }

        class CurrentRecoilValues
        {
            public float value;
            public float velocity;

            public void AddForce(float force)
            {
                velocity += force;
            }

            public void Update(float maxRecoilSpeed, float maxReduceRecoilSpeed, float maxReduceRecoilAcceleration, float maxValue, float deltaTime)
            {
                value = FloatUtilities.CalculatePhysicalRecoil(value, maxValue, maxRecoilSpeed, maxReduceRecoilSpeed, maxReduceRecoilAcceleration, ref velocity, deltaTime);
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
        public void UpdateRecoilSettings(RecoilGunSettings gunStats, bool useTwoHanded)
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
            float sideForceToApply = maxSideForce * currentRecoilSettings.recoilSideMultiplyerSurve.Evaluate(currentRotUp.value / currentRecoilSettings.recoilUp.maxValue);
            // add a bool and a curve to scale this?
            //if (currentRotUp.value <= 0)
            //   return 0;

            //float sideForceToApply = maxSideForce * (currentRotUp.value / currentRecoilSettings.recoilUp.maxValue);
            //float sideForceToApply = maxSideForce;

            // Rotate left or right partly random.
            if (Random.value > currentRecoilSettings.recoilSideDirectionThreshold)
                sideForceToApply = -sideForceToApply;

            return sideForceToApply;
        }

        private void Update()
        {
            currentRotUp.Update(
                currentRecoilSettings.recoilUp.maxSpeed, 
                currentRecoilSettings.recoilUp.maxReduceRecoilSpeed,
                currentRecoilSettings.recoilUp.maxReduceRecoilAcceleration, 
                currentRecoilSettings.recoilUp.maxValue,
                Time.deltaTime);
            
            currentRotSide.Update(currentRecoilSettings.recoilSide.maxSpeed, 
                currentRecoilSettings.recoilSide.maxReduceRecoilSpeed,
                currentRecoilSettings.recoilSide.maxReduceRecoilAcceleration, 
                currentRecoilSettings.recoilSide.maxValue,
                Time.deltaTime);

            currentPosBack.Update(currentRecoilSettings.recoilBack.maxSpeed, 
                currentRecoilSettings.recoilBack.maxReduceRecoilSpeed,
                currentRecoilSettings.recoilBack.maxReduceRecoilAcceleration,
                currentRecoilSettings.recoilBack.maxValue,
                Time.deltaTime);

            transformToApplyRecoilTo.localPosition = new Vector3(0, 0, -currentPosBack.value*0.01f); // We are using centimeters instead of meters as speed.
            transformToApplyRecoilTo.localRotation = Quaternion.Euler(-currentRotUp.value, currentRotSide.value, 0);
        }
    }
}


