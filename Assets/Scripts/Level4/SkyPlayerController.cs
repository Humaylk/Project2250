using UnityEngine;

public class SkyPlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float fallSpeed = 0.8f; // slow fall

    private Animator       _animator;
    private SpriteRenderer _sr;

    void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _sr       = GetComponentInChildren<SpriteRenderer>();

        if (_animator != null)
            _animator.SetBool("Grounded", true);
    }

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(h, v, 0f);
        transform.position += move * moveSpeed * Time.deltaTime;

        // slow falling
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;

        // Animations
        bool isMoving = Mathf.Abs(h) > 0.01f || Mathf.Abs(v) > 0.01f;
        if (_animator != null)
            _animator.SetInteger("AnimState", isMoving ? 1 : 0);

        // Flip sprite to face movement direction
        if (_sr != null)
        {
            if      (h > 0.01f)  _sr.flipX = false;
            else if (h < -0.01f) _sr.flipX = true;
        }
    }
}