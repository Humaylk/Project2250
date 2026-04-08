using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    public int health    = 100;
    public int maxHealth = 100;

    [Header("Health Bar UI")]
    public Image    healthBarFill;
    public TMP_Text healthText;

    // Subscribe to respond to player death (used by death screen)
    public static event System.Action OnDeath;

    private bool     isDead = false;
    private Animator animator;

    void Awake()
    {
        // Use serialized values so each level can set its own max health
        health = maxHealth;
    }

    void Start()
    {
        isDead   = false;
        animator = GetComponentInChildren<Animator>();
        UpdateHealthBar();
        GameManager.Instance?.uiManager?.UpdateHPDisplay(health);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        health -= damage;
        health  = Mathf.Max(0, health);

        UpdateHealthBar();
        GameManager.Instance?.uiManager?.UpdateHPDisplay(health);
        DamageFlashCanvas.Instance?.Flash();

        // Play hurt animation — use Level3PlayerAnimator if present, else direct
        Level3PlayerAnimator l3anim = GetComponent<Level3PlayerAnimator>();
        if (l3anim != null) l3anim.TriggerHurt();
        else animator?.SetTrigger("Hurt");

        if (health <= 0)
            Die();
    }

    private void Die()
    {
        isDead = true;

        // Play death animation — use Level3PlayerAnimator if present, else direct
        Level3PlayerAnimator l3anim = GetComponent<Level3PlayerAnimator>();
        if (l3anim != null) l3anim.TriggerDeath();
        else if (animator != null)
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

    void UpdateHealthBar()
    {
        if (healthBarFill != null)
            healthBarFill.fillAmount = (float)health / maxHealth;
        if (healthText != null)
            healthText.text = health + "/" + maxHealth;
    }
}
