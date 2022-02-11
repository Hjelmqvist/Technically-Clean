using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(ChoreStation))]
public class LandlineTelephone : MonoBehaviour
{
    private enum PhoneStates
    {
        Waiting,
        Ringing,
        Talking
    }

    [SerializeField] private Timer timer;
    [SerializeField] private float timeBetweenCalls;
    [SerializeField] private float timeToAnswer;
    [SerializeField] private GameObject progressBar;
    [SerializeField] private int scoreOnSuccess = 0;
    [SerializeField] private int scoreOnFail = -30;
    [SerializeField] private float pauseTime = 10f;

    [SerializeField] private UnityEvent OnStartCalling;
    [SerializeField] private UnityEvent OnStopCalling;

    private ChoreStation choreStation;
    private float lastCallTime;
    private float timeStartedCalling;
    private PhoneStates phoneState = PhoneStates.Waiting;

    public static event Action OnHangup;

    public void Success()
    {
        phoneState = PhoneStates.Waiting;
        choreStation.AddScore(scoreOnSuccess);
        if (timer)
        {
            timer.PauseTime( pauseTime );
        }
        lastCallTime = Time.time;
        OnHangup?.Invoke();
        choreStation.canInteract = false;
    }

    private void HangUp()
    {
        if (phoneState != PhoneStates.Talking) return;
        phoneState = PhoneStates.Waiting;
        choreStation.TaskCompleted();
        choreStation.AddScore(scoreOnFail);
        lastCallTime = Time.time;
        OnHangup?.Invoke();
        choreStation.canInteract = false;
    }

    public void PickUp()
    {
        phoneState = PhoneStates.Talking;
        OnStopCalling?.Invoke();
    }

    private void FixedUpdate()
    {
        switch (phoneState)
        {
            case PhoneStates.Waiting:
                bool isCalling = Time.time >= lastCallTime + timeBetweenCalls;
                if (isCalling)
                {
                    phoneState = PhoneStates.Ringing;
                    timeStartedCalling = Time.time;
                    OnStartCalling?.Invoke();
                }

                choreStation.canInteract = isCalling;
                progressBar.SetActive(isCalling);
                break;

            case PhoneStates.Ringing:
                bool failedToAnswer = Time.time > timeStartedCalling + timeToAnswer;
                if (failedToAnswer)
                {
                    choreStation.AddScore(scoreOnFail);
                    lastCallTime = Time.time;
                    OnStopCalling?.Invoke();
                    phoneState = PhoneStates.Waiting;
                }
                break;

            default:
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && phoneState == PhoneStates.Talking)
        {
            choreStation.OnTaskStopped.Invoke();
        }
    }

    private void Awake()
    {
        choreStation = GetComponent<ChoreStation>();
    }

    private void Start()
    {
        lastCallTime = Time.time;
    }
}