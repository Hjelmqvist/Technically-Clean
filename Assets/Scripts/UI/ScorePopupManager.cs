using UnityEngine;

public class ScorePopupManager : MonoBehaviour
{
    [SerializeField] private ChoreStation choreStation;
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private float animationDuration;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float endScale = 0f;
    [SerializeField] private float minScale = .5f;
    [SerializeField] private float scoreForMinScale = -25f;
    [SerializeField] private float maxScale = 2f;
    [SerializeField] private float scoreForMaxScale = 100f;
    [SerializeField] private float scoreForScaleOfOne = 0f;

    private ScorePopup[] scorePopups;
    private int index;
 
    private void ScoreVisual(int score)
    {
        float startScale;
        if (score >= scoreForScaleOfOne)
        {
            startScale = Mathf.InverseLerp(scoreForScaleOfOne, scoreForMaxScale, score);
            startScale = Mathf.Lerp(1f, maxScale, startScale);
        }
        else
        {
            startScale = Mathf.InverseLerp(scoreForMinScale, scoreForScaleOfOne, score);
            startScale = Mathf.Lerp(minScale, 1f, startScale);
        }
        
        ScorePopup displayMe = scorePopups[index];
        displayMe.gameObject.SetActive(true);
        displayMe.PopupAnimation(score, startPosition, animationDuration, moveSpeed, endScale, startScale);
        index++;
        if (index >= scorePopups.Length) index = 0;
    }
    

    private void Awake()
    {
        scorePopups = GetComponentsInChildren<ScorePopup>();
    }

    private void OnEnable()
    {
        choreStation.OnScoredIndividual += ScoreVisual;
    }

    private void OnDisable()
    {
        choreStation.OnScoredIndividual -= ScoreVisual;
    }
}