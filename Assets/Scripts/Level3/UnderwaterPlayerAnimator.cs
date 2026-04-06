using UnityEngine;

// Bridges the PlayerHealth damage events to the underwater player's Animator.
// Attach this alongside PlayerHealth on the Alex GameObject in Level 3.
// Listens for HP changes and triggers the hurt animation on the diver sprite.
[RequireComponent(typeof(PlayerHealth))]
[RequireComponent(typeof(Animator))]
public class UnderwaterPlayerAnimator : MonoBehaviour
{
    private Animator animator;
    private PlayerHealth playerHealth;
    private int lastHealth;
    private bool hasIsHurt = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        playerHealth = GetComponent<PlayerHealth>();
        lastHealth = playerHealth.health;

        if (animator != null)
            foreach (AnimatorControllerParameter p in animator.parameters)
                if (p.name == "isHurt") { hasIsHurt = true; break; }

        // Flip sprite to face right by default
        Vector3 s = transform.localScale;
        s.x = Mathf.Abs(s.x);
        transform.localScale = s;
    }

    void Update()
    {
        // Detect when HP drops and trigger hurt animation
        if (playerHealth.health < lastHealth)
        {
            if (hasIsHurt) animator.SetTrigger("isHurt");
        }
        lastHealth = playerHealth.health;

        // Flip sprite to face movement direction
        float moveX = Input.GetAxisRaw("Horizontal");
        if (moveX != 0)
        {
            Vector3 s = transform.localScale;
            s.x = Mathf.Abs(s.x) * Mathf.Sign(moveX);
            transform.localScale = s;
        }
    }
}
