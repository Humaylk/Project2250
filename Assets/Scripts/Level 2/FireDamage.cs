using UnityEngine;

public class FireDamage : MonoBehaviour
{
    public int damageAmount = 5;
    public float damageInterval = 1f;

    private bool playerInside = false;
    private float timer = 0f;
    private PlayerHealth playerHealth;

    void Update()
    {
        if (playerInside && playerHealth != null)
        {
            timer += Time.deltaTime;

            if (timer >= damageInterval)
            {
                playerHealth.TakeDamage(damageAmount);
                timer = 0f;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            playerHealth = other.GetComponent<PlayerHealth>();
            timer = 0f;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            playerHealth = null;
            timer = 0f;
        }
    }
}