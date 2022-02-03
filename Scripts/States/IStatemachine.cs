using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Benito.ScriptingFoundations.States
{
    public interface IStatemachine<T> where T : IState
    {
        public T CurrentState { get; }

        public void SetState(T newState);
    }
}
