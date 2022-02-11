using System;
using UnityEngine;
using TMPro;
using FMODUnity;

public class EndMenu : MonoBehaviour
{
    [Serializable]
    public struct ScoreToGrade
    {
        public int score;
        public char grade;
        public GameObject endUIState;
    }

    [SerializeField] private PlayerScore playerScore;
    [SerializeField] private ScoreText score;
    [SerializeField] private ScoreToGrade[] states;
    [SerializeField] private TextMeshProUGUI gradeText;
    [SerializeField] private TextMeshProUGUI percentageText;
    [SerializeField] private TextMeshProUGUI scoreText;

    public static Action OnGameEnd;

    private void Start()
    {
        RuntimeManager.GetBus( "bus:/" ).stopAllEvents( FMOD.Studio.STOP_MODE.IMMEDIATE );
        ScoreToGrade state = CheckScore();
        gradeText.text = state.grade.ToString();
        percentageText.text = $"{(int)score.GetPercentage()}%";
        scoreText.text = playerScore.Score.ToString();
        state.endUIState.SetActive( true );
        OnGameEnd?.Invoke();
    }

    public ScoreToGrade CheckScore()
    {
        int highestGradeScore = int.MinValue;
        ScoreToGrade grade = new ScoreToGrade();
        foreach (ScoreToGrade state in states)
        {
            if (state.score <= playerScore.Score && state.score > highestGradeScore)
            {
                highestGradeScore = state.score;
                grade = state;
            }
        }
        return grade;
    }
}