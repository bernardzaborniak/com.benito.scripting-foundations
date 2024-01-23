using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Benito.ScriptingFoundations.States
{
    public interface IState
    {
        public void OnStateEnter();

        public void OnStateExit();
    }
}
