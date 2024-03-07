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
                Debug.Log("A index: " + i);
                listToUpdate[i]?.UpdateObject();

                if (Time.realtimeSinceStartup - startUpdateTime > TimeBudget)
                {
                    Debug.Log("B stoppedAtIndex: " + stoppedAtIndex);
                    stoppedAtIndex = i;
                    Progress = (1f * i) / (1f * listToUpdateCount);
                    return;
                }
            }
            Debug.Log("C arrived at finishing with index at : " + stoppedAtIndex + " list count: " + listToUpdateCount + " or " + listToUpdate.Count);
            Finished = true;
        }

        public void Reset(List<T> listToUpdate, float timeBudgetInMs)
        {
            TimeBudget = timeBudgetInMs / 1000;

            Finished = false;
            stoppedAtIndex = 0;
            this.listToUpdate = new List<T>(listToUpdate);
            Debug.Log("[BudgetedOperation] Reset");
            Debug.Log("list input as parameter: " + listToUpdate);
            Debug.Log("list input as parameter.count: " + listToUpdate.Count);
            Debug.Log("list copied: " + this.listToUpdate);
            Debug.Log("list copied.count: " + this.listToUpdate.Count);
            listToUpdateCount = listToUpdate.Count;
        }
    }

}