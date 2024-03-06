using System.Collections.Generic;
using UnityEngine;

namespace Benito.ScriptingFoundations.Optimisation
{
    /// <summary>
    /// List can change every frame, thats why time deltas for update don make sense
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BudgetedListUpdaterNoTimeDeltas<T> : IBudgetedOperation where T : IBudgetedUpdatableNoTimeDelta
    {
        List<T> listToUpdate;

        public bool Finished { get; private set; }

        public float Progress { get; private set; }

        public float TimeBudget { get; private set; }

        int stoppedAtIndex;
        int listToUpdateCount;

        public BudgetedListUpdaterNoTimeDeltas()
        {
            Finished = true;
        }

        public void Update(float deltaTime)
        {
            float startUpdateTime = Time.realtimeSinceStartup;

            for (int i = stoppedAtIndex; i < listToUpdateCount; i++)
            {
                // this delta time is not the correct one? sensing itself should know the delta time? does it actually need it though?
                listToUpdate[i].UpdateObject();

                if (Time.realtimeSinceStartup - startUpdateTime > TimeBudget)
                {
                    stoppedAtIndex = i;
                    Progress = (1f * i) / (1f * listToUpdateCount);
                    return;
                }
            }

            Finished = true;
        }

        public void Reset(List<T> listToUpdate, float timeBudgetInMs)
        {
            TimeBudget = timeBudgetInMs / 1000;

            Finished = false;
            stoppedAtIndex = 0;
            this.listToUpdate = listToUpdate;
            listToUpdateCount = listToUpdate.Count;
        }
    }

}