using UnityEngine;

// Munadir: Rewrote GolemAI to use 2D movement and Vector2 distance
// Munadir: Removed transform.LookAt which breaks 2D games
// Munadir: Fixed attack to use 2D distance calculation
public class GolemAI : MonoBehaviour
{
    public Transform player;
    public float speed = 2f;
    public float attackRange = 1.5f;
    public int damage = 10;

    float attackCooldown = 1.5f;
    float lastAttackTime = 0f;

    void Update()
    {
        if (player == null) return;

        // Munadir: Using Vector2 distance for 2D game instead of Vector3
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > attackRange)
        {
            // Munadir: Move towards player using Vector2 direction
            Vector2 direction = (player.position - transform.position).normalized;
            transform.position = Vector2.MoveTowards(
                transform.position,
                player.position,
                speed * Time.deltaTime
            );
        }
        else
        {
            // Munadir: Attack player when in range with cooldown
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                Debug.Log("Golem Attacking!");

                PlayerHealth ph = player.GetComponent<PlayerHealth>();
                if (ph != null)
                    ph.TakeDamage(damage);

                lastAttackTime = Time.time;
            }
        }
    }
}