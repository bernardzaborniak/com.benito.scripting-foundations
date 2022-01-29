using UnityEngine;
using System;

namespace Benito.ScriptingFoundations.ColliderHelpers
{
    /// <summary>
    /// Attach this Class to a gameobject with a collider you wish to reference. It will allow you to acess the OnTrigger() etc... functions from a class that is not on the same GameObject as the collider.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class ColliderCallbackHelperT3 : MonoBehaviour
    {
        public Action<Collider> OnTriggerExitDelegate;


        void OnTriggerExit(Collider other)
        {
            OnTriggerExitDelegate?.Invoke(other);
        }
    }
}
