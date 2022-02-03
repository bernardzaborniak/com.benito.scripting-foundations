using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Benito.ScriptingFoundations.States
{
    public class SimpleStatemachine<T> : IStatemachine<T> where T : class, IState
    {
        public T CurrentState { get; protected set; }

        public void SetState(T newState)
        {
            if (CurrentState == newState)
                return;

            CurrentState?.OnStateExit();
            CurrentState = newState;
            CurrentState?.OnStateEnter();
        }

        public void Update()
        {
            CurrentState?.UpdateState();
        }
    }
}
