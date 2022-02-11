using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Chore Object Prefab References", menuName = "ScriptableObjects/ChoreObjectPrefabReferences")]
public class ChoreObjectPrefabReferences : ScriptableObject
{
    public List<ChoreObject> prefabReferences;

    public ChoreObject ChoreTypeToChoreObjectPrefab(ChoreType choreType)
    {
        foreach (ChoreObject choreObject in prefabReferences)
        {
            if (choreObject.ChoreType == choreType)
            {
                return choreObject;
            }
        }

        return null;
    }
}
