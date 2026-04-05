using UnityEngine;
using System.Collections;

// Munadir: Individual laser cannon that rotates and fires projectile bullets
// Munadir: Rotates back and forth 45 degrees then fires a bullet
public class LaserCannon : MonoBehaviour
{
    [Header("Settings")]
    public float rotationRange = 45f;
    public float rotationSpeed = 30f;
    public float fireInterval = 2f;
    public float bulletSpeed = 6f;
    public int damage = 15;
    public Color cannonColor = Color.magenta;

    private bool rotatingRight = true;
    private float startAngle;
    private bool isRunning = false;

    void Start()
    {
        startAngle = transform.eulerAngles.z;

        // Create visual for the cannon itself
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = cannonColor;
            sr.sortingOrder = 6;
        }
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

    public void SetSpeed(float newSpeed)
    {
        rotationSpeed = newSpeed;
        fireInterval = Mathf.Max(0.5f, fireInterval - 0.3f);
    }

    void Update()
    {
        if (!isRunning) return;

        // Rotate back and forth
        float currentZ = transform.eulerAngles.z;
        float delta = rotatingRight ? rotationSpeed : -rotationSpeed;
        transform.Rotate(0, 0, delta * Time.deltaTime);

        float angleDiff = Mathf.DeltaAngle(transform.eulerAngles.z, startAngle);
        if (angleDiff > rotationRange) rotatingRight = false;
        if (angleDiff < -rotationRange) rotatingRight = true;
    }

    private IEnumerator FireLoop()
    {
        while (isRunning)
        {
            yield return new WaitForSeconds(fireInterval);
            if (isRunning) FireBullet();
        }
    }

    private void FireBullet()
    {
        // Create bullet object
        GameObject bullet = new GameObject("LaserBullet");
        bullet.transform.position = transform.position;

        // Visual
        SpriteRenderer sr = bullet.AddComponent<SpriteRenderer>();
        sr.sprite = MakeSprite();
        sr.color = new Color(1f, 0.2f, 1f, 1f); // bright magenta
        sr.sortingOrder = 7;
        sr.material = new Material(Shader.Find("Sprites/Default"));
        bullet.transform.localScale = new Vector3(0.3f, 0.3f, 1f);

        // Collider
        CircleCollider2D col = bullet.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = 1f;

        // Bullet movement — fires in the direction cannon is facing
        Vector2 dir = transform.up;
        LaserBullet lb = bullet.AddComponent<LaserBullet>();
        lb.direction = dir;
        lb.speed = bulletSpeed;
        lb.damage = damage;

        Destroy(bullet, 4f); // auto destroy after 4 seconds
    }

    private Sprite MakeSprite()
    {
        Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        tex.SetPixels(new Color[] { Color.white, Color.white, Color.white, Color.white });
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 2, 2), new Vector2(0.5f, 0.5f), 1f);
    }
}