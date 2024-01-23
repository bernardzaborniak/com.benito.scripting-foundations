using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Benito.ScriptingFoundations.States
{
    public interface IStateUpdatable: IState
    {
        public void UpdateState();
    }
}
