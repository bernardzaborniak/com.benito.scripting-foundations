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
            /*Debug.Log("update budgeted operation");
            Debug.Log("startUpdateTime: " + startUpdateTime);
            Debug.Log("stoppedAtIndex: " + stoppedAtIndex);
            Debug.Log("listToUpdateCount: " + listToUpdateCount);
            Debug.Log("listToUpdate: " + listToUpdate);
            Debug.Log("listToUpdate.Count: " + listToUpdate.Count);*/

            for (int i = stoppedAtIndex; i < listToUpdateCount; i++)
            {
                // this delta time is not the correct one? sensing itself should know the delta time? does it actually need it though?
                //Debug.Log("A index: " + i);
                listToUpdate[i]?.UpdateObject();

                if (i>0 && Time.realtimeSinceStartup - startUpdateTime > TimeBudget)
                {
                    Debug.Log("B stoppedAtIndex: " + stoppedAtIndex + " time: " + (Time.realtimeSinceStartup - startUpdateTime + " budget: " + TimeBudget) + " listToUpdateCount: " + listToUpdateCount);
                    stoppedAtIndex = i;
                    Progress = (1f * i) / (1f * listToUpdateCount);
                    return;
                }
            }
            /*Debug.Log("C arrived at finishing with index at : " + stoppedAtIndex);
            Debug.Log(" list count: " + listToUpdateCount);
            Debug.Log(" listToUpdate.Count: " + listToUpdate.Count);*/
            Finished = true;
        }

        public void Reset(List<T> listToUpdateInput, float timeBudgetInMs)
        {
            TimeBudget = timeBudgetInMs / 1000;

            Finished = false;
            stoppedAtIndex = 0;
            listToUpdate = new List<T>(listToUpdateInput);
            listToUpdateCount = listToUpdate.Count;

            Debug.Log("[BudgetedOperation] Reset");
            Debug.Log("list input as parameter: " + listToUpdateInput);
            Debug.Log("list input as parameter.count: " + listToUpdateInput.Count);
            Debug.Log("list copied: " + listToUpdate);
            Debug.Log("list copied.count: " + listToUpdate.Count);
            Debug.Log("listToUpdateCount: " + listToUpdateCount);
        }
    }

}