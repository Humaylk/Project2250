using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackRange = 2f;
    public string attackTriggerName = "Attack";
    public string attackStateName = "Attack";

    private Animator animator;
    private PlayerWeapon weapon;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        weapon = GetComponent<PlayerWeapon>();

        if (animator == null)
        {
            Debug.LogWarning("No Animator found on Player or its children.");
        }

        if (weapon == null)
        {
            Debug.LogWarning("No PlayerWeapon found on Player.");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            PlayAttackAnimationWithoutReset();
            DealDamage();
        }
    }

    private void PlayAttackAnimationWithoutReset()
    {
        if (animator == null) return;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // Only trigger the animation if we are NOT already in the Attack state
        if (!stateInfo.IsName(attackStateName))
        {
            animator.SetTrigger(attackTriggerName);
        }
    }

    private void DealDamage()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange);

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                int damage = 1;

                if (weapon != null)
                {
                    damage = weapon.GetDamage();
                }

                EnemyHealth enemyHealth = hit.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(damage);
                }

                GameManager.Instance?.progressionSystem?.AddCombatXP();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}