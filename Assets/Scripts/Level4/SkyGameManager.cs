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

    public bool levelComplete = false;

    void Start()
    {
        Time.timeScale = 1f;
        UpdateUI();
    }

    void Update()
    {
        // ⛔ STOP TIMER AFTER WIN
        if (levelComplete) return;

        timeLeft -= Time.deltaTime;

        if (timerText != null)
            timerText.text = "Time: " + Mathf.Ceil(timeLeft);

        if (timeLeft <= 0)
        {
            Debug.Log("FAILED");

            SceneManager.LoadScene(
                SceneManager.GetActiveScene().name
            );
        }
    }

    public void AddProgress()
    {
        if (levelComplete) return; // prevent double calls

        current++;
        UpdateUI();

        Debug.Log("Progress: " + current + "/" + total);

        if (current >= total)
        {
            EndLevel();
        }
    }

    void EndLevel()
    {
        levelComplete = true;

        Debug.Log("LEVEL COMPLETE TRIGGERED");

        FreezeEnemies();

        if (titleText != null)
        {
            titleText.text = "YOU MASTERED THE SKY";
        }
    }

    void FreezeEnemies()
    {
        GolemAI_Level4[] golems = FindObjectsOfType<GolemAI_Level4>();

        foreach (var g in golems)
        {
            g.enabled = false;
        }

        Debug.Log("Enemies frozen");
    }

    void UpdateUI()
    {
        if (progressText != null)
            progressText.text = current + "/" + total;
    }
}