using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int health = 100;

    // Subscribe to respond to player death (used by death screen)
    public static event System.Action OnDeath;

    private bool     isDead = false;
    private Animator animator;

    void Start()
    {
        isDead   = false;
        animator = GetComponentInChildren<Animator>();
        GameManager.Instance?.uiManager?.UpdateHPDisplay(health);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        health -= damage;
        health  = Mathf.Max(0, health);

        GameManager.Instance?.uiManager?.UpdateHPDisplay(health);
        DamageFlashCanvas.Instance?.Flash();

        // Play hurt animation
        animator?.SetTrigger("Hurt");

        if (health <= 0)
            Die();
    }

    private void Die()
    {
        isDead = true;

        // Play death animation
        if (animator != null)
        {
            animator.SetBool("noBlood", false);
            animator.SetTrigger("Death");
        }

        // Disable movement and attacking
        PlayerController pc = GetComponent<PlayerController>();
        if (pc != null) pc.enabled = false;

        PlayerAttack pa = GetComponent<PlayerAttack>();
        if (pa != null) pa.enabled = false;

        OnDeath?.Invoke();
    }
}
