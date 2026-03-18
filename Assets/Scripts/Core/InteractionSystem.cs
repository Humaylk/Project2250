using UnityEngine;

// <summary>
// This system attaches to the Player GameObject. It is responsible for continuously scanning the environment
// for objects that implement the IInteractable interface within a specific range.
// It manages displaying the interaction prompt via the UIManager and handles triggering the interaction
// when the player presses the designated interaction key (default 'E').
// </summary>
public class InteractionSystem : MonoBehaviour
{
    [Header("Settings")]
    // <summary>
    // The circular radius around the player within which interactable objects can be detected.
    // </summary>
    public float interactionRange = 1.5f;

    // <summary>
    // The current prompt text received from the nearest interactable object (e.g., "Open Chest", "Talk").
    // </summary>
    public string currentPrompt = "";

    // --- Private Fields for Internal Logic ---

    // Reference to the nearest object's IInteractable component found during the last scan.
    private IInteractable nearestInteractable;
    // Reference to the UIManager to display/clear prompts.
    private UIManager uiManager;

    void Start()
    {
        // Cache the reference to the UIManager present in the scene.
        uiManager = FindObjectOfType<UIManager>();
    }

    void Update()
    {
        // 1. Every frame, scan the environment for nearby interactable objects.
        ScanForInteractables();

        // 2. If an interactable object is nearby AND the player presses the 'E' key...
        if (nearestInteractable != null && Input.GetKeyDown(KeyCode.E))
        {
            // ...trigger the interaction logic.
            TriggerInteraction();
        }
    }

    // <summary>
    // Public helper method (used by UIManager or other scripts) to check if an interaction is currently possible.
    // It forces a quick scan and returns true if an interactable object is found.
    // </summary>
    // <returns>True if an interactable object is within range, false otherwise.</returns>
    public bool DetectInteraction()
    {
        ScanForInteractables();
        return nearestInteractable != null;
    }

    // <summary>
    // Executes the primary interaction logic by calling the Interact() method on the nearest object.
    // </summary>
    public void TriggerInteraction()
    {
        // Safety check to ensure we still have a valid target.
        if (nearestInteractable == null) return;

        Debug.Log("Interacting: " + currentPrompt);
        // Call the Interact method defined by the object implementing IInteractable.
        // This decouples the InteractionSystem from specific object logic (chests, NPCs, etc.).
        nearestInteractable.Interact();
    }

    // <summary>
    // Sends the interaction prompt text to the UIManager to be displayed as a hint.
    // </summary>
    // <param name="prompt">The prompt text to show.</param>
    public void ShowPrompt(string prompt)
    {
        uiManager?.ShowHint(prompt);
    }

    // <summary>
    // Performed every Update frame. Uses Physics2D to detect all colliders within interactionRange,
    // filters them for the IInteractable component, and identifies the closest valid target.
    // It then updates the current target reference and tells the UIManager to show or clear the prompt.
    // </summary>
    private void ScanForInteractables()
    {
        // Perform a spatial query to find all 2D colliders within a circle centered on the player.
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactionRange);
        
        float closest = float.MaxValue;
        IInteractable best = null;

        // Iterate through all colliders found in the circle.
        foreach (Collider2D hit in hits)
        {
            // Ignore the Player's own collider.
            if (hit.gameObject == gameObject) continue;

            // Check if the detected object has a component that implements IInteractable.
            IInteractable interactable = hit.GetComponent<IInteractable>();
            
            // If it doesn't implement the interface, skip it.
            if (interactable == null) continue;

            // Calculate the distance between the player and the detected object.
            float dist = Vector2.Distance(transform.position, hit.transform.position);
            
            // If this object is closer than any valid object found so far in this scan...
            if (dist < closest)
            {
                // ...update the closest distance and set this object as the "best" candidate.
                closest = dist;
                best = interactable;
            }
        }

        // --- Update Target and UI State ---

        // If the best candidate found *this frame* is different from the target found *last frame*...
        if (best != nearestInteractable)
        {
            // ...update the private reference to the new nearest interactable.
            nearestInteractable = best;

            // Get the prompt text from the new object (or an empty string if nothing was found).
            currentPrompt = best != null ? best.GetInteractPrompt() : "";

            // If a valid prompt exists...
            if (currentPrompt != "")
                // ...tell the UIManager to show the prompt, prefixed with the key binding "[E]".
                ShowPrompt("[E] " + currentPrompt);
            else
                // ...otherwise, nothing is nearby, so tell the UIManager to clear any active messages.
                uiManager?.ClearMessages();
        }
    }

    // <summary>
    // Unity Editor callback method. Draws a visual representation of the interaction range
    // in the Scene view when the Player GameObject is selected, aiding in level design and debugging.
    // </summary>
    void OnDrawGizmosSelected()
    {
        // Set the gizmo color to cyan.
        Gizmos.color = Color.cyan;
        // Draw a wireframe sphere representing the interaction range.
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}