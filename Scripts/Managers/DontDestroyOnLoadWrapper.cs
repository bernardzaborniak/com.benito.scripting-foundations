using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Benitos.ScriptingFoundations.Managers
{
    public class DontDestroyOnLoadWrapper : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(this);
        }
    }
}
