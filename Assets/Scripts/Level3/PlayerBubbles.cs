using UnityEngine;

// Attaches to a child GameObject on Alex.
// The entire Level 3 scene is underwater, so bubbles are always
// visible and looping for the full duration of the level.
// Follows Alex automatically since it is parented to him.
public class PlayerBubbles : MonoBehaviour
{
    private SpriteRenderer bubbleRenderer;
    private Animator bubbleAnimator;

    void Start()
    {
        bubbleRenderer = GetComponent<SpriteRenderer>();
        bubbleAnimator = GetComponent<Animator>();

        // Always underwater — bubbles on from the start
        SetBubblesVisible(true);
    }

    private void SetBubblesVisible(bool visible)
    {
        if (bubbleRenderer != null)
            bubbleRenderer.enabled = visible;

        if (bubbleAnimator != null)
            bubbleAnimator.enabled = visible;
    }
}
