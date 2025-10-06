using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Benito.ScriptingFoundations.Managers
{
    public abstract class SingletonManagerLocalScene : MonoBehaviour, ISingletonManager
    {
        public abstract void InitialiseManager();

        public abstract void UpdateManager();

        // late and fixed update are optional
        public virtual void LateUpdateManager()
        {

        }

        public virtual void FixedUpdateManager()
        {

        }
    }
}
