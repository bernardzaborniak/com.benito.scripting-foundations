using Benito.ScriptingFoundations.NaughtyAttributes;
using UnityEngine;

namespace Benito.ScriptingFoundations.States
{
    public class StateMachineAllUpdates<T> : IStateMachine<T> where T : class, IStateAllUpdates
    {
        [ReadOnly][SerializeField] string currentStateNameDebug;
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
            if(printChangeToConsole) Debug.Log($"[{stateMachineDebugName}] changed state from {PreviousState?.GetType().Name} to: {CurrentState?.GetType().Name}");
        }

        public void UpdateSm()
        {
            CurrentState?.UpdateState();
        }

        public void LateUpdateSm()
        {
            CurrentState?.LateUpdateState();
        }

        public void FixedUpdateSm()
        {
            CurrentState?.FixedUpdateState();
        }
    }
}