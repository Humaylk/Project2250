using UnityEngine;

// Wolf enemy AI for Level 2 - Shadow Swamp
// Moves toward the player and attacks on a cooldown, using the DarkWolf_2d animator.
public class WolfAI : MonoBehaviour
{
    public Transform player;
    public float speed = 3f;
    public float attackRange = 1.2f;
    public int damage = 15;
    float attackCooldown = 1.2f;
    float lastAttackTime = 0f;

    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (player == null)
        {
            PlayerController pc = FindFirstObjectByType<PlayerController>();
            if (pc != null) player = pc.transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > attackRange)
        {
            // Flip sprite to face the player
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

            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);

            if (Time.time >= lastAttackTime + attackCooldown)
            {
                animator.SetTrigger("Attack");
                PlayerHealth ph = player.GetComponent<PlayerHealth>();
                if (ph != null)
                    ph.TakeDamage(damage);
                lastAttackTime = Time.time;
            }
        }
    }
}
