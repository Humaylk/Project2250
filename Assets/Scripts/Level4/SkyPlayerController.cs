using UnityEngine;

public class SkyPlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float fallSpeed = 0.8f; // slow fall

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(h, v, 0f);
        transform.position += move * moveSpeed * Time.deltaTime;

        // slow falling
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;
    }
}