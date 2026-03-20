using UnityEngine;

// Munadir: Updated attack key from Space to G for combat
// Munadir: Switched from 3D Physics.OverlapSphere to 2D Physics2D.OverlapCircleAll
// Munadir: Added ProgressionSystem combat XP call on successful hit
public class PlayerAttack : MonoBehaviour
{
    public float attackRange = 2f;

    void Update()
    {
        // Munadir: Changed from Space to G key for attacking enemies
        if (Input.GetKeyDown(KeyCode.G))
        {
            // Munadir: Using 2D overlap circle instead of 3D - this is a 2D game
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange);

            foreach (Collider2D hit in hits)
            {
                if (hit.CompareTag("Enemy"))
                {
                    PlayerWeapon weapon = GetComponent<PlayerWeapon>();
                    int damage = weapon.GetDamage();
                    Debug.Log("Hit enemy for " + damage + " damage");

                    EnemyHealth eh = hit.GetComponent<EnemyHealth>();
                    if (eh != null)
                        eh.TakeDamage(damage);

                    // Munadir: Award combat XP through ProgressionSystem on each kill
                    GameManager.Instance?.progressionSystem?.AddCombatXP();
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        // Munadir: Visualize attack range in Scene view for debugging
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}