using UnityEngine;
using System;

public class CountdownTimer : MonoBehaviour
{
    public float timeRemaining;
    public bool isRunning = false;

    public event Action OnTimeUp;

    private float _initialDuration;

    public void StartTimer(float duration)
    {
        _initialDuration = duration;
        timeRemaining = duration;
        isRunning = true;
        Debug.Log("Timer started: " + duration + " seconds.");
    }

    public void StopTimer()
    {
        isRunning = false;
        Debug.Log("Timer paused.");
    }

    public void ResetTimer()
    {
        timeRemaining = _initialDuration;
        isRunning = false;
        Debug.Log("Timer reset.");
    }

    public bool IsTimeUp()
    {
        return timeRemaining <= 0f;
    }

    void Update()
    {
        if (!isRunning) return;
        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            isRunning = false;
            Debug.Log("Time is up!");
            OnTimeUp?.Invoke();
        }
    }
}