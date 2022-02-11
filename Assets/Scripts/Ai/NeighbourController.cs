using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class NeighbourController : MonoBehaviour
{
    private static readonly int IsWalking = Animator.StringToHash("IsWalking");
    [SerializeField] private ChoreStation window;
    [SerializeField] private Transform[] walkPoints;
    [SerializeField] private Transform stopPoint;
    [SerializeField] private float stopTime = 3f;
    [SerializeField] private float distanceBeforeTuning = 0.33f;
    [SerializeField] private float turnSpeed = .03f;
    [SerializeField] private float walkSpeed = 2.5f;
    [SerializeField] private float failMultiplier = -5f;
    
    private int walkPointIndex;
    private bool stopped;
    private bool stoppedThisRotation;
    private Animator animator;

    public UnityEvent OnHit;

    private void FixedUpdate()
    {
        UpdateWalkPoint();
        Walk();
    }

    private void UpdateWalkPoint()
    {
        if (CompareFloats(transform.position.x, walkPoints[walkPointIndex].position.x, distanceBeforeTuning)
            && CompareFloats(transform.position.z, walkPoints[walkPointIndex].position.z, distanceBeforeTuning))
        {
            walkPointIndex = walkPointIndex == walkPoints.Length - 1 ? 0 : walkPointIndex + 1;
            stoppedThisRotation = false;
        }
        
        if (stoppedThisRotation) return;
        if (CompareFloats(transform.position.x, stopPoint.position.x, distanceBeforeTuning)
            && CompareFloats(transform.position.z, stopPoint.position.z, distanceBeforeTuning))
        {
            stoppedThisRotation = true;
            StartCoroutine(StopWalking());
        }
    }


    private IEnumerator StopWalking()
    {
        stopped = true;
        animator.SetBool(IsWalking, false);
        Vector3 lookTowards = window.transform.position - transform.position;
        float startTime = Time.time;
        while (startTime + stopTime > Time.time)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookTowards, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, turnSpeed);
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
        animator.SetBool(IsWalking, true);
        stopped = false;
    }

    private bool CompareFloats(float a, float b, float t) => Mathf.Abs(a - b) <= t;

    private void Walk()
    {
        if (stopped) return;
        
        Vector3 walkDirection = walkPoints[walkPointIndex].position - transform.position;
        walkDirection.y = 0;
        walkDirection = walkDirection.normalized;
        Quaternion targetRotation = Quaternion.LookRotation(walkDirection, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, turnSpeed);
        transform.position += transform.forward.normalized * walkSpeed * Time.deltaTime;
    }

    public void GroundCollision(Collision other)
    {
        if (other.gameObject.TryGetComponent(out ChoreObject choreObject))
        {
            window.AddScore(choreObject,true);
            Destroy(choreObject.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ChoreObject choreObject))
        {
            window.AddScore(choreObject, true, failMultiplier);
            Destroy(choreObject.gameObject);
            OnHit.Invoke();
        }
    }

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }
}