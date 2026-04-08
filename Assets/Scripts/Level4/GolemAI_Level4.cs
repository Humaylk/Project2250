using UnityEngine;

public class GolemAI_Level4 : MonoBehaviour
{
    public Transform player;
    public float speed = 4f;
    public float attackRange = 1.8f;
    public int damage = 10;

    float attackCooldown = 0.7f;
    float lastAttackTime = 0f;

    Animator        animator;
    SpriteRenderer  sr;

    void Start()
    {
        animator = GetComponent<Animator>();
        sr       = GetComponent<SpriteRenderer>();

        // Auto-find the player if not assigned in Inspector
        if (player == null)
        {
            SkyPlayerController spc = FindFirstObjectByType<SkyPlayerController>();
            if (spc != null) player = spc.transform;
        }
    }

    void Update()
    {
        // Retry finding player each frame until found
        if (player == null)
        {
            SkyPlayerController spc = FindFirstObjectByType<SkyPlayerController>();
            if (spc != null) player = spc.transform;
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);

        // Flip sprite to always face the player
        if (sr != null)
            sr.flipX = player.position.x < transform.position.x;

        // MOVE TOWARD PLAYER
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

            // ATTACK aggressively
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                if (animator != null)
                    animator.SetTrigger("Attack");

                PlayerHealth_Level4 ph = player.GetComponent<PlayerHealth_Level4>();
                if (ph != null)
                    ph.TakeDamage(damage);

                lastAttackTime = Time.time;
            }
        }
    }
}