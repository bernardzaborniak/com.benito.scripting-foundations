using Benito.ScriptingFoundations.Optimisation;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public abstract class BudgetedTimeListUpdater<T> : IBudgetedOperation
{
    public bool Finished { get; protected set; }

    public float Progress { get; protected set; }

    // Watch out -  if you switch the budget very low, it first needs to finish one whole iteraion on this low budget,
    // probably only one item per frame, before starting a new iteration with the new budget
    public float TimeBudget { get; protected set; }

    protected int stoppedAtIndex;
    protected List<T> listToTraverse;

    protected Stopwatch stopwatch;

    public BudgetedTimeListUpdater()
    {
        Finished = true;
        stopwatch = new Stopwatch();
    }

    public void Restart(List<T> listToTraverse, float timeBudgetInMs)
    {
        this.listToTraverse = listToTraverse;
        stoppedAtIndex = 0;
        TimeBudget = timeBudgetInMs;
        Finished = false;

    }

    public void Update(float deltaTime)
    {
        stopwatch.Restart();

        for (int i = stoppedAtIndex; i < listToTraverse.Count; i++)
        {
            PerFrameOperation(i);

            if (GetElapsedMiliseconds()>TimeBudget)
            {
                StopIterationForThisFrame(i);
                return;
            }
        }

        stopwatch.Stop();
        Finished = true;
    }

    protected abstract void PerFrameOperation(int index);

    protected void StopIterationForThisFrame(int index)
    {
        stopwatch.Stop();
        stoppedAtIndex = index;
        Progress = (1f * index) / (1f * listToTraverse.Count);
    }

    /// <summary>
    /// Dont use often, calling this method means its being calculated twice
    /// </summary>
    /// <returns></returns>
    protected double GetElapsedMiliseconds()
    {
        return (stopwatch.ElapsedTicks * 1000.0) / Stopwatch.Frequency;
    }
}


