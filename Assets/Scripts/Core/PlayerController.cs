using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;

    // Optional constant drift applied every frame (used by Level 3 underwater effect)
    [HideInInspector] public Vector2 constantDrift = Vector2.zero;

    private Animator        animator;
    private SpriteRenderer  spriteRenderer;
    private Rigidbody2D     rb;
    private float           _driftTime = 0f;

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
        if (rb == null) return;

        Vector2 move = _moveInput * speed;

        if (constantDrift != Vector2.zero)
        {
            _driftTime += Time.fixedDeltaTime;
            // Slight sine-wave oscillation on the downward drift — varies between 60% and 100%
            // of the base drift speed to mimic gentle underwater current fluctuation.
            float wave = 0.8f + Mathf.Sin(_driftTime * 1.2f) * 0.2f;
            move += constantDrift * wave;
        }

        // MovePosition respects physics colliders — player can't walk through walls
        rb.MovePosition(rb.position + move * Time.fixedDeltaTime);
    }
}
