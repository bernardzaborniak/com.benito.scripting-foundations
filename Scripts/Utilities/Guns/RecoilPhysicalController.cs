using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Benito.ScriptingFoundations.Utilities.Guns
{
    public class RecoilPhysicalController : MonoBehaviour
    {
        class RecoilPhysicalInternalValue
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

        RecoilPhysicalSettings currentRecoilSettings;
        RecoilPhysicalInternalValue currentRotUp = new RecoilPhysicalInternalValue();
        RecoilPhysicalInternalValue currentRotSide = new RecoilPhysicalInternalValue();
        RecoilPhysicalInternalValue currentPosBack = new RecoilPhysicalInternalValue();


        /// <summary>
        /// Called when switching the weapon or switching from 1 handed to 2 handed.
        /// </summary>
        public void SetRecoilSettings(RecoilPhysicalSettings recoilSettings)
        {
            currentRecoilSettings = recoilSettings;
        }

        public void AddDefaultRecoil()
        {
            AddRecoil(currentRecoilSettings.recoilUpShootForce, currentRecoilSettings.recoilSideShootForce, currentRecoilSettings.recoilBackShootForce);
        }
        

        public void AddRecoil(float upForce, float sideForce, float backForce)
        {
            currentRotUp.AddForce(upForce);
            currentRotSide.AddForce(AddRandomScaledSideVelocity(sideForce));
            currentPosBack.AddForce(backForce);
        }

        float AddRandomScaledSideVelocity(float maxSideForce)
        {
            float sideForceToApply = maxSideForce * currentRecoilSettings.upToSideRecoilMultiplierCurve.Evaluate(currentRotUp.value / currentRecoilSettings.recoilUp.maxValue);
            // add a bool and a curve to scale this?
            //if (currentRotUp.value <= 0)
            //   return 0;

            //float sideForceToApply = maxSideForce * (currentRotUp.value / currentRecoilSettings.recoilUp.maxValue);
            //float sideForceToApply = maxSideForce;

            // Rotate left or right partly random.
            if (Random.value > currentRecoilSettings.recoilSideDirectionBalance)
                sideForceToApply = -sideForceToApply;

            return sideForceToApply;
        }

        private void Update()
        {
            if (currentRecoilSettings == null) return;


            // TODO instead of giving all values inside values, set them up once

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

            transformToApplyRecoilTo.localPosition = new Vector3(0, 0, -currentPosBack.value * 0.01f); // We are using centimeters instead of meters as speed.
            transformToApplyRecoilTo.localRotation = Quaternion.Euler(-currentRotUp.value, currentRotSide.value, 0);
        }
    }
}


