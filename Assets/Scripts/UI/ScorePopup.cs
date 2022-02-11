using System.Collections;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI), typeof(RectTransform))]
public class ScorePopup : MonoBehaviour
{
    private TextMeshProUGUI textComponent;
    private RectTransform rectTransform;
    private Coroutine animation;

    public void PopupAnimation(int score, Vector3 startPosition, float animationDuration, float moveSpeed,
        float scaleSpeed, float startScale)
    {
        if (animation != null) StopCoroutine(animation);
        animation = StartCoroutine(StartAnimation(score, startPosition, animationDuration, moveSpeed, scaleSpeed, startScale));
    }

    private IEnumerator StartAnimation(int score, Vector3 startPosition, float animationDuration, float moveSpeed,
        float endScale, float startScale)
    {
        textComponent.text = score.ToString();
        rectTransform.localPosition = startPosition;
        rectTransform.localScale = new Vector3(startScale, startScale, 1);
        float moveEachTick = moveSpeed * Time.fixedDeltaTime;
        float scaleEachTick = (endScale - startScale) * Time.fixedDeltaTime;

        float timePassed = 0;
        while (timePassed < animationDuration)
        {
            float movedYPosition = rectTransform.localPosition.y + moveEachTick;
            rectTransform.localPosition = new Vector3(0, movedYPosition, 0);
            float newScaledValue = rectTransform.localScale.x + scaleEachTick;
            rectTransform.localScale = new Vector3(newScaledValue, newScaledValue, 1);
            timePassed += Time.fixedDeltaTime;
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }

        gameObject.SetActive(false);
    }

    private void Awake()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
        rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }
}