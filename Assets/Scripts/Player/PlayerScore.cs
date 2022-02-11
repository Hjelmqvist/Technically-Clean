using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerScore : MonoBehaviour
{
    [SerializeField]private int totalScore;

    // To display scores per choretype
    Dictionary<ChoreType, int> choreScores = new Dictionary<ChoreType, int>();

    public UnityEvent<int> OnChange;
    public UnityEvent OnPositiveScore;
    public UnityEvent OnNegativeScore;

    public int Score => totalScore;

    private void OnEnable()
    {
        ChoreStation.OnScored += ChoreInteractableOnOnScored;
    }

    private void OnDisable()
    {
        ChoreStation.OnScored -= ChoreInteractableOnOnScored;
    }

    private void ChoreInteractableOnOnScored(ChoreType choreType, int toAdd)
    {
        if (choreScores.ContainsKey(choreType))
        {
            choreScores[choreType] = Mathf.Clamp(choreScores[choreType] + toAdd, 0, int.MaxValue );
        }
        else
        {
            int newToAdd = toAdd;
            if (toAdd < 0) newToAdd = 0;
            choreScores.Add(choreType, newToAdd);
        }

        totalScore = Mathf.Clamp(totalScore + toAdd, 0, int.MaxValue);
        OnChange.Invoke(totalScore);
        if (toAdd > 0) OnPositiveScore.Invoke();
        else if (toAdd < 0) OnNegativeScore.Invoke();
    }
}