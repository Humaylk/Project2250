using UnityEngine;
using TMPro;

public class PlayerHealth_Level4 : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    public TextMeshProUGUI healthText;
    public DeathScreen deathScreen;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateUI();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        UpdateUI();

        DamageFlashCanvas.Instance?.Flash();

        if (currentHealth <= 0)
        {
            if (deathScreen != null)
                deathScreen.Show();
            else
                UnityEngine.SceneManagement.SceneManager.LoadScene(
                    UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
                );
        }
    }

    void UpdateUI()
    {
        if (healthText != null)
            healthText.text = "HP: " + currentHealth;
    }
}