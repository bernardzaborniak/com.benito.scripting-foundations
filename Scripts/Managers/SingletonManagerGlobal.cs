using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Benito.ScriptingFoundations.Managers
{
    public abstract class SingletonManagerGlobal : MonoBehaviour,ISingletonManager
    {
        public abstract void InitialiseManager();

        public abstract void UpdateManager();

        public virtual void LateUpdateManager()
        {

        }

    }
}
