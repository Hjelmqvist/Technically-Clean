using UnityEngine;

public class VisualUpdater : MonoBehaviour
{
    [SerializeField] int progressNumberToAdvance;
    [SerializeField] GameObject startVisual;
    [SerializeField] GameObject[] progressVisuals;

    int progress = 0;
    GameObject current;
    int currentIndex;

    private void Awake()
    {
        current = startVisual;
        currentIndex = -1;
    }

    public void Progress()
    {
        progress++;
        if (progress >= progressNumberToAdvance && currentIndex + 1 < progressVisuals.Length)
        {
            progress = 0;
            current.SetActive( false );
            currentIndex = currentIndex + 1;
            current = progressVisuals[currentIndex];
            current.SetActive( true );
        }
    }
}