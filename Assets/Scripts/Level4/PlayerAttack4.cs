using UnityEngine;

public class PlayerAttack4 : MonoBehaviour
{
    public float attackRange = 2f;
    public int damage = 10;
    public AudioClip hitSound;

    private Animator     _animator;
    private AudioSource  _audio;
    private int          _attackIndex = 0;
    private bool         _gHeld = false;

    void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _audio = gameObject.AddComponent<AudioSource>();
        _audio.playOnAwake = false;
        _audio.spatialBlend = 0f;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.G) && !_gHeld)
        {
            _gHeld = true;
            Attack();
        }
        if (Input.GetKeyUp(KeyCode.G))
            _gHeld = false;
    }

    void Attack()
    {
        // Cycle through Attack1 / Attack2 / Attack3
        _attackIndex = (_attackIndex % 3) + 1;
        if (_animator != null)
            _animator.SetTrigger("Attack" + _attackIndex);

        if (hitSound != null)
            _audio.PlayOneShot(hitSound, 1.5f);

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange);

        foreach (Collider2D hit in hits)
        {
            GolemHealth gh = hit.GetComponent<GolemHealth>();

            if (gh != null)
            {
                gh.TakeDamage(damage);
                Debug.Log("Hit golem!");
            }
        }
    }

    // optional: show attack range in scene
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}