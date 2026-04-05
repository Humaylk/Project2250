using UnityEngine;

public class SkyPlayerFall : MonoBehaviour
{
    public float fallSpeed = 3f;

    void Update()
    {
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;
    }
}