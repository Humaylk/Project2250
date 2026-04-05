using UnityEngine;

public class PlayerAttack4 : MonoBehaviour
{
    public float attackRange = 2f;
    public int damage = 10;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            Attack();
        }
    }

    void Attack()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange);

        foreach (Collider2D hit in hits)
        {
            GolemHealth gh = hit.GetComponent<GolemHealth>();

            if (gh != null)
            {
                gh.TakeDamage(damage);
                Debug.Log("Hit golem!");
            }
        }
    }

    // optional: show attack range in scene
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}