using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;

    private Animator        animator;
    private SpriteRenderer  spriteRenderer;
    private Rigidbody2D     rb;

    void Start()
    {
        animator       = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        rb             = GetComponent<Rigidbody2D>();

        // Force Grounded=true — this is a top-down game, no jumping/falling
        if (animator != null)
            animator.SetBool("Grounded", true);
    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Vector2 move = new Vector2(moveX, moveY).normalized;

        // Flip sprite to face movement direction
        if (moveX > 0 && spriteRenderer != null)
            spriteRenderer.flipX = false;
        else if (moveX < 0 && spriteRenderer != null)
            spriteRenderer.flipX = true;

        // HeroKnight uses AnimState int: 0 = Idle, 1 = Run
        if (animator != null)
            animator.SetInteger("AnimState", move != Vector2.zero ? 1 : 0);

        // Store move for FixedUpdate
        _moveInput = move;
    }

    private Vector2 _moveInput;

    void FixedUpdate()
    {
        // MovePosition respects physics colliders — player can't walk through golems, bridges, walls
        if (rb != null)
            rb.MovePosition(rb.position + _moveInput * speed * Time.fixedDeltaTime);
    }
}
