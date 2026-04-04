using UnityEngine;

// Enemy AI for Level 3 - Drowned Vault (Water Island).
// Represents fish enemy units that actively hunt and attack the player underwater,
// reducing HP during combat encounters. Supports multiple instances (two in this level).
public class FishAssassin : MonoBehaviour
{
    public Transform player;
    public float speed = 2.5f;
    public float attackRange = 1.2f;
    public int damage = 12;
    public int maxHealth = 30;

    [Header("Death Effect")]
    public GameObject deathEffectPrefab; // Drag FishDeathEffect prefab here

    private int currentHealth;
    private float attackCooldown = 1.5f;
    private float lastAttackTime = 0f;
    private bool isDead = false;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;

        // Auto-find player if not assigned in Inspector
        if (player == null)
        {
            PlayerController pc = FindFirstObjectByType<PlayerController>();
            if (pc != null) player = pc.transform;
        }
    }

    void Update()
    {
        if (player == null || isDead) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > attackRange)
        {
            // Flip sprite to face the player horizontally
            float dir = player.position.x - transform.position.x;
            if (dir != 0f)
            {
                Vector3 s = transform.localScale;
                s.x = Mathf.Abs(s.x) * Mathf.Sign(dir);
                transform.localScale = s;
            }

            transform.position = Vector2.MoveTowards(
                transform.position,
                player.position,
                speed * Time.deltaTime
            );

            if (animator != null) animator.SetBool("isWalking", true);
        }
        else
        {
            if (animator != null) animator.SetBool("isWalking", false);

            if (Time.time >= lastAttackTime + attackCooldown)
            {
                if (animator != null) animator.SetTrigger("Attack");

                PlayerHealth ph = player.GetComponent<PlayerHealth>();
                if (ph != null)
                    ph.TakeDamage(damage);

                lastAttackTime = Time.time;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        isDead = true;

        // Spawn the explosion/death effect at this position
        if (deathEffectPrefab != null)
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
