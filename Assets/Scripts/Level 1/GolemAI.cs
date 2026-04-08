using UnityEngine;

// Munadir: Rewrote GolemAI to use 2D movement and Vector2 distance
// Munadir: Removed transform.LookAt which breaks 2D games
// Munadir: Fixed attack to use 2D distance calculation
//Yoseph: Changed the GolemAI to match the attack animation when it attacks
//Yoseph: Fixed the animation
public class GolemAI : MonoBehaviour
{
    public Transform player;
    public float speed = 3.5f;
    public float attackRange = 1.5f;
    public int damage = 10;
    float attackCooldown = 1.5f;
    float lastAttackTime = 0f;
 
    Animator animator;
    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;
    bool isInRange;

    void Start()
    {
        animator        = GetComponent<Animator>();
        rb              = GetComponent<Rigidbody2D>();
        spriteRenderer  = GetComponent<SpriteRenderer>();

        if (rb != null)
        {
            rb.gravityScale    = 0f;
            rb.freezeRotation  = true;
            rb.linearDamping   = 10f; // stops sliding immediately when not moving
        }
    }

    void Update()
    {
        if (player == null) return;
        float distance = Vector2.Distance(transform.position, player.position);
        isInRange = distance <= attackRange;

        animator.SetBool("isWalking", !isInRange);

        // Flip sprite to face the player
        if (spriteRenderer != null && !isInRange)
        {
            float dirX = player.position.x - transform.position.x;
            if (dirX > 0) spriteRenderer.flipX = true;
            else if (dirX < 0) spriteRenderer.flipX = false;
        }

        if (isInRange && Time.time >= lastAttackTime + attackCooldown)
        {
            animator.SetTrigger("Attack");
            PlayerHealth ph = player.GetComponent<PlayerHealth>();
            if (ph != null)
                ph.TakeDamage(damage);
            lastAttackTime = Time.time;
        }
    }

    void FixedUpdate()
    {
        if (player == null || isInRange) return;

        Vector2 nextPos = Vector2.MoveTowards(
            rb.position,
            player.position,
            speed * Time.fixedDeltaTime
        );

        rb.MovePosition(nextPos);
    }
}