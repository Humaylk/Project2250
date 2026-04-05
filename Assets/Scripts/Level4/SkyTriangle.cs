using UnityEngine;

public class SkyTriangle : MonoBehaviour
{
    public float rotateSpeed = 150f;
    public bool solved = false;

    public float interactDistance = 2f;

    public Transform player; // 👈 assign manually
    public SkyGameManager manager;

    void Update()
    {
        if (!solved && player != null)
        {
            transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);

            float dist = Vector2.Distance(transform.position, player.position);

            Debug.Log("Distance to triangle: " + dist);

            if (dist < interactDistance)
            {
                Debug.Log("Player in range");

                if (Input.GetKeyDown(KeyCode.E))
                {
                    Debug.Log("E pressed");

                    solved = true;

                    if (manager != null)
                        manager.AddProgress();

                    Debug.Log("Triangle solved!");
                }
            }
        }
    }
}