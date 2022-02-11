using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreText : MonoBehaviour
{
    [SerializeField] private PlayerScore playerScore;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI percentageText;
    [SerializeField] private List<Task> dirtyTasksInLevel = new List<Task>();
    [SerializeField] private List<Task> cleanTasksInLevel = new List<Task>();
    [SerializeField] private List<ChoreStationTask> stationTasks = new List<ChoreStationTask>();

    private int totalDirtyObjectsInLevel = 0;
    private int currentDirtyObjectsInLevel = 0;
    private int totalCleanObjectsInLevel = 0;
    private int currentCleanObjectsInLevel = 0;

    int lastMissing = 0;

    private void Awake()
    {
        foreach (Task task in dirtyTasksInLevel)
        {
            task.OnTaskProgressed += CalculateDirtyObjects;
            task.OnTaskCompleted += CalculateDirtyObjects;
        }

        foreach (Task task in cleanTasksInLevel)
        {
            task.OnTaskProgressed += CalculateCleanObjects;
            task.OnTaskCompleted += CalculateCleanObjects;
        }
    }

    private void CalculateDirtyObjects()
    {
        totalDirtyObjectsInLevel = 0;
        currentDirtyObjectsInLevel = 0;

        foreach (Task task in dirtyTasksInLevel)
        {
            totalDirtyObjectsInLevel += task.TotalObjectsInLevel();
            currentDirtyObjectsInLevel += task.CurrentObjectsInLevel();
        }
        UpdateScore( playerScore.Score );
    }

    private void CalculateCleanObjects()
    {
        totalCleanObjectsInLevel = 0;
        currentCleanObjectsInLevel = 0;

        foreach (Task task in cleanTasksInLevel)
        {
            totalCleanObjectsInLevel += task.TotalObjectsInLevel();
            currentCleanObjectsInLevel += task.CurrentObjectsInLevel();
        }
        UpdateScore( playerScore.Score );
    }

    private void OnDestroy()
    {
        foreach (Task task in dirtyTasksInLevel)
        {
            task.OnTaskProgressed -= CalculateDirtyObjects;
            task.OnTaskCompleted -= CalculateDirtyObjects;
        }

        foreach (Task task in cleanTasksInLevel)
        {
            task.OnTaskProgressed -= CalculateCleanObjects;
            task.OnTaskCompleted -= CalculateCleanObjects;
        }
    }

    public void UpdateScore(int score)
    {
        float percentage = CalculatePercentage();

        scoreText.text = score.ToString();
        percentageText.text = Mathf.RoundToInt( percentage ) + "%";
    }

    private float CalculatePercentage()
    {
        int total = totalDirtyObjectsInLevel;
        int cleaned = totalCleanObjectsInLevel - currentCleanObjectsInLevel;
        int missing = totalDirtyObjectsInLevel - currentDirtyObjectsInLevel - totalCleanObjectsInLevel;

        int objectsInStationsCount = 0;
        foreach (ChoreStationTask task in stationTasks)
        {
            objectsInStationsCount += task.GetObjectsInStationCount();
        }
        missing -= objectsInStationsCount;

        float percentage = (float)(cleaned + missing) / total; // 0-1

        if (cleaned == 0 && missing == 0)
        {
            percentage = 0;
        }

        percentage *= 100f; // To 0-100%
        percentage = Mathf.Clamp( percentage, 0, 100 );
        return percentage;
    }

    public float GetPercentage()
    {
        CalculateDirtyObjects();
        CalculateCleanObjects();
        return CalculatePercentage();
    }
}