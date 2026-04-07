using UnityEngine;

// Munadir: Manages all player abilities for Level 5 boss fight
// Munadir: G = Sword melee (uses PlayerAttack combo), also damages boss via IDamageable
// Munadir: F = Fireball projectile (ranged, 3s cooldown, spawns visible fireball)
// Munadir: P = Heavy Attack (close range, 2s cooldown, high damage)
public class AbilityManager : MonoBehaviour
{
    [Header("Fire Ability (F key)")]
    public float fireDamage = 40f;
    public float fireCooldown = 3f;
    public float fireRange = 10f;
    public Sprite fireballSprite;

    [Header("Heavy Attack (P key)")]
    public float heavyDamage = 30f;
    public float heavyCooldown = 2f;
    public float heavyRange = 5f;

    [Header("Sword Boss Damage (G key)")]
    public float swordRange = 4f;
    public int swordDamage = 15;

    [Header("Audio")]
    public AudioClip fireSound;
    public AudioClip heavySound;
    public AudioClip swordHitSound;

    [Header("References")]
    public UIManager uiManager;
    public ElementalBoss boss;

    private float lastFireTime = -999f;
    private float lastHeavyTime = -999f;
    private Animator playerAnimator;
    private AudioSource audioSource;
    private bool gHeld = false;

    void Start()
    {
        playerAnimator = GetComponentInChildren<Animator>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f;
        }
    }

    void Update()
    {
        // Munadir: F key = Fireball
        if (Input.GetKeyDown(KeyCode.F))
            UseFireAbility();

        // Munadir: P key = Heavy Attack
        if (Input.GetKeyDown(KeyCode.P))
            UseHeavyAttack();

        // Munadir: G key = Sword also damages boss (PlayerAttack handles animation)
        if (Input.GetKey(KeyCode.G) && !gHeld)
        {
            gHeld = true;
            UseSwordOnBoss();
        }
        if (Input.GetKeyUp(KeyCode.G))
            gHeld = false;
    }

    private void UseSwordOnBoss()
    {
        if (boss == null || boss.IsDefeated()) return;

        float dist = Vector2.Distance(transform.position, boss.transform.position);
        if (dist <= swordRange)
        {
            boss.TakeDamage(swordDamage);
            uiManager?.ShowHint("Sword hit! " + swordDamage + " damage!");

            if (swordHitSound != null && audioSource != null)
                audioSource.PlayOneShot(swordHitSound);
        }
    }

    private void UseFireAbility()
    {
        if (Time.time - lastFireTime < fireCooldown)
        {
            float remaining = fireCooldown - (Time.time - lastFireTime);
            uiManager?.ShowHint("Fireball cooling down: " + remaining.ToString("F1") + "s");
            return;
        }

        if (boss == null || boss.IsDefeated()) return;

        float dist = Vector2.Distance(transform.position, boss.transform.position);
        if (dist > fireRange)
        {
            uiManager?.ShowHint("Too far for Fireball!");
            return;
        }

        lastFireTime = Time.time;

        // Munadir: Play attack animation as cast gesture
        if (playerAnimator != null)
            playerAnimator.SetTrigger("Attack1");

        if (fireSound != null && audioSource != null)
            audioSource.PlayOneShot(fireSound);

        // Munadir: Spawn fireball projectile that flies to boss
        SpawnFireball();
    }

    private void SpawnFireball()
    {
        GameObject fireball = new GameObject("Fireball");
        fireball.transform.position = transform.position + Vector3.right * 0.5f;

        // Munadir: Visual
        SpriteRenderer sr = fireball.AddComponent<SpriteRenderer>();
        if (fireballSprite != null)
        {
            sr.sprite = fireballSprite;
        }
        else
        {
            // Munadir: Fallback if no sprite assigned - orange circle
            Texture2D tex = new Texture2D(4, 4, TextureFormat.RGBA32, false);
            for (int x = 0; x < 4; x++)
                for (int y = 0; y < 4; y++)
                    tex.SetPixel(x, y, new Color(1f, 0.5f, 0f, 1f));
            tex.Apply();
            sr.sprite = Sprite.Create(tex, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 4f);
        }
        sr.color = new Color(1f, 0.6f, 0.1f, 1f);
        sr.sortingOrder = 10;
        sr.material = new Material(Shader.Find("Sprites/Default"));
        fireball.transform.localScale = new Vector3(1.5f, 1.5f, 1f);

        // Munadir: Collider
        CircleCollider2D col = fireball.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = 0.3f;

        // Munadir: Movement script
        FireballProjectile fp = fireball.AddComponent<FireballProjectile>();
        fp.target = boss.transform;
        fp.damage = (int)fireDamage;
        fp.speed = 8f;
        fp.boss = boss;

        Object.Destroy(fireball, 3f);
    }

    private void UseHeavyAttack()
    {
        if (Time.time - lastHeavyTime < heavyCooldown)
        {
            float remaining = heavyCooldown - (Time.time - lastHeavyTime);
            uiManager?.ShowHint("Heavy Attack cooling down: " + remaining.ToString("F1") + "s");
            return;
        }

        if (boss == null || boss.IsDefeated()) return;

        float dist = Vector2.Distance(transform.position, boss.transform.position);
        if (dist > heavyRange)
        {
            uiManager?.ShowHint("Get closer for Heavy Attack!");
            return;
        }

        lastHeavyTime = Time.time;

        // Munadir: Use Attack3 (heaviest swing) for visual
        if (playerAnimator != null)
            playerAnimator.SetTrigger("Attack3");

        if (heavySound != null && audioSource != null)
            audioSource.PlayOneShot(heavySound);

        boss.TakeDamage((int)heavyDamage);
        uiManager?.ShowHint("Heavy Attack! " + heavyDamage + " damage!");
        GameManager.Instance?.progressionSystem?.AddCombatXP(10);
    }
}
