using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class PhoneHoverMove : MonoBehaviour , IPointerEnterHandler
{
    [SerializeField] private float moveSpeed = 10000f;
    [SerializeField] private float downHeight = -666f;
    [SerializeField] private float phoneUpTimeOnStart = 10f;
    private const string inspectKey = "Inspect";
    private float upHeight;
    private Coroutine moving;
    private bool isUp = true;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        isUp = true;
        MovePhone();
    }

    private void Update()
    {
        if (Input.GetButtonDown(inspectKey))
        {
            isUp = !isUp;
            MovePhone();
        }
    }

    private void MovePhone()
    {
        float height = isUp ? upHeight : downHeight;
        
        if (moving != null) 
            StopCoroutine(moving);
        moving = StartCoroutine(AnimateMove(height));
    }

    private IEnumerator AnimateMove(float height)
    {
        Vector3 currPos = transform.localPosition;
        while (!Mathf.Approximately(currPos.y, height))
        {
            currPos.y = Mathf.Lerp(currPos.y, height, moveSpeed * Time.deltaTime);
            transform.localPosition = currPos;
            yield return null;
        }

        moving = null;
    }
    
    private IEnumerator GoDownOnStart()
    {
        yield return new WaitForSeconds(phoneUpTimeOnStart);
        if (isUp)
        {
            isUp = false;
            MovePhone();
        }
    }

    private void Start()
    {
        StartCoroutine(GoDownOnStart());
    }

    private void Awake()
    {
        upHeight = transform.localPosition.y;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
