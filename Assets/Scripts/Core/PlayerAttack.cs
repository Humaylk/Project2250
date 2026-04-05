using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackRange             = 2f;
    public float heavyAttackRange        = 2.5f;
    public string attackTriggerName      = "Attack";
    public string heavyAttackTriggerName = "HeavyAttack";
    public int    heavyAttackDamage      = 20;

    [Header("Heavy Attack Cooldown")]
    public float heavyAttackCooldown = 2.1f;
    private float lastHeavyAttackTime = -99f;

    private Animator     animator;
    private PlayerWeapon weapon;

    private bool HeavyAttackUnlocked => SceneManager.GetActiveScene().buildIndex >= 1;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        weapon   = GetComponent<PlayerWeapon>();

        if (animator == null)
            Debug.LogWarning("No Animator found on Player or its children.");
        if (weapon == null)
            Debug.LogWarning("No PlayerWeapon found on Player.");
    }

    private void Update()
    {
        // G key — normal attack
        if (Input.GetKeyDown(KeyCode.G))
        {
            animator?.SetTrigger(attackTriggerName);
            DealDamage(weapon != null ? weapon.GetDamage() : 1, attackRange);
        }

        // B key — heavy attack ability with cooldown
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (!HeavyAttackUnlocked)
            {
                Debug.Log("[PlayerAttack] Heavy Attack not unlocked yet!");
                return;
            }

            float timeSinceLastHeavy = Time.time - lastHeavyAttackTime;
            if (timeSinceLastHeavy < heavyAttackCooldown)
            {
                float remaining = heavyAttackCooldown - timeSinceLastHeavy;
                Debug.Log("[PlayerAttack] Heavy Attack on cooldown! " + remaining.ToString("F1") + "s remaining");
                return;
            }

            lastHeavyAttackTime = Time.time;
            animator?.ResetTrigger(heavyAttackTriggerName);
            animator?.SetTrigger(heavyAttackTriggerName);
            DealDamage(heavyAttackDamage, heavyAttackRange);
        }
    }

    private void DealDamage(int damage, float range)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range);

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                EnemyHealth enemyHealth = hit.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                    enemyHealth.TakeDamage(damage);

                GameManager.Instance?.progressionSystem?.AddCombatXP();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, heavyAttackRange);
    }
}