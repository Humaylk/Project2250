using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public float attackRange = 2f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, attackRange);

            foreach (Collider hit in hits)
            {
                if (hit.CompareTag("Enemy"))
                {
                    PlayerWeapon weapon = GetComponent<PlayerWeapon>();
                    int damage = weapon.GetDamage();

                    Debug.Log("Hit enemy for " + damage + " damage");

                    EnemyHealth eh = hit.GetComponent<EnemyHealth>();
                    if (eh != null)
                    {
                        eh.TakeDamage(damage);
                    }
                }
            }
        }
    }
}