using UnityEngine;
using System.Collections;

// Munadir: Fixed Die() to use 2D Collider instead of 3D Collider
// Munadir: Added UIManager combat XP notification on death
// Munadir: Added null checks to prevent missing component errors
//Yoseph: Added a code to validate death so that a death animation may be played
public class EnemyHealth : MonoBehaviour
{
    public int health = 50;
    Animator animator;
    private bool hasIsDead = false;
    private bool hasAttack = false;
    private bool isDying   = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator != null)
            foreach (AnimatorControllerParameter p in animator.parameters)
            {
                if (p.name == "isDead") hasIsDead = true;
                if (p.name == "Attack") hasAttack = true;
            }
    }
    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log("Enemy Health: " + health);
        GameManager.Instance?.uiManager?.ShowHint("Enemy HP: " + health);
        if (health <= 0)
            Die();
    }
    void Die()
    {
        if (isDying) return;
        isDying = true;

        Debug.Log("Enemy Died!");
        if (animator != null)
        {
            if (hasIsDead) animator.SetBool("isDead", true);
            if (hasAttack) animator.ResetTrigger("Attack");
        }

        GolemAI ai = GetComponent<GolemAI>();
        if (ai != null) ai.enabled = false;

        GameManager.Instance?.progressionSystem?.AddCombatXP();
        GameManager.Instance?.uiManager?.ShowHint("Enemy defeated! +10 XP");

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        StartCoroutine(DestroyAfterAnimation());
    }

    private IEnumerator DestroyAfterAnimation()
    {
        // Wait one frame so the Animator has time to transition into the death state
        yield return null;

        // Read the exact length of whichever clip is now playing (the death anim)
        float clipLength = animator != null
            ? animator.GetCurrentAnimatorStateInfo(0).length
            : 1.5f;

        yield return new WaitForSeconds(clipLength);
        Destroy(gameObject);
    }
}