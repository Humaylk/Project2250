using UnityEngine;

public class GolemAI : MonoBehaviour
{
    public Transform player;
    public float speed = 2f;
    public float attackRange = 2f;
    public int damage = 10;

    float attackCooldown = 1.5f;
    float lastAttackTime = 0f;

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > attackRange)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
            transform.LookAt(player);
        }
        else
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                Debug.Log("Golem Attacking!");

                PlayerHealth ph = player.GetComponent<PlayerHealth>();
                if (ph != null)
                {
                    ph.TakeDamage(damage);
                }

                lastAttackTime = Time.time;
            }
        }
    }
}