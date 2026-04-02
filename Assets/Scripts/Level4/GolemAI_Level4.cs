using UnityEngine;

public class GolemAI_Level4 : MonoBehaviour
{
    public Transform player;
    public float speed = 2f;
    public float attackRange = 1.8f;
    public int damage = 10;

    float attackCooldown = 1.5f;
    float lastAttackTime = 0f;

    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // 🔁 MOVE TOWARD PLAYER
        if (distance > attackRange)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                player.position,
                speed * Time.deltaTime
            );

            if (animator != null)
                animator.SetBool("isWalking", true);
        }
        else
        {
            if (animator != null)
                animator.SetBool("isWalking", false);

            // ⚔️ ATTACK
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                if (animator != null)
                    animator.SetTrigger("Attack");

                // IMPORTANT FIX
                PlayerHealth_Level4 ph = player.GetComponent<PlayerHealth_Level4>();

                if (ph != null)
                {
                    ph.TakeDamage(damage);
                    Debug.Log("Golem hit player!");
                }

                lastAttackTime = Time.time;
            }
        }
    }
}