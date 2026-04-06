using UnityEngine;

public class LaserBullet : MonoBehaviour
{
    public Vector2 direction;
    public float speed = 7f;
    public int damage = 15;

    void Update()
    {
        // Munadir: Moves the bullet in the direction the cannon was facing when it fired
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealth>()?.TakeDamage(damage);
            Destroy(gameObject);
        }
        
        // Munadir: Destroy bullet if it hits any object tagged "Wall"
        if (other.CompareTag("Wall"))
            Destroy(gameObject);
    }
}