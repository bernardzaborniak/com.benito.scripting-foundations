using UnityEngine;
using System;

namespace Benito.ScriptingFoundations.ColliderHelpers
{
    /// <summary>
    /// Attach this Class to a gameobject with a collider you wish to reference. It will allow you to acess the OnTrigger() etc... functions from a class that is not on the same GameObject as the collider.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    [DisallowMultipleComponent]
    public class ColliderCallbackHelperT1 : MonoBehaviour
    {
        public Action<Collider> OnTriggerEnterDelegate;


        void OnTriggerEnter(Collider other)
        {
            OnTriggerEnterDelegate?.Invoke(other);
        }
    }
}
