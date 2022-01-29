using UnityEngine;
using System;


namespace Benito.ScriptingFoundations.ColliderHelpers
{
    /// <summary>
    /// Attach this Class to a gameobject with a collider you wish to reference. It will allow you to acess the OnTrigger() etc... functions from a class that is not on the same GameObject as the collider.
    /// </summary>
    public class ColliderCallbackHelperC13 : MonoBehaviour
    {
        public Action<Collision> OnColliderEnterDelegate;
        public Action<Collision> OnColliderExitDelegate;


        private void OnCollisionEnter(Collision collision)
        {
            OnColliderEnterDelegate?.Invoke(collision);
        }

        private void OnCollisionExit(Collision collision)
        {
            OnColliderExitDelegate?.Invoke(collision);
        }
    }
}
