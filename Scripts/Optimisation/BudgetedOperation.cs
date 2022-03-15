using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Benito.ScriptingFoundations.Optimisation
{
    /// <summary>
    /// Operation that has a fixed ms budget per frame.
    /// </summary>
    public interface IBudgetedOperation 
    {
        public bool Finished {  get; }
        public float Progress { get; }
        public float TimeBudget { get; }

        public void Update(float deltaTime);
    }

}


