using UnityEngine;
using Benito.ScriptingFoundations.Managers;
using System.Collections.Generic;


namespace Benito.ScriptingFoundations.CameraHelpers
{
    public class BillboardSimpleManager : SingletonManagerLocalScene
    {
        public Camera currentCamera; // Reference to the main camera

        public enum RotationType
        {
            PointBillboarding,
            ViewAlignedBillboarding          
        }

        public enum VerticalRotationType
        {
            Full,
            PartialY,
            NoY
        }

        HashSet<BillboardSimple> billboards = new HashSet<BillboardSimple>();

        public override void InitialiseManager()
        {
            currentCamera = Camera.main;
        }

        public override void UpdateManager()
        {

            if (billboards.Count == 0) return;

            Vector3 camPos = currentCamera.transform.position;
            Vector3 camForward = currentCamera.transform.forward;

            Vector3 direction = Vector3.zero;

            foreach (BillboardSimple billboard in billboards)
            {
                RotationType rotationType = billboard.rotationType;
                VerticalRotationType verticalRotationType = billboard.verticalRotationType;
     
                if (rotationType == RotationType.PointBillboarding) direction = camPos - billboard.transform.position;
                else if (rotationType == RotationType.ViewAlignedBillboarding) direction = -camForward;

                if (verticalRotationType == VerticalRotationType.NoY) direction.y = 0;
                else if (verticalRotationType == VerticalRotationType.PartialY) direction.y *= billboard.partialYRotationAmount;

                billboard.transform.rotation = Quaternion.LookRotation(direction);

            }
        }

        public void RegisterBillboard(BillboardSimple billboard) { billboards.Add(billboard); }

        public void UnregisterBillboard(BillboardSimple billboard) { billboards.Remove(billboard); }
    }
}
