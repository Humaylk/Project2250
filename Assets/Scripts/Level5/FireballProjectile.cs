using UnityEngine;

// Munadir: Fireball that flies toward the boss and deals damage on contact
// Munadir: Spawned by AbilityManager when player presses F
public class FireballProjectile : MonoBehaviour
{
    public Transform target;
    public int damage = 40;
    public float speed = 8f;
    public ElementalBoss boss;

    private Vector2 direction;
    private bool hasHit = false;

    void Start()
    {
        if (target != null)
            direction = ((Vector2)target.position - (Vector2)transform.position).normalized;
        else
            direction = Vector2.right;
    }

    void Update()
    {
        if (hasHit) return;
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;

        // Munadir: Check if we hit the boss
        if (boss != null && other.gameObject == boss.gameObject)
        {
            hasHit = true;
            boss.TakeDamage(damage);
            boss.TriggerFireAnimation();
            GameManager.Instance?.progressionSystem?.AddCombatXP(15);
            GameManager.Instance?.uiManager?.ShowHint("Fireball hit! " + damage + " damage!");
            Destroy(gameObject);
        }
    }
}
