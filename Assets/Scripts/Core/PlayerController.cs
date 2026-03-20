using UnityEngine;

// Munadir: Fixed movement axes for 2D top-down game
public class PlayerController : MonoBehaviour
{
    public float speed = 5f;

    void Update()
    {
        // Munadir: GetAxisRaw gives clean input without smoothing
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        // Munadir: Normalized so diagonal movement isnt faster
        Vector2 move = new Vector2(moveX, moveY).normalized;
        transform.Translate(move * speed * Time.deltaTime);
    }
}