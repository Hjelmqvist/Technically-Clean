using UnityEngine;

[CreateAssetMenu( fileName = "New MultiTask", menuName = "ScriptableObjects/MultiTask" )]
public class MultiTask : Task
{
    [SerializeField] Task[] tasks;

    public override bool IsCompleted()
    {
        bool completed = true;
        foreach (Task task in tasks)
        {
            if (task.IsCompleted() == false)
            {
                completed = false;
                break;
            }
        }
        return completed;
    }

    public override int CurrentObjectsInLevel()
    {
        int current = 0;
        foreach (Task task in tasks)
        {
            current = task.CurrentObjectsInLevel();
        }
        return current;
    }

    public override int TotalObjectsInLevel()
    {
        int total = 0;
        foreach (Task task in tasks)
        {
            total = task.TotalObjectsInLevel();
        }
        return total;
    }

    private void OnEnable()
    {
        foreach (Task task in tasks)
        {
            task.OnTaskProgressed += TaskChanged;
            task.OnTaskCompleted += TaskChanged;
        }
    }

    private void TaskChanged()
    {
        TaskProgressed();
        if (IsCompleted())
        {
            TaskCompleted();
        }
    }
}