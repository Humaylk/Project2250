using UnityEngine;
using System.Collections;

// Munadir: Individual cannon that sweeps in an arc and fires bullet projectiles
// Munadir: Bullets use assigned sprite or fallback to magenta square
public class LaserCannon : MonoBehaviour
{
    [Header("Current Stats")]
    public float rotationRange = 25f;
    public float rotationSpeed = 30f;
    public float fireInterval = 4f;
    public float bulletSpeed = 4f;
    public int damage = 5;

    [Header("Bullet Visual")]
    public Sprite bulletSprite;

    private float startAngle;
    private bool isRunning = false;

    void Start()
    {
        startAngle = transform.eulerAngles.z;

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

    public void SetSpeed(float newRotSpeed, float newFireInterval)
    {
        rotationSpeed = newRotSpeed;
        fireInterval = newFireInterval;
    }

    void Update()
    {
        if (!isRunning) return;

        float swing = Mathf.Sin(Time.time * (rotationSpeed * 0.1f)) * rotationRange;
        transform.rotation = Quaternion.Euler(0f, 0f, startAngle + swing);
    }

    private IEnumerator FireLoop()
    {
        // Munadir: Stagger so cannons don't fire simultaneously
        yield return new WaitForSeconds(Random.Range(0f, fireInterval));

        while (isRunning)
        {
            FireBullet();
            yield return new WaitForSeconds(fireInterval);
        }
    }

    private void FireBullet()
    {
        GameObject bullet = new GameObject("LaserBullet");
        bullet.transform.position = transform.position;

        SpriteRenderer sr = bullet.AddComponent<SpriteRenderer>();

        if (bulletSprite != null)
        {
            sr.sprite = bulletSprite;
        }
        else
        {
            sr.sprite = MakeSprite();
        }

        sr.color = new Color(1f, 0.2f, 1f, 1f);
        sr.sortingOrder = 7;
        sr.material = new Material(Shader.Find("Sprites/Default"));
        bullet.transform.localScale = new Vector3(0.4f, 0.4f, 1f);

        CircleCollider2D col = bullet.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = 0.4f;

        Vector2 dir = transform.up;

        LaserBullet lb = bullet.AddComponent<LaserBullet>();
        lb.direction = dir;
        lb.speed = bulletSpeed;
        lb.damage = damage;

        Destroy(bullet, 5f);
    }

    private Sprite MakeSprite()
    {
        Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        tex.SetPixels(new Color[] { Color.white, Color.white, Color.white, Color.white });
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 2, 2), new Vector2(0.5f, 0.5f), 1f);
    }
}
