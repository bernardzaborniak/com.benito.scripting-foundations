using UnityEngine;
using System;

namespace Benito.ScriptingFoundations.ColliderHelpers
{
    /// <summary>
    /// Attach this Class to a gameobject with a collider you wish to reference. It will allow you to acess the OnTrigger() etc... functions from a class that is not on the same GameObject as the collider.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    [DisallowMultipleComponent]
    public class ColliderCallbackHelperC123T123 : MonoBehaviour
    {
        public Action<Collider> OnTriggerEnterDelegate;
        public Action<Collider> OnTriggerStayDelegate;
        public Action<Collider> OnTriggerExitDelegate;

        public Action<Collision> OnColliderEnterDelegate;
        public Action<Collision> OnColliderStayDelegate;
        public Action<Collision> OnColliderExitDelegate;

        void OnTriggerEnter(Collider other)
        {
            OnTriggerEnterDelegate?.Invoke(other);
        }

        void OnTriggerStay(Collider other)
        {
            OnTriggerStayDelegate?.Invoke(other);
        }

        void OnTriggerExit(Collider other)
        {
            OnTriggerExitDelegate?.Invoke(other);
        }


        private void OnCollisionEnter(Collision collision)
        {
            OnColliderEnterDelegate?.Invoke(collision);
        }

        private void OnCollisionStay(Collision collision)
        {
            OnColliderStayDelegate?.Invoke(collision);
        }

        private void OnCollisionExit(Collision collision)
        {
            OnColliderExitDelegate?.Invoke(collision);
        }
    }
}
