using UnityEngine;

public class FireDamage : MonoBehaviour
{
    public int   damageAmount   = 5;
    public float damageInterval = 2.5f;

    private bool         playerInside = false;
    private float        timer        = 0f;
    private PlayerHealth playerHealth;

    // Cache the global PlayerHealth once so every fireball doesn't search repeatedly
    private static PlayerHealth _cachedPlayerHealth;

    void Start()
    {
        // Pre-cache PlayerHealth at scene start
        if (_cachedPlayerHealth == null)
            _cachedPlayerHealth = FindFirstObjectByType<PlayerHealth>();
    }

    void Update()
    {
        if (!playerInside) return;

        // Ensure we always have a reference even if it was found late
        if (playerHealth == null)
            playerHealth = _cachedPlayerHealth ?? FindFirstObjectByType<PlayerHealth>();

        if (playerHealth == null) return;

        timer += Time.deltaTime;
        if (timer >= damageInterval)
        {
            playerHealth.TakeDamage(damageAmount);
            timer = 0f;
            Debug.Log("[FireDamage] Tick damage: " + damageAmount);
        }
    }

    // --- Trigger-based (collider isTrigger = true) ---
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsPlayer(other)) return;

        playerHealth = ResolvePlayerHealth(other);
        playerInside = true;
        timer        = damageInterval; // first hit is immediate
        Debug.Log("[FireDamage] Player entered fire (trigger).");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!IsPlayer(other)) return;
        playerInside = false;
        timer        = 0f;
    }

    // --- Collision-based (collider isTrigger = false) ---
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!IsPlayer(other.collider)) return;

        playerHealth = ResolvePlayerHealth(other.collider);
        playerInside = true;
        timer        = damageInterval; // first hit is immediate
        Debug.Log("[FireDamage] Player entered fire (collision).");
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (!IsPlayer(other.collider)) return;
        playerInside = false;
        timer        = 0f;
    }

    // --- Helpers ---
    private bool IsPlayer(Collider2D col)
    {
        // Match HeroKnight by component (it's tagged Untagged)
        return col.CompareTag("Player")
            || col.GetComponent<HeroKnight>()               != null
            || col.GetComponentInParent<HeroKnight>()       != null
            || col.GetComponent<PlayerController>()         != null
            || col.GetComponentInParent<PlayerController>() != null;
    }

    private PlayerHealth ResolvePlayerHealth(Collider2D col)
    {
        // Try the collider and its parents first
        PlayerHealth ph = col.GetComponent<PlayerHealth>();
        if (ph == null) ph = col.GetComponentInParent<PlayerHealth>();

        // Fall back to global search (handles HeroKnight child colliders)
        if (ph == null)
            ph = _cachedPlayerHealth ?? FindFirstObjectByType<PlayerHealth>();

        if (ph != null) _cachedPlayerHealth = ph; // keep cache warm
        return ph;
    }
}
