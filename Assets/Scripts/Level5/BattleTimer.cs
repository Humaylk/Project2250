using UnityEngine;

// Munadir: Countdown timer for the boss fight
// Munadir: When time runs out, player loses the level
// Munadir: Displayed on screen at all times during boss fight
public class BattleTimer : MonoBehaviour
{
    public float timeRemaining;
    public bool isRunning = false;

    private float initialDuration;

    public void StartTimer(float duration)
    {
        initialDuration = duration;
        timeRemaining = duration;
        isRunning = true;
        Debug.Log("Battle timer started: " + duration + " seconds.");
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public void ResetTimer()
    {
        timeRemaining = initialDuration;
        isRunning = false;
    }

    public bool IsTimeUp()
    {
        return timeRemaining <= 0f;
    }

    void Update()
    {
        if (!isRunning) return;

        timeRemaining -= Time.deltaTime;
        timeRemaining = Mathf.Max(timeRemaining, 0f);

        if (timeRemaining <= 0f)
        {
            isRunning = false;
            Debug.Log("Battle timer expired!");
        }
    }

    // Format as MM:SS for display
    public string GetFormattedTime()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}