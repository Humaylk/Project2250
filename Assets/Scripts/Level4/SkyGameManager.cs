using UnityEngine;
using TMPro;

using UnityEngine.SceneManagement;

public class SkyGameManager : MonoBehaviour
{
    public int total = 3;
    private int current = 0;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI progressText;
    public TextMeshProUGUI timerText;
    public float timeLeft = 20f;

    void Start()
    {
        Time.timeScale = 1f; // reset in case previous run froze game
        UpdateUI();
    }

    void Update()
    {
        timeLeft -= Time.deltaTime;

        timerText.text = "Time: " + Mathf.Ceil(timeLeft);

        if (timeLeft <= 0)
        {
            Debug.Log("FAILED");
            // restart scene
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
            );
        }
    }

    public void AddProgress()
    {
        current++;
        UpdateUI();

        if (current >= total)
        {
            EndLevel();
        }
    }
    void EndLevel()
    {
        Debug.Log("LEVEL COMPLETE");

        // stop time (freezes everything: player + golems)
        Time.timeScale = 0f;

        // optional: show message
        if (titleText != null)
        {
            titleText.text = "YOU MASTERED THE SKY";
        }
    }

    void UpdateUI()
    {
        progressText.text = current + "/" + total;
    }
}