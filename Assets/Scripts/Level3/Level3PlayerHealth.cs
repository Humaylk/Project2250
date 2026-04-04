using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Level3PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int health = 100;

    [Header("Health Bar UI")]
    public Image healthBarFill;       // The red/green fill image
    public TextMeshProUGUI hpText;    // Shows "110 / 110" next to the bar

    private PlayerHealth coreHealth;

    void Start()
    {
        coreHealth = GetComponent<PlayerHealth>();
        health = maxHealth;
        UpdateHPUI();
    }

    void Update()
    {
        // Mirror the Core PlayerHealth so the bar stays in sync when fish deal damage
        if (coreHealth != null && coreHealth.health != health)
        {
            health = coreHealth.health;
            UpdateHPUI();
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health < 0) health = 0;
        UpdateHPUI();
    }

    public void Heal(int amount)
    {
        health += amount;
        if (health > maxHealth) health = maxHealth;
        UpdateHPUI();
    }

    void UpdateHPUI()
    {
        // Update the fill bar (0 = empty, 1 = full)
        if (healthBarFill != null)
            healthBarFill.fillAmount = (float)health / maxHealth;

        // Update the number text
        if (hpText != null)
            hpText.text = health + " / " + maxHealth;
    }
}
