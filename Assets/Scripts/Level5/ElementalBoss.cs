using UnityEngine;

// Munadir: Full Level 5 boss with dragon animations, 3 phases, IDamageable
// Munadir: Walk animation plays while chasing, attack animation on contact
// Munadir: Fire animation triggers when player uses fire ability nearby
// Munadir: Phase changes speed up boss and change color tint
public class ElementalBoss : MonoBehaviour, IDamageable
{
    [Header("Boss Stats")]
    public int maxHP = 300;
    public int currentHP;
    public float moveSpeed = 1f;
    public int contactDamage = 20;
    public float attackCooldown = 2f;
    public float stopDistance = 2.5f;

    [Header("Phase Thresholds")]
    public float phase2Threshold = 0.66f;
    public float phase3Threshold = 0.33f;

    [Header("Visual - color tints per phase")]
    public SpriteRenderer bossRenderer;
    public Color phase1Color = Color.white;
    public Color phase2Color = new Color(1f, 0.6f, 0.2f);
    public Color phase3Color = new Color(1f, 0.1f, 0.1f);

    [Header("References")]
    public LaserSystem laserSystem;
    public UIManager uiManager;

    // Internal
    private Transform playerTransform;
    private PlayerHealth playerHealth;
    private float lastAttackTime;
    private bool isDefeated = false;
    private int currentPhase = 1;
    private Animator animator;

    public void Initialize()
    {
        currentHP = maxHP;
        isDefeated = false;
        currentPhase = 1;

        PlayerController pc = FindFirstObjectByType<PlayerController>();
        if (pc != null)
        {
            playerTransform = pc.transform;
            playerHealth = pc.GetComponent<PlayerHealth>();
        }

        animator = GetComponent<Animator>();
        bossRenderer = GetComponent<SpriteRenderer>();

        if (bossRenderer != null)
            bossRenderer.color = phase1Color;

        // Start idle then wake up after 2 seconds
        if (animator != null)
        {
            animator.SetBool("isIdle", true);
            Invoke("WakeUp", 2f);
        }

        UpdateBossUI();
        Debug.Log("Dragon Boss initialized - Phase 1");
    }

    void WakeUp()
    {
        if (animator != null)
            animator.SetBool("isIdle", false);
        uiManager?.ShowHint("The Elemental Dragon awakens! Defeat it before time runs out!");
    }

    void Update()
    {
        if (isDefeated || playerTransform == null) return;
        MoveTowardsPlayer();
        CheckPhaseTransition();
    }

    private void MoveTowardsPlayer()
    {
        float dist = Vector2.Distance(transform.position, playerTransform.position);

        if (dist > stopDistance)
        {
            // Walking animation
            if (animator != null)
                animator.SetBool("isAttacking", false);

            transform.position = Vector2.MoveTowards(
                transform.position,
                playerTransform.position,
                moveSpeed * Time.deltaTime
            );

            // Flip sprite to face player
            if (bossRenderer != null)
                bossRenderer.flipX = playerTransform.position.x > transform.position.x;
        }
        else
        {
            // In attack range — trigger attack animation
            if (animator != null)
                animator.SetBool("isAttacking", true);

            // Deal damage on cooldown
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                playerHealth?.TakeDamage(contactDamage);
                lastAttackTime = Time.time;
                Debug.Log("Dragon attacked player for " + contactDamage);
            }
        }
    }

    private void CheckPhaseTransition()
    {
        float hpPercent = (float)currentHP / maxHP;

        if (hpPercent <= phase3Threshold && currentPhase < 3)
            EnterPhase(3);
        else if (hpPercent <= phase2Threshold && currentPhase < 2)
            EnterPhase(2);
    }

    private void EnterPhase(int phase)
    {
        currentPhase = phase;
        Debug.Log("Dragon entering phase " + phase);

        switch (phase)
        {
            case 2:
                moveSpeed = 1.8f;
                contactDamage = 25;
                if (bossRenderer != null) bossRenderer.color = phase2Color;
                laserSystem?.IncreaseIntensity();
                uiManager?.ShowHint("The Dragon grows stronger! Watch out for more lasers!");
                break;
            case 3:
                moveSpeed = 2.8f;
                contactDamage = 35;
                if (bossRenderer != null) bossRenderer.color = phase3Color;
                laserSystem?.MaxIntensity();
                uiManager?.ShowHint("FINAL PHASE - The Dragon is enraged! Use all abilities!");
                break;
        }
    }

    // IDamageable
    public void TakeDamage(int amount)
    {
        if (isDefeated) return;
        currentHP -= amount;
        currentHP = Mathf.Max(currentHP, 0);
        Debug.Log("Dragon HP: " + currentHP + "/" + maxHP);
        UpdateBossUI();
        GameManager.Instance?.progressionSystem?.AddCombatXP(5);
        if (currentHP <= 0) Die();
    }

    public bool IsAlive() => !isDefeated;
    public bool IsDefeated() => isDefeated;

    // Called by AbilityManager when fire ability hits
    public void TriggerFireAnimation()
    {
        if (animator != null)
        {
            animator.SetBool("isFiring", true);
            Invoke("StopFireAnim", 1f);
        }
    }

    void StopFireAnim()
    {
        if (animator != null)
            animator.SetBool("isFiring", false);
    }

    private void Die()
    {
        isDefeated = true;
        if (animator != null) animator.SetBool("isAttacking", false);
        Debug.Log("DRAGON DEFEATED!");
        gameObject.SetActive(false);
    }

    private void UpdateBossUI()
    {
        if (uiManager != null)
            uiManager.ShowHint("Dragon HP: " + currentHP + "/" + maxHP + "  |  Phase " + currentPhase);
    }
}