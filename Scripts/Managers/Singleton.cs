using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Benitos.ScriptingFoundations.Managers
{
    public abstract class Singleton : MonoBehaviour
    {
        public abstract void InitialiseSingleton();

        public abstract void UpdateSingleton();
    }
}
