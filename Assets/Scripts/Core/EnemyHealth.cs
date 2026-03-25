using UnityEngine;

// Munadir: Fixed Die() to use 2D Collider instead of 3D Collider
// Munadir: Added UIManager combat XP notification on death
// Munadir: Added null checks to prevent missing component errors
//Yoseph: Added a code to validate death so that a death animation may be played
public class EnemyHealth : MonoBehaviour
{
    public int health = 50;
    Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }
    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log("Enemy Health: " + health);
        GameManager.Instance?.uiManager?.ShowHint("Golem HP: " + health);
        if (health <= 0)
            Die();
    }
    void Die()
    {
        Debug.Log("Enemy Died!");
        animator.SetBool("isDead", true);
        animator.ResetTrigger("Attack");
        GolemAI ai = GetComponent<GolemAI>();
        
        if (ai != null) ai.enabled = false;
        
        GameManager.Instance?.progressionSystem?.AddCombatXP();
        GameManager.Instance?.uiManager?.ShowHint("Golem defeated! +10 XP");
        Collider2D col = GetComponent<Collider2D>();
        
        if (col != null) col.enabled = false;
        Destroy(gameObject, 1.5f);
    }
}