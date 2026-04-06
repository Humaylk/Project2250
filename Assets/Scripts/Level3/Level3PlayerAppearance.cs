using UnityEngine;
using System.Collections;

// Handles player sprite swapping in Level 3.
// Moving = cycles through swimming frames, Idle = static sprite.
// After helmet pickup: switches to helmet versions of both.
public class Level3PlayerAppearance : MonoBehaviour
{
    [Header("Idle Sprites (single frame)")]
    public Sprite idleSprite;           // Player Sheet No Effect.png
    public Sprite idleHelmetSprite;     // Player Sheet No Effect Helmet.png

    [Header("Swimming Frames (slice Player Swimming.png as Multiple)")]
    public Sprite[] swimmingFrames;         // all frames from Player Swimming.png
    public Sprite[] swimmingHelmetFrames;   // all frames from Player Swimming Helmet.png

    [Header("Animation Speed")]
    public float frameRate = 0.1f;  // seconds per frame

    private SpriteRenderer sr;
    private Animator animator;
    private bool hasHelmet = false;
    private bool isMoving = false;
    private Coroutine swimCoroutine;

    void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();

        if (animator != null)
            animator.enabled = false;

        SetIdle();
    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        bool moving = (moveX != 0 || moveY != 0);

        if (moving != isMoving)
        {
            isMoving = moving;
            if (isMoving)
                StartSwimming();
            else
                SetIdle();
        }
    }

    public void EquipHelmet()
    {
        hasHelmet = true;
        if (isMoving)
            StartSwimming();
        else
            SetIdle();
    }

    private void SetIdle()
    {
        if (swimCoroutine != null)
        {
            StopCoroutine(swimCoroutine);
            swimCoroutine = null;
        }
        if (sr != null)
            sr.sprite = hasHelmet ? idleHelmetSprite : idleSprite;
    }

    private void StartSwimming()
    {
        if (swimCoroutine != null)
            StopCoroutine(swimCoroutine);

        Sprite[] frames = hasHelmet ? swimmingHelmetFrames : swimmingFrames;
        if (frames != null && frames.Length > 0)
            swimCoroutine = StartCoroutine(AnimateSwimming(frames));
    }

    private IEnumerator AnimateSwimming(Sprite[] frames)
    {
        int index = 0;
        while (true)
        {
            if (sr != null)
                sr.sprite = frames[index];
            index = (index + 1) % frames.Length;
            yield return new WaitForSeconds(frameRate);
        }
    }

    // Call this to restore the original appearance when leaving Level 3
    public void RestoreOriginal()
    {
        if (swimCoroutine != null)
            StopCoroutine(swimCoroutine);
        hasHelmet = false;
        if (animator != null)
            animator.enabled = true;
    }
}
