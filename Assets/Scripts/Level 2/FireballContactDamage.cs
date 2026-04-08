using UnityEngine;

// Automatically added to Fireball_0 objects by FireballSetup at runtime.
// Deals damage when the player touches the fireball, with a cooldown.
public class FireballContactDamage : MonoBehaviour
{
    public int   damageAmount  = 5;
    public float damageCooldown = 1f;

    private float _cooldownTimer = 0f;

    void Update()
    {
        if (_cooldownTimer > 0f)
            _cooldownTimer -= Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryDamage(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryDamage(other);
    }

    private void TryDamage(Collider2D other)
    {
        if (_cooldownTimer > 0f) return;
        if (!IsPlayer(other)) return;

        PlayerHealth ph = other.GetComponent<PlayerHealth>();
        if (ph == null) ph = other.GetComponentInParent<PlayerHealth>();
        if (ph == null) ph = FindFirstObjectByType<PlayerHealth>(); // global fallback
        if (ph == null) return;

        ph.TakeDamage(damageAmount);
        _cooldownTimer = damageCooldown;
        Debug.Log("[FireballContactDamage] Hit player for " + damageAmount);
    }

    private bool IsPlayer(Collider2D col)
    {
        return col.CompareTag("Player")
            || col.GetComponent<HeroKnight>()               != null
            || col.GetComponentInParent<HeroKnight>()       != null
            || col.GetComponent<PlayerController>()         != null
            || col.GetComponentInParent<PlayerController>() != null;
    }
}
