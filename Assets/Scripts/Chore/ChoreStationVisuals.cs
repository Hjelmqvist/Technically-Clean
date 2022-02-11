using System.Collections.Generic;
using UnityEngine;

public class ChoreStationVisuals : MonoBehaviour
{
    public ChoreType choreType;

    private MeshRenderer[] toShow;

    public void ShowChoreObject(ChoreType inputType, int index)
    {
        if (inputType != choreType || index >= toShow.Length) return;

        toShow[index].enabled = true;
    }

    public void HideChoreObjects()
    {
        foreach (MeshRenderer meshRenderer in toShow)
        {
            meshRenderer.enabled = false;
        }
    }

    private void Start()
    {
        List<MeshRenderer> tempToShow = new List<MeshRenderer>();
        foreach (MeshRenderer mr in transform.GetComponentsInChildren<MeshRenderer>())
        {
            tempToShow.Add(mr);
        }

        toShow = tempToShow.ToArray();
    }

    private void OnEnable()
    {
        ChoreStation station = GetComponentInParent<ChoreStation>();
        if (station)
        {
            station.OnShowObject += ShowChoreObject;
            station.OnHideObject += HideChoreObjects;
        }
    }

    private void OnDisable()
    {
        ChoreStation station = GetComponentInParent<ChoreStation>();
        if (station)
        {
            station.OnShowObject -= ShowChoreObject;
            station.OnHideObject -= HideChoreObjects;
        }
    }
}
