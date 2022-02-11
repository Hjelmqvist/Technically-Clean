using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using FMODUnity;

public class Timer : MonoBehaviour
{
    [Serializable]
    public struct FloatAndColor
    {
        public float time;
        public Color colorAtTime;
    }
    
    private float timeValue;

    public float startTimeValue = 180;
    public bool timerIsRunning = false;
    
    public UnityEvent OnTimerStarted;
    public UnityEvent OnTimerStopped;

    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] List<FloatAndColor> colorList = new List<FloatAndColor>();
    [SerializeField] private Slider slider;
    [SerializeField] private Image fillImage;

    private FMOD.Studio.EventInstance instance;
    public EventReference fmodEvent;
    private Coroutine pause;
    public Action<float> OnTimePaused;

    public void PauseTime(float pauseForSeconds)
    {
        if (pause != null)
            StopCoroutine(pause);
        pause = StartCoroutine(Pause(pauseForSeconds));
        OnTimePaused.Invoke(pauseForSeconds);
    }

    private IEnumerator Pause(float time)
    {
        timerIsRunning = false;
        yield return new WaitForSeconds(time);
        timerIsRunning = true;
    }

    private void Start()
    {
        timeValue = startTimeValue;
        timerIsRunning = true;
        OnTimerStarted.Invoke();
        instance = RuntimeManager.CreateInstance(fmodEvent);
        instance.start();
        RuntimeManager.StudioSystem.setParameterByName("TimeCounter(100%-0%)", 1f);
    }

    void Update()
    {
        if (timerIsRunning)
        {
            if (timeValue > 0)
            {
                timeValue -= Time.deltaTime;
                float percentage = Mathf.InverseLerp (startTimeValue, 0, timeValue);
                slider.value = percentage;
                float remainingPercentage = 1 - percentage;
                RuntimeManager.StudioSystem.setParameterByName("TimeCounter(100%-0%)", remainingPercentage);
                foreach (FloatAndColor floatAndColor in colorList)
                {
                    if (remainingPercentage <= floatAndColor.time)
                    {
                        timeText.color = floatAndColor.colorAtTime;
                        fillImage.color = floatAndColor.colorAtTime;
                    }
                }
            }
            else
            {
                timeValue = 0;
                timerIsRunning = false;
                OnTimerStopped.Invoke();
            }
        }  
    }

    void DisplayTime(float timeToDisplay)
    {
        if (timeToDisplay < 0)
        {
            timeToDisplay = 0;
        }
        else if (timeToDisplay > 0)
        {
            timeToDisplay += 1;
        }

        float minutes = Mathf.FloorToInt( timeToDisplay / 60 );
        float seconds = Mathf.FloorToInt( timeToDisplay % 60 );
        timeText.text = $"{minutes:00}:{seconds:00}";
    }
}