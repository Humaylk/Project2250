using UnityEngine;

// Represents the rocks at the ocean floor that block the exit doorway in Level 3.
// Tracks whether rocks have been removed by the player to create the escape opening
// required for level completion.
// Implements IInteractable so the player's InteractionSystem detects and activates it via E key.
// Could be generalized into an Obstacle or EnvironmentalBarrier class for use in other levels
// (e.g., collapsed walls, frozen gates, corrupted vines).
public class RockBarrier : MonoBehaviour, IInteractable
{
    public bool isCleared = false;

    private Collider2D barrierCollider;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        barrierCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Returns whether the rocks have been cleared
    public bool IsCleared() => isCleared;

    // Called by InteractionSystem when player presses E nearby
    public void Interact()
    {
        if (isCleared) return;

        isCleared = true;
        Debug.Log("RockBarrier: Bomb defused! Exit opening created.");

        // Disable the physical blocker so the player can pass through
        if (barrierCollider != null)
            barrierCollider.enabled = false;

        // Hide the bomb visuals
        if (spriteRenderer != null)
            spriteRenderer.enabled = false;

        GameManager.Instance?.progressionSystem?.AddCollectionXP(20);
        GameManager.Instance?.uiManager?.ShowHint("Bomb defused! Head for the exit doorway!");
    }

    // Returns the interaction prompt shown to the player when nearby
    // The InteractionSystem prefixes this with "[E]" automatically
    public string GetInteractPrompt()
        => isCleared ? "" : "DEFUSE THE BOMB";

    // Resets the barrier back to its blocked state (called on level reset)
    public void ResetBarrier()
    {
        isCleared = false;
        if (barrierCollider != null) barrierCollider.enabled = true;
        if (spriteRenderer != null) spriteRenderer.enabled = true;
    }
}
