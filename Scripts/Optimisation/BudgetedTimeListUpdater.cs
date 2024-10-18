using Benito.ScriptingFoundations.Optimisation;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public abstract class BudgetedTimeListUpdater<T> : IBudgetedOperation
{
    public bool Finished { get; private set; }

    public float Progress { get; private set; }

    public float TimeBudget { get; private set; }

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

            if (TimeLimitReached())
            {
                StopIterationForThisFrame(i);
                return;
            }

        }

        stopwatch.Stop();
        Finished = true;
    }

    protected abstract void PerFrameOperation(int index);


    protected bool TimeLimitReached()
    {
        //double elapsedMiliseconds = (stopwatch.ElapsedTicks * 1000.0) / Stopwatch.Frequency;
        return ((stopwatch.ElapsedTicks * 1000.0) / Stopwatch.Frequency) > TimeBudget;
    }

    protected void StopIterationForThisFrame(int index)
    {
        stopwatch.Stop();
        stoppedAtIndex = index;
        Progress = (1f * index) / (1f * listToTraverse.Count);
    }
}


