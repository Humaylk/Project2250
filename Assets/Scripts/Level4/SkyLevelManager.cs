using UnityEngine;

public class SkyLevelManager : MonoBehaviour
{
    public GameObject player;
    public float fallSpeed = 2f;

    void Update()
    {
        if (player != null)
        {
            player.transform.position += Vector3.down * fallSpeed * Time.deltaTime;
        }
    }

    public void StopFalling()
    {
        fallSpeed = 0f;
    }
}