using UnityEngine;

public class SkyPuzzle : MonoBehaviour
{
    public Transform rotatingCircle;
    public float rotationSpeed = 200f;

    public float successStart = 90f;
    public float successEnd = 180f;

    public SkyLevelManager manager;

    void Update()
    {
        rotatingCircle.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);

        if (Input.GetMouseButtonDown(0))
        {
            float angle = rotatingCircle.eulerAngles.z;

            if (angle >= successStart && angle <= successEnd)
            {
                Debug.Log("SUCCESS");
                manager.StopFalling();
            }
            else
            {
                Debug.Log("MISS");
            }
        }
    }
}