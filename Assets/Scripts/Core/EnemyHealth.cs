using UnityEngine;

// Munadir: Fixed Die() to use 2D Collider instead of 3D Collider
// Munadir: Added UIManager combat XP notification on death
// Munadir: Added null checks to prevent missing component errors
public class EnemyHealth : MonoBehaviour
{
    public int health = 50;

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log("Enemy Health: " + health);

        // Munadir: Show remaining health as hint via UIManager
        GameManager.Instance?.uiManager?.ShowHint("Golem HP: " + health);

        if (health <= 0)
            Die();
    }

    void Die()
    {
        Debug.Log("Enemy Died!");

        // Munadir: Award combat XP when golem is killed
        GameManager.Instance?.progressionSystem?.AddCombatXP();
        GameManager.Instance?.uiManager?.ShowHint("Golem defeated! +10 XP");

        // Munadir: Fixed - using Collider2D instead of 3D Collider
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // Munadir: Fixed - using SpriteRenderer instead of Renderer
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = false;

        Destroy(gameObject, 0.1f);
    }
}