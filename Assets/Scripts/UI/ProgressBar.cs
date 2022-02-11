using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] ChoreStation choreStation;
    [SerializeField] GameObject progressBar;
    [SerializeField] Image progressFill;

    private void OnEnable()
    {
        choreStation.OnInteractableFull.AddListener( ShowProgressBar );
        choreStation.OnTaskProgressed.AddListener( ProgressChanged );
        choreStation.OnTaskCompleted.AddListener( HideProgressBar );
    }

    private void OnDisable()
    {
        choreStation.OnInteractableFull.RemoveListener( ShowProgressBar );
        choreStation.OnTaskProgressed.RemoveListener( ProgressChanged );
        choreStation.OnTaskCompleted.RemoveListener( HideProgressBar );
    }

    /// <summary>
    /// From 0 to 1
    /// </summary>
    public void ProgressChanged(float percentage)
    {
        progressFill.fillAmount = percentage;
        if (percentage > 0)
        {
            ShowProgressBar();
        }
        else
        {
            HideProgressBar();
        }
    }

    private void ShowProgressBar()
    {
        progressBar.SetActive( true );
    }

    private void HideProgressBar()
    {
        progressFill.fillAmount = 0f;
        progressBar.SetActive( false );
    }
}