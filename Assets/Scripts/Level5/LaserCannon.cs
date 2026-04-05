using UnityEngine;
using System.Collections;

// Munadir: Individual cannon logic
// Oscillates in a tight arc and fires toward the center of the arena
public class LaserCannon : MonoBehaviour
{
    [Header("Current Stats")]
    public float rotationRange = 20f;
    public float rotationSpeed = 40f;
    public float fireInterval = 2f;
    public float bulletSpeed = 7f;
    public int damage = 15;

    private float startAngle;
    private bool isRunning = false;

    void Start()
    {
        // Munadir: Capture the initial angle (facing center) set by LaserSystem
        startAngle = transform.eulerAngles.z;
        
        // Ensure visual is magenta as per project style
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.color = new Color(0.8f, 0f, 0.8f, 1f);
    }

    public void StartFiring() 
    { 
        isRunning = true; 
        StartCoroutine(FireLoop()); 
    }

    public void StopFiring() 
    { 
        isRunning = false; 
        StopAllCoroutines(); 
    }

    // Munadir: Updates stats during Phase 2/3 transitions
    public void SetSpeed(float newRotSpeed, float newFireInterval)
    {
        rotationSpeed = newRotSpeed;
        fireInterval = newFireInterval;
    }

    void Update()
    {
        if (!isRunning) return;

        // Munadir: Use Sin wave for smoother 'sweeping' motion. 
        // Multiplying speed by 0.1 to keep the oscillation controlled.
        float swing = Mathf.Sin(Time.time * (rotationSpeed * 0.1f)) * rotationRange;
        transform.rotation = Quaternion.Euler(0f, 0f, startAngle + swing);
    }

    private IEnumerator FireLoop()
    {
        // Munadir: Stagger the start of each cannon so they don't all fire at the exact same frame
        yield return new WaitForSeconds(Random.Range(0f, fireInterval));
        
        while (isRunning)
        {
            FireBullet();
            yield return new WaitForSeconds(fireInterval);
        }
    }

    private void FireBullet()
    {
        // Create the bullet object
        GameObject bullet = new GameObject("LaserBullet");
        bullet.transform.position = transform.position;

        // Visual Setup
        SpriteRenderer sr = bullet.AddComponent<SpriteRenderer>();
        sr.sprite = MakeSprite();
        sr.color = new Color(1f, 0.2f, 1f, 1f); // Bright magenta glow
        sr.sortingOrder = 7;
        sr.material = new Material(Shader.Find("Sprites/Default"));
        bullet.transform.localScale = new Vector3(0.3f, 0.3f, 1f);

        // Physics Setup
        CircleCollider2D col = bullet.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = 0.5f;

        // Munadir: Direction is transform.up because we aligned the cannons 
        // to face the center using Atan2 in the LaserSystem script.
        Vector2 dir = transform.up;

        // Add movement script
        LaserBullet lb = bullet.AddComponent<LaserBullet>();
        lb.direction = dir;
        lb.speed = bulletSpeed;
        lb.damage = damage;

        // Cleanup
        Destroy(bullet, 4f); 
    }

    private Sprite MakeSprite()
    {
        Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        tex.SetPixels(new Color[] { Color.white, Color.white, Color.white, Color.white });
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 2, 2), new Vector2(0.5f, 0.5f), 1f);
    }
}