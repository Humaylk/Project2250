using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackRange = 2f;

    [Header("Audio")]
    public AudioClip hitSound;

    private Animator    animator;
    private PlayerWeapon weapon;
    private AudioSource  audioSource;

    private bool  gHeld          = false;
    private int   currentAttack  = 0;       // cycles 1 → 2 → 3 → 1
    private float timeSinceAttack = 0f;

    private void Start()
    {
        animator  = GetComponentInChildren<Animator>();
        weapon    = GetComponent<PlayerWeapon>();
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake  = false;
        audioSource.spatialBlend = 0f;
    }

    private void Update()
    {
        timeSinceAttack += Time.deltaTime;

        if (Input.GetKey(KeyCode.G) && !gHeld)
        {
            gHeld = true;
            TriggerAttack();
            DealDamage();
        }
        if (Input.GetKeyUp(KeyCode.G))
            gHeld = false;
    }

    private void TriggerAttack()
    {
        if (animator == null) return;

        // Advance combo; reset if gap between attacks is too long
        if (timeSinceAttack > 1.0f)
            currentAttack = 0;

        currentAttack++;
        if (currentAttack > 3)
            currentAttack = 1;

        timeSinceAttack = 0f;

        // HeroKnight uses Attack1 / Attack2 / Attack3 triggers
        animator.SetTrigger("Attack" + currentAttack);

        if (hitSound != null)
            audioSource.PlayOneShot(hitSound);
    }

    private void DealDamage()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange);
        foreach (Collider2D hit in hits)
        {
            if (!hit.CompareTag("Enemy")) continue;

            int damage = weapon != null ? weapon.GetDamage() : 1;
            EnemyHealth enemyHealth = hit.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
                enemyHealth.TakeDamage(damage);

            GameManager.Instance?.progressionSystem?.AddCombatXP();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
