using UnityEngine;

[RequireComponent(typeof(ChoreStation))]
public class ChoreStationSetAdder : MonoBehaviour
{
    [SerializeField] ChoreStationSet choreStationSet;
    [SerializeField] bool addOnStart = true;
    ChoreStation choreStation;
    bool isAddedToSet = false;

    private void Awake()
    {
        choreStation = GetComponent<ChoreStation>();
        choreStation.OnTaskCompleted.AddListener( RemoveFromSet );
    }

    private void Start()
    {
        if (addOnStart)
        {
            AddToSet();
        }
    }

    private void OnDestroy()
    {
        RemoveFromSet();
    }

    public void AddToSet()
    {
        if (choreStationSet && isAddedToSet == false)
        {
            choreStationSet.AddObject( choreStation );
            isAddedToSet = true;
        }
    }

    public void RemoveFromSet()
    {
        if (choreStationSet)
        {
            choreStationSet.RemoveObject( choreStation );
            isAddedToSet = false;
        }
    }
}