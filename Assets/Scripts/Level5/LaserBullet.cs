using UnityEngine;

// Munadir: Projectile fired by LaserCannon - moves in straight line and damages player
public class LaserBullet : MonoBehaviour
{
    public Vector2 direction;
    public float speed = 6f;
    public int damage = 15;

    private float lastHitTime = -999f;

    void Update()
    {
        // Move in firing direction
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealth>()?.TakeDamage(damage);
            Debug.Log("Laser bullet hit player for " + damage);
            Destroy(gameObject);
        }

        // Destroy on hitting walls
        if (other.CompareTag("Wall"))
            Destroy(gameObject);
    }
}