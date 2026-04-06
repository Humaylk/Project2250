using UnityEngine;

// Munadir: Attached to each laser beam - deals damage when player touches it
public class LaserDamage : MonoBehaviour
{
    public int damage = 15;
    private float lastHitTime = 0f;
    public float hitCooldown = 0.5f; // damage every 0.5 seconds

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (Time.time - lastHitTime >= hitCooldown)
            {
                other.GetComponent<PlayerHealth>()?.TakeDamage(damage);
                lastHitTime = Time.time;
                Debug.Log("Laser hit player for " + damage);
            }
        }
    }
}