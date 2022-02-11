using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu( fileName = "New ChoreStationTask", menuName = "ScriptableObjects/ChoreStationTask" )]
public class ChoreStationTask : Task
{
    [SerializeField] ChoreStationSet choreStationSet;
    int totalObjectsInLevel = 0;

    public override bool IsCompleted()
    {
        return choreStationSet.List.Count == 0;
    }

    public int GetObjectsInStationCount()
    {
        int count = 0;
        foreach (ChoreStation station in choreStationSet.List)
        {
            count += station.CurrentContainedCount;
        }
        return count;
    }

    public override int CurrentObjectsInLevel()
    {
        return choreStationSet.List.Count;
    }

    public override int TotalObjectsInLevel()
    {
        return totalObjectsInLevel;
    }

    private void OnEnable()
    {
        totalObjectsInLevel = 0;
        choreStationSet.OnObjectAdded += ChoreStationSet_OnObjectAdded;
        choreStationSet.OnObjectRemoved += SetChanged;
        SceneManager.sceneUnloaded += SceneManager_sceneUnloaded;
    }

    private void OnDisable()
    {
        totalObjectsInLevel = 0;
        choreStationSet.OnObjectAdded -= ChoreStationSet_OnObjectAdded;
        choreStationSet.OnObjectRemoved -= SetChanged;
        SceneManager.sceneUnloaded -= SceneManager_sceneUnloaded;
    }

    private void SceneManager_sceneUnloaded(Scene arg0)
    {
        totalObjectsInLevel = 0;
    }

    private void ChoreStationSet_OnObjectAdded(ChoreStation station)
    {
        totalObjectsInLevel++;
        SetChanged( station );
    }

    private void SetChanged(ChoreStation station)
    {
        TaskProgressed();
        if (IsCompleted())
        {
            TaskCompleted();
        }
    }
}