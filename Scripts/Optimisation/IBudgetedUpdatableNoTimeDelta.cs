using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Benito.ScriptingFoundations.Optimisation
{
    public interface IBudgetedUpdatableNoTimeDelta
    {
        public float LastUpdateTime { get; }

        public void UpdateObject();
    }
}
