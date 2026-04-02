using UnityEngine;

public class SkyTriangle : MonoBehaviour
{
    public float rotateSpeed = 150f;
    public bool solved = false;

    public float interactDistance = 2f;
    private Transform player;

    public SkyGameManager manager;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (!solved)
        {
            transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);

            float dist = Vector2.Distance(transform.position, player.position);

            if (dist < interactDistance && Input.GetKeyDown(KeyCode.E))
            {
                solved = true;
                manager.AddProgress();
                Debug.Log("Triangle solved!");
            }
        }
    }
}