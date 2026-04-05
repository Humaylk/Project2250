using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;

    private Animator animator;

    void Start()
    {

        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");


        Vector2 move = new Vector2(moveX, moveY).normalized;


        transform.Translate(move * speed * Time.deltaTime);


        if (animator != null)
        {
            bool isMoving = move != Vector2.zero;
            animator.SetBool("isRunning", isMoving);
        }
    }
}