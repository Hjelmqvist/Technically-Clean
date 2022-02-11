using System;
using System.Collections.Generic;
using UnityEngine;

public class RuntimeSetBase<T> : ScriptableObject
{
    List<T> runtimeSet = new List<T>();

    public List<T> List => runtimeSet;

    public event Action<T> OnObjectAdded;
    public event Action<T> OnObjectRemoved;

    public virtual void AddObject(T objectToAdd)
    {
        if (runtimeSet.Contains(objectToAdd) == false)
        {
            runtimeSet.Add( objectToAdd );
            OnObjectAdded?.Invoke( objectToAdd );
        }
    }

    public virtual void RemoveObject(T objectToRemove)
    {
        runtimeSet.Remove( objectToRemove );
        OnObjectRemoved?.Invoke( objectToRemove );
    }
}