using System;
using System.Collections.Generic;
using UnityEngine;

/*

namespace Benito.ScriptingFoundations.Optimisation
{
    /// <summary>
    /// List can change every frame, thats why time deltas for update don make sense
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BudgetedListUpdaterNoTimeDeltasNonAlloc<T> : IBudgetedOperation where T : MonoBehaviour,IBudgetedUpdatableNoTimeDelta
    {
        List<T> listToUpdate;

        public bool Finished { get; private set; }

        public float Progress { get; private set; }

        public float TimeBudget { get; private set; }

        int stoppedAtIndex;
        int listToUpdateCount;

        public BudgetedListUpdaterNoTimeDeltasNonAlloc()
        {
            Finished = true;
        }

        public void Update(float deltaTime)
        {
            float startUpdateTime = Time.realtimeSinceStartup;
            for (int i = stoppedAtIndex; i < listToUpdateCount; i++)
            {
                if (listToUpdate[i] != null)
                {
                    listToUpdate[i].UpdateObject();
                }               

                if (Time.realtimeSinceStartup - startUpdateTime > TimeBudget)
                {
                    // Debug.Log("B stoppedAtIndex: " + stoppedAtIndex + " time: " + (Time.realtimeSinceStartup - startUpdateTime + " budget: " + TimeBudget) + " listToUpdateCount: " + listToUpdateCount);
                    stoppedAtIndex = i + 1;
                    Progress = (1f * i) / (1f * listToUpdateCount);
                    return;
                }
            }

            Finished = true;
        }

        public void Reset(List<T> listToUpdateInput, float timeBudgetInMs)
        {
            TimeBudget = timeBudgetInMs / 1000;

            Finished = false;
            stoppedAtIndex = 0;
            listToUpdate = listToUpdateInput;
            listToUpdateCount = listToUpdate.Count;
        }
    }

}
*/