using UnityEngine;

// Munadir: Rewrote GolemAI to use 2D movement and Vector2 distance
// Munadir: Removed transform.LookAt which breaks 2D games
// Munadir: Fixed attack to use 2D distance calculation
//Yoseph: Changed the GolemAI to match the attack animation when it attacks
//Yoseph: Fixed the animation
public class GolemAI : MonoBehaviour
{
    public Transform player;
    public float speed = 2f;
    public float attackRange = 1.5f;
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
 
        if (distance > attackRange)
        {
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