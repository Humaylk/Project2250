using UnityEngine;

// Munadir: The main boss for Level 5
// Munadir: Has 3 phases based on HP thresholds
// Munadir: Slow movement but high damage - gets faster as HP drops
// Munadir: Has weak points that take extra damage
public class ElementalBoss : MonoBehaviour, IDamageable
{
    [Header("Boss Stats")]
    public int maxHP = 300;
    public int currentHP;
    public float moveSpeed = 1f;
    public int contactDamage = 20;
    public float attackCooldown = 2f;

    [Header("Phase Thresholds")]
    public float phase2Threshold = 0.66f; // 66% HP = phase 2
    public float phase3Threshold = 0.33f; // 33% HP = phase 3

    [Header("Visual")]
    public SpriteRenderer bossRenderer;
    public Color phase1Color = new Color(0.5f, 0f, 1f); // purple
    public Color phase2Color = new Color(1f, 0.3f, 0f); // orange
    public Color phase3Color = new Color(1f, 0f, 0f);   // red

    [Header("References")]
    public LaserSystem laserSystem;
    public UIManager uiManager;

    private Transform playerTransform;
    private float lastAttackTime;
    private bool isDefeated = false;
    private int currentPhase = 1;

    public void Initialize()
    {
        currentHP = maxHP;
        isDefeated = false;
        currentPhase = 1;
        playerTransform = FindFirstObjectByType<PlayerController>()?.transform;

        if (bossRenderer != null)
            bossRenderer.color = phase1Color;

        UpdateBossUI();
        Debug.Log("Boss initialized - Phase 1");
    }

    void Update()
    {
        if (isDefeated) return;
        if (playerTransform == null) return;

        MoveTowardsPlayer();
        CheckPhaseTransition();
    }

    private void MoveTowardsPlayer()
    {
        if (playerTransform == null) return;

        // Munadir: Stop moving when close enough - creates attack range instead of pushing
        float dist = Vector2.Distance(transform.position, playerTransform.position);
        float stopDistance = 2.5f;

        if (dist > stopDistance)
        {
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            transform.position = Vector2.MoveTowards(
                transform.position,
                playerTransform.position,
                moveSpeed * Time.deltaTime
            );
        }
    }

    private void CheckPhaseTransition()
    {
        float hpPercent = (float)currentHP / maxHP;

        if (hpPercent <= phase3Threshold && currentPhase < 3)
        {
            EnterPhase(3);
        }
        else if (hpPercent <= phase2Threshold && currentPhase < 2)
        {
            EnterPhase(2);
        }
    }

    private void EnterPhase(int phase)
    {
        currentPhase = phase;
        Debug.Log("Boss entering phase " + phase);

        switch (phase)
        {
            case 2:
                moveSpeed = 1.8f;
                contactDamage = 25;
                if (bossRenderer != null) bossRenderer.color = phase2Color;
                if (laserSystem != null) laserSystem.IncreaseIntensity();
                uiManager?.ShowHint("The boss grows stronger! More lasers incoming!");
                break;
            case 3:
                moveSpeed = 2.5f;
                contactDamage = 35;
                if (bossRenderer != null) bossRenderer.color = phase3Color;
                if (laserSystem != null) laserSystem.MaxIntensity();
                uiManager?.ShowHint("FINAL PHASE - The boss is enraged!");
                break;
        }
    }

    // IDamageable implementation
    public void TakeDamage(int amount)
    {
        if (isDefeated) return;

        currentHP -= amount;
        currentHP = Mathf.Max(currentHP, 0);
        Debug.Log("Boss HP: " + currentHP + "/" + maxHP);

        UpdateBossUI();
        GameManager.Instance?.progressionSystem?.AddCombatXP(5);

        if (currentHP <= 0)
            Die();
    }

    public bool IsAlive() => !isDefeated;
    public bool IsDefeated() => isDefeated;

    private void Die()
    {
        isDefeated = true;
        Debug.Log("ELEMENTAL BOSS DEFEATED!");
        gameObject.SetActive(false);
    }

    private void UpdateBossUI()
    {
        uiManager?.ShowHint("Boss HP: " + currentHP + "/" + maxHP + " | Phase " + currentPhase);
    }

    // Damage player on contact
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                other.gameObject.GetComponent<PlayerHealth>()?.TakeDamage(contactDamage);
                lastAttackTime = Time.time;
            }
        }
    }
}