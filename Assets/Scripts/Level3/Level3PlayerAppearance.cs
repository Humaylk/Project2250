using UnityEngine;

// Handles player sprite swapping in Level 3.
// Moving = swimming sprite, Idle = normal sprite.
// After helmet pickup: switches to helmet versions of both.
public class Level3PlayerAppearance : MonoBehaviour
{
    [Header("No Helmet Sprites")]
    public Sprite idleSprite;       // Player Sheet No Effect.png
    public Sprite swimmingSprite;   // Player Swimming.png

    [Header("Helmet Sprites")]
    public Sprite idleHelmetSprite;     // Player Sheet No Effect Helmet.png
    public Sprite swimmingHelmetSprite; // Player Swimming Helmet.png

    private SpriteRenderer sr;
    private Animator animator;
    private bool hasHelmet = false;
    private bool wasMoving = false;

    void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();

        // Disable animator so it doesn't override our sprite swapping
        if (animator != null)
            animator.enabled = false;

        ApplySprite(false);
    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        bool isMoving = (moveX != 0 || moveY != 0);

        if (isMoving != wasMoving)
        {
            wasMoving = isMoving;
            ApplySprite(isMoving);
        }
    }

    public void EquipHelmet()
    {
        hasHelmet = true;
        ApplySprite(wasMoving);
    }

    private void ApplySprite(bool isMoving)
    {
        if (sr == null) return;

        if (hasHelmet)
            sr.sprite = isMoving ? swimmingHelmetSprite : idleHelmetSprite;
        else
            sr.sprite = isMoving ? swimmingSprite : idleSprite;
    }

    // Call this to restore the original appearance when leaving Level 3
    public void RestoreOriginal()
    {
        hasHelmet = false;
        if (animator != null)
            animator.enabled = true;
    }
}
