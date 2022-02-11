using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class PauseTimePopup : MonoBehaviour
{
    [SerializeField] private Timer timer;
    
    private TextMeshProUGUI tmp;
    private Coroutine FadeText;

    private void OnTimePause(float waitFor)
    {
        if (FadeText != null)
            StopCoroutine(FadeText);
        FadeText = StartCoroutine(AnimateTextFade(waitFor));
    }

    private IEnumerator AnimateTextFade(float waitFor)
    {
        Color currColor = tmp.color;
        currColor.a = 1f;
        while (currColor.a > 0f)
        {
            currColor.a -= Time.deltaTime / waitFor;
            tmp.color = currColor;
            yield return null;
        }
        FadeText = null;
    }

    private void OnEnable()
    {
        timer.OnTimePaused += OnTimePause;
    }

    private void OnDisable()
    {
        timer.OnTimePaused -= OnTimePause;
    }
    
    private void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
    }
}
