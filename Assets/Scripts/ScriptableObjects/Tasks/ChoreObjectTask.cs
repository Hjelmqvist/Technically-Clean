using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu( fileName = "New ChoreObjectTask", menuName = "ScriptableObjects/ChoreObjectTask" )]
public class ChoreObjectTask : Task
{
    [SerializeField] ChoreObjectSet choreObjectSet;
    int totalObjectsInLevel = 0;

    public override bool IsCompleted()
    {
        return choreObjectSet.List.Count == 0;
    }

    public override int CurrentObjectsInLevel()
    {
        int count = 0;
        foreach (ChoreObject choreObject in choreObjectSet.List)
        {
            count += choreObject.Amount;
        }
        return count;
    }

    public override int TotalObjectsInLevel()
    {
        return totalObjectsInLevel;
    }

    private void OnEnable()
    {
        totalObjectsInLevel = 0;
        choreObjectSet.OnObjectAdded += ChoreObjectSet_OnObjectAdded;
        choreObjectSet.OnObjectRemoved += SetChanged;
        SceneManager.sceneUnloaded += SceneManager_sceneUnloaded;
    }

    private void OnDisable()
    {
        totalObjectsInLevel = 0;
        choreObjectSet.OnObjectAdded -= ChoreObjectSet_OnObjectAdded;
        choreObjectSet.OnObjectRemoved -= SetChanged;
        SceneManager.sceneUnloaded -= SceneManager_sceneUnloaded;
    }

    private void SceneManager_sceneUnloaded(Scene arg0)
    {
        totalObjectsInLevel = 0;
    }

    private void ChoreObjectSet_OnObjectAdded(ChoreObject choreObject)
    {
        totalObjectsInLevel += choreObject.Amount;
        SetChanged( choreObject );
    }

    private void SetChanged(ChoreObject choreObject)
    {
        TaskProgressed();
        if (IsCompleted())
        {
            TaskCompleted();
        }
    }
}