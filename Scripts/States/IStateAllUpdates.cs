using UnityEngine;

namespace Benito.ScriptingFoundations.States
{
    public interface IStateAllUpdates : IState
    {
        public void UpdateState();
        public void FixedUpdateState();
        public void LateUpdateState();
    }
}
