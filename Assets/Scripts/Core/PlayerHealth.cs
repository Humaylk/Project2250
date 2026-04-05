using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth;
    public int health;

    void Start()
    {
        SetHealthForCurrentScene();

        health = maxHealth;

        if (GameManager.Instance != null)
            GameManager.Instance.currentMaxHealth = maxHealth;

        RefreshUI();

        Debug.Log("Scene name: " + SceneManager.GetActiveScene().name);
        Debug.Log("Max health set to: " + maxHealth);
        Debug.Log("Current health set to: " + health);
    }

    void SetHealthForCurrentScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        switch (sceneName)
        {
            case "Level1_CrackedForest":
                maxHealth = 100;
                break;

            case "Level2_EmberDepths":
                maxHealth = 105;
                break;

            case "Level3":
                maxHealth = 110;
                break;

            case "Level4":
                maxHealth = 115;
                break;

            case "Level5":
                maxHealth = 120;
                break;

            default:
                maxHealth = 100;
                break;
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health < 0)
            health = 0;

        Debug.Log("Player Health: " + health);
        UIManager ui = FindObjectOfType<UIManager>();
        if (ui != null)
        {
            ui.UpdateHPDisplay(health);
        }
        if (health <= 0)
        {
            Debug.Log("Player Died!");
            GameManager.Instance?.ResetOnDeath();
        }
    }

    void RefreshUI()
    {
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.uiManager == null)
                GameManager.Instance.uiManager = FindObjectOfType<UIManager>();

            GameManager.Instance.uiManager?.UpdateHPDisplay(health);
        }
    }
}