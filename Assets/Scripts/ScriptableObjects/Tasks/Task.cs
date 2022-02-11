using System;
using UnityEngine;

public abstract class Task : ScriptableObject
{
    public abstract bool IsCompleted();
    public abstract int CurrentObjectsInLevel();
    public abstract int TotalObjectsInLevel();

    public event Action OnTaskProgressed;
    public event Action OnTaskCompleted;

    protected void TaskProgressed()
    {
        OnTaskProgressed?.Invoke();
    }

    protected void TaskCompleted()
    {
        OnTaskCompleted?.Invoke();
    }
}