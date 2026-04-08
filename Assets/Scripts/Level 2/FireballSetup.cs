using UnityEngine;

// Add this script to any GameObject in the Level 2 scene (e.g. a Manager object).
// At runtime it finds every Fireball_0 object and gives it a PolygonCollider2D trigger
// + FireDamage script so the player takes damage on contact.
public class FireballSetup : MonoBehaviour
{
    [Tooltip("Damage dealt per tick")]
    public int damageAmount = 5;

    [Tooltip("Seconds between each damage tick")]
    public float damageInterval = 1f;

    void Awake()
    {
        int count = 0;

        GameObject[] all = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach (GameObject go in all)
        {
            if (!go.name.StartsWith("Fireball_0")) continue;

            // --- PolygonCollider2D ---
            // Remove any old collider types first to avoid duplicates
            foreach (var old in go.GetComponents<CircleCollider2D>())
                Destroy(old);
            foreach (var old in go.GetComponents<BoxCollider2D>())
                Destroy(old);

            PolygonCollider2D poly = go.GetComponent<PolygonCollider2D>();
            if (poly == null)
                poly = go.AddComponent<PolygonCollider2D>();

            // Simple diamond shape that fits a typical fireball sprite (~1.7 x 2.3 units)
            poly.SetPath(0, new Vector2[]
            {
                new Vector2( 0.00f,  1.00f),  // top
                new Vector2( 0.65f,  0.30f),  // right-upper
                new Vector2( 0.65f, -0.30f),  // right-lower
                new Vector2( 0.00f, -1.00f),  // bottom
                new Vector2(-0.65f, -0.30f),  // left-lower
                new Vector2(-0.65f,  0.30f),  // left-upper
            });
            poly.isTrigger = true;

            // --- FireDamage ---
            FireDamage fd = go.GetComponent<FireDamage>();
            if (fd == null)
                fd = go.AddComponent<FireDamage>();

            fd.damageAmount   = damageAmount;
            fd.damageInterval = damageInterval;

            count++;
        }

        Debug.Log("[FireballSetup] Added PolygonCollider2D + FireDamage to " + count + " Fireball_0 objects.");
    }
}
