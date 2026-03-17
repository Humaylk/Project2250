using UnityEngine;


public class Gate : MonoBehaviour
{
    [Header("Gate Settings")]
    [SerializeField] protected bool isOpen = false;
    [SerializeField] protected bool blocksCollisionWhenClosed = true;

    // Reference to the physical collider to toggle pass-through
    protected Collider2D gateCollider;
    
    // Optional: Reference to an animator for visual opening/closing
    protected Animator animator;

    protected virtual void Awake()
    {
        gateCollider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
    }

    protected virtual void Start()
    {
        // Initialize the gate state based on the document's reset requirements
        ResetGate();
    }

    /// <summary>
    /// Logic to open the gate, allowing player pass-through.
    /// </summary>
    public virtual void OpenGate()
    {
        isOpen = true;
        
        if (gateCollider != null && blocksCollisionWhenClosed)
        {
            gateCollider.isTrigger = true; // Allow passing through
        }

        if (animator != null)
        {
            animator.SetBool("IsOpen", true);
        }

        Debug.Log($"{gameObject.name}: Gate is now OPEN.");
    }

    /// <summary>
    /// Logic to close the gate and block collision.
    /// </summary>
    public virtual void CloseGate()
    {
        isOpen = false;

        if (gateCollider != null && blocksCollisionWhenClosed)
        {
            gateCollider.isTrigger = false; // Block collision
        }

        if (animator != null)
        {
            animator.SetBool("IsOpen", false);
        }

        Debug.Log($"{gameObject.name}: Gate is now CLOSED.");
    }

    /// <summary>
    /// Resets the gate to its default closed state for level restarts.
    /// </summary>
    public virtual void ResetGate()
    {
        CloseGate();
    }

    /// <summary>
    /// Determines if the player can pass through the gate.
    /// </summary>
    /// <returns>True if the gate is open.</returns>
    public bool CanPassThrough()
    {
        return isOpen;
    }

    // Triggered when something enters the gate area (e.g., the player escaping)
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (isOpen && other.CompareTag("Player"))
        {
            OnPlayerEnterExit();
        }
    }

    /// <summary>
    /// Logic to execute when the player successfully reaches the open exit.
    /// This can be overridden to trigger level transitions via the GameManager.
    /// </summary>
    protected virtual void OnPlayerEnterExit()
    {
        Debug.Log("Player has reached the exit!");
        // Example: GameManager.Instance.AdvanceLevel();
    }
}