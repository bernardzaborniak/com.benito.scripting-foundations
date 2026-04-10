using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using EditorAttributes;

namespace Benito.ScriptingFoundations.States
{
    public class StateMachineUpdatable<T> : IStateMachine<T> where T : class, IStateUpdatable
    {
        [SerializeField, ReadOnly] string currentStateNameDebug;
        public bool printChangeToConsole = true;
        public string stateMachineDebugName = "StateMachine"; // adjust for debug statement


        public T CurrentState { get; protected set; }
        public T PreviousState { get; protected set; }

        public void SetState(T newState)
        {
            if (CurrentState == newState)
                return;

            CurrentState?.OnStateExit();
            PreviousState = CurrentState;
            CurrentState = newState;
            CurrentState?.OnStateEnter();

            currentStateNameDebug = CurrentState?.GetType().Name;
            if (printChangeToConsole) Debug.Log($"[{stateMachineDebugName}] changed state from {PreviousState?.GetType().Name} to: {CurrentState?.GetType().Name}");
        }

        public void UpdateSm()
        {
            CurrentState?.UpdateState();
        }
    }
}
