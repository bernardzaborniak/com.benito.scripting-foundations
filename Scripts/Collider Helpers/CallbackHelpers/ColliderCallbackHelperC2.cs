using UnityEngine;
using System;

namespace Benito.ScriptingFoundations.ColliderHelpers
{
    /// <summary>
    /// Attach this Class to a gameobject with a collider you wish to reference. It will allow you to acess the OnTrigger() etc... functions from a class that is not on the same GameObject as the collider.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    [DisallowMultipleComponent]
    public class ColliderCallbackHelperC2 : MonoBehaviour
    {
        public Action<Collision> OnColliderStayDelegate;


        private void OnCollisionStay(Collision collision)
        {
            OnColliderStayDelegate?.Invoke(collision);
        }
    }
}
