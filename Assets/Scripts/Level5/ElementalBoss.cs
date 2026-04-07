using UnityEngine;
using System.Collections;

// Munadir: Full Level 5 boss with dragon animations, 3 phases, IDamageable
// Munadir: Uses Rigidbody2D.MovePosition in FixedUpdate for physics-safe movement
// Munadir: Flashes white when hit for visual damage feedback
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

    [Header("Audio")]
    public AudioClip hitSound;
    public AudioClip deathSound;
    public AudioClip phaseChangeSound;

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
    private Rigidbody2D rb;
    private AudioSource audioSource;
    private Color currentPhaseColor;
    private bool isFlashing = false;

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
        rb = GetComponent<Rigidbody2D>();

        // Munadir: Audio setup
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f;
        }

        if (rb != null)
        {
            rb.gravityScale = 0;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        }

        currentPhaseColor = phase1Color;
        if (bossRenderer != null)
            bossRenderer.color = phase1Color;

        if (animator != null)
        {
            animator.SetBool("isIdle", true);
            Invoke("WakeUp", 2f);
        }
    }

    void WakeUp()
    {
        if (animator != null) animator.SetBool("isIdle", false);
        uiManager?.ShowHint("The Elemental Dragon awakens!");
    }

    void Update()
    {
        if (isDefeated || playerTransform == null) return;

        float dist = Vector2.Distance(transform.position, playerTransform.position);
        bool inRange = dist <= stopDistance;

        if (animator != null) animator.SetBool("isAttacking", inRange);

        if (inRange && Time.time - lastAttackTime >= attackCooldown)
        {
            playerHealth?.TakeDamage(contactDamage);
            lastAttackTime = Time.time;
        }

        // Munadir: Face player
        if (bossRenderer != null)
            bossRenderer.flipX = playerTransform.position.x > transform.position.x;

        CheckPhaseTransition();
    }

    void FixedUpdate()
    {
        if (isDefeated || playerTransform == null || rb == null) return;

        float dist = Vector2.Distance(rb.position, playerTransform.position);

        if (dist > stopDistance)
        {
            Vector2 direction = ((Vector2)playerTransform.position - rb.position).normalized;
            rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
        }
    }

    private void CheckPhaseTransition()
    {
        float hpPercent = (float)currentHP / maxHP;
        if (hpPercent <= phase3Threshold && currentPhase < 3) EnterPhase(3);
        else if (hpPercent <= phase2Threshold && currentPhase < 2) EnterPhase(2);
    }

    private void EnterPhase(int phase)
    {
        currentPhase = phase;

        if (phaseChangeSound != null && audioSource != null)
            audioSource.PlayOneShot(phaseChangeSound);

        switch (phase)
        {
            case 2:
                moveSpeed = 1.8f;
                contactDamage = 25;
                currentPhaseColor = phase2Color;
                if (bossRenderer != null && !isFlashing) bossRenderer.color = phase2Color;
                laserSystem?.IncreaseIntensity();
                uiManager?.ShowHint("The Dragon grows stronger!");
                break;
            case 3:
                moveSpeed = 2.8f;
                contactDamage = 35;
                currentPhaseColor = phase3Color;
                if (bossRenderer != null && !isFlashing) bossRenderer.color = phase3Color;
                laserSystem?.MaxIntensity();
                uiManager?.ShowHint("FINAL PHASE - The Dragon is enraged!");
                break;
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDefeated) return;
        currentHP -= amount;
        currentHP = Mathf.Max(currentHP, 0);

        // Munadir: Visual damage flash
        StartCoroutine(DamageFlash());

        // Munadir: Sound
        if (hitSound != null && audioSource != null)
            audioSource.PlayOneShot(hitSound);

        GameManager.Instance?.progressionSystem?.AddCombatXP(5);

        if (currentHP <= 0) Die();
    }

    // Munadir: Flash boss white briefly when taking damage
    private IEnumerator DamageFlash()
    {
        if (bossRenderer == null) yield break;
        isFlashing = true;
        bossRenderer.color = Color.white;
        yield return new WaitForSeconds(0.12f);
        bossRenderer.color = currentPhaseColor;
        isFlashing = false;
    }

    public bool IsAlive() => !isDefeated;
    public bool IsDefeated() => isDefeated;

    public void TriggerFireAnimation()
    {
        if (animator != null)
        {
            animator.SetBool("isFiring", true);
            Invoke("StopFireAnim", 1f);
        }
    }

    void StopFireAnim() => animator?.SetBool("isFiring", false);

    private void Die()
    {
        isDefeated = true;

        if (deathSound != null && audioSource != null)
            audioSource.PlayOneShot(deathSound);

        // Munadir: Disable boss visually but don't destroy (win screen needs to check IsDefeated)
        if (bossRenderer != null) bossRenderer.enabled = false;
        if (rb != null) rb.simulated = false;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
    }
}
