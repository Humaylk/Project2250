using UnityEngine;

// Munadir: Updated Gate to implement IInteractable interface
// Munadir: Added H key interaction for player to advance level when gate is open
// Munadir: Added proximity detection so hints show when player is nearby
// Munadir: Connected to GameManager and UIManager for level advancement and hints
public class Gate : MonoBehaviour, IInteractable
{
    // Munadir: Changed to public so CrackedForestLevel can check gate.isOpen
    public bool isOpen = false;
    [SerializeField] protected bool blocksCollisionWhenClosed = true;

    protected Collider2D gateCollider;
    protected Animator animator;

    // Munadir: Track if player is nearby to enable H key interaction
    private bool playerNearby = false;

    protected virtual void Awake()
    {
        gateCollider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
    }

    protected virtual void Start()
    {
        ResetGate();
    }

    void Update()
    {
        // When the gate opens, the player may already be overlapping (was blocked by solid collider).
        // OnTriggerEnter2D won't fire in that case, so do a direct overlap check as fallback.
        if (isOpen && !playerNearby)
        {
            Collider2D hit = Physics2D.OverlapCircle(transform.position, 2f);
            if (hit != null && hit.CompareTag("Player"))
                playerNearby = true;
        }

        // Munadir: H key advances level if player is near an open gate
        if (playerNearby && Input.GetKeyDown(KeyCode.H))
        {
            if (isOpen)
            {
                Debug.Log("Player entered gate - advancing level!");
                // Munadir: Award puzzle XP when player completes the level
                GameManager.Instance?.progressionSystem?.AddPuzzleXP(30);
                GameManager.Instance?.AdvanceLevel();
            }
            else
            {
                // Munadir: Tell player what they need to do
                GameManager.Instance?.uiManager?.ShowHint("Gate is sealed. Align all pillars first!");
            }
        }
    }

    public virtual void OpenGate()
    {
        isOpen = true;

        // Munadir: Make collider a trigger so player can pass through
        if (gateCollider != null && blocksCollisionWhenClosed)
            gateCollider.isTrigger = true;

        if (animator != null)
            animator.SetBool("IsOpen", true);

        Debug.Log(gameObject.name + ": Gate is now OPEN.");

        // Munadir: Notify player via UIManager that gate is open
        GameManager.Instance?.uiManager?.ShowHint("Gate is open! Press H to advance!");
    }

    public virtual void CloseGate()
    {
        isOpen = false;

        // Munadir: Block collision when gate is closed
        if (gateCollider != null && blocksCollisionWhenClosed)
            gateCollider.isTrigger = false;

        if (animator != null)
            animator.SetBool("IsOpen", false);

        Debug.Log(gameObject.name + ": Gate is now CLOSED.");
    }

    public virtual void ResetGate() => CloseGate();
    public bool CanPassThrough() => isOpen;

    // Munadir: IInteractable implementation - called by InteractionSystem
    public void Interact()
    {
        if (!isOpen)
            GameManager.Instance?.uiManager?.ShowHint("Gate is sealed. Align all pillars first!");
    }

    // Munadir: IInteractable implementation - shows context hint to player
    public string GetInteractPrompt()
        => isOpen ? "Press H to advance" : "Gate is sealed";

    // Munadir: Detect when player enters gate trigger zone
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            if (isOpen)
                GameManager.Instance?.uiManager?.ShowHint("Press H to advance to next island!");
            else
                GameManager.Instance?.uiManager?.ShowHint("Gate is sealed. Align all pillars first!");
        }
    }

    // Munadir: Clear proximity flag when player leaves gate area
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerNearby = false;
    }
}