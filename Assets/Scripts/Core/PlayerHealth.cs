using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int health = 100;

    // Subscribe to this to respond to player death (Level 3 uses this for death screen)
    public static event System.Action OnDeath;

    private bool isDead = false;

    void Start()
    {
        isDead = false;
        GameManager.Instance?.uiManager?.UpdateHPDisplay(health);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        health -= damage;
        health = Mathf.Max(0, health);

        Debug.Log("Player Health: " + health);
        GameManager.Instance?.uiManager?.UpdateHPDisplay(health);
        DamageFlashCanvas.Instance?.Flash();

        if (health <= 0)
        {
            isDead = true;
            Debug.Log("Player Died!");
            OnDeath?.Invoke();
        }
    }
}