using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class Tasklist : MonoBehaviour
{
    [SerializeField] Task[] tasks;
    [SerializeField] string[] taskStrings;

    [SerializeField] Color notCompletedColor;
    [SerializeField] Color completedColor;
    [SerializeField] string[] completedRichTextTags;
    [SerializeField] GameObject[] completedMessages;

    [SerializeField] TextMeshProUGUI parentsText;

    [Header( "{0-9} will correspond to the tasks" )]
    [TextArea]
    [SerializeField] string parentsMessage = "You did remember to do the {0} right? And have all the {1} done when we're back!";

    public UnityEvent OnAllTasksCompleted;

    // Subscribe/unsubscribe in Awake/OnDestroy to be subscribed before events are invoked.
    private void Awake()
    {
        foreach (Task task in tasks)
        {
            task.OnTaskProgressed += Task_OnTaskProgressed;
            task.OnTaskCompleted += Task_OnTaskCompleted;
        }
    }

    private void OnDestroy()
    {
        foreach (Task task in tasks)
        {
            task.OnTaskProgressed -= Task_OnTaskProgressed;
            task.OnTaskCompleted -= Task_OnTaskCompleted;
        }
    }

    private void Start()
    {
        UpdateParentsMessage();
    }

    private void Task_OnTaskProgressed()
    {
        UpdateParentsMessage();
    }

    private void Task_OnTaskCompleted()
    {
        UpdateParentsMessage();
        bool allTasksCompleted = true;
        for (int i = 0; i < tasks.Length; i++)
        {
            bool taskCompleted = tasks[i].IsCompleted();

            if (i < completedMessages.Length)
            {
                completedMessages[i].SetActive( taskCompleted );
            }

            if (taskCompleted == false)
            {
                allTasksCompleted = false;
            }
        }
        if (allTasksCompleted)
        {
            OnAllTasksCompleted.Invoke();
        }
    }

    void UpdateParentsMessage()
    {
        string message = parentsMessage;
        List<string> replacingStrings = new List<string>();
        for (int i = 0; i < tasks.Length && i < taskStrings.Length; i++)
        {
            string replacing = GetTaskStringWithTags( i );
            replacingStrings.Add( replacing );
        }
        message = string.Format( message, replacingStrings.ToArray() );
        parentsText.text = message;
    }

    private string GetTaskStringWithTags(int i)
    {
        string taskString = taskStrings[i];
        bool isCompleted = tasks[i].IsCompleted();
        Color color = isCompleted ? completedColor : notCompletedColor;
        taskString = AddRichTextTag( taskString, "color", $"#{ColorUtility.ToHtmlStringRGBA( color )}" );

        if (isCompleted)
        {
            foreach (string richTextTag in completedRichTextTags)
            {
                taskString = AddRichTextTag( taskString, richTextTag );
            }
        }
        return taskString;
    }

    private string AddRichTextTag(string message, string thing, string value = "")
    {
        if (value.Length > 0)
            return $"<{thing}={value}>{message}</{thing}>";
        else
            return $"<{thing}>{message}</{thing}>";
    }
}