using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Benito.ScriptingFoundations.Managers
{
    public interface ISingletonManager 
    {
        public void InitialiseManager();

        public void UpdateManager();

        public void LateUpdateManager();

        public void FixedUpdateManager();
    }
}
