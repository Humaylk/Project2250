using UnityEngine;

// Attach to the Dragon GameObject in Level 2.
// Moves it up and down smoothly using a sine wave — no animation clips needed.
public class DragonHover : MonoBehaviour
{
    [Tooltip("How far up and down the dragon moves (in world units)")]
    public float amplitude = 0.2f;

    [Tooltip("How fast the dragon bobs up and down")]
    public float speed = 1.2f;

    private Vector3 _startPosition;

    void Start()
    {
        _startPosition = transform.position;
    }

    void Update()
    {
        float offset = Mathf.Sin(Time.time * speed) * amplitude;
        transform.position = _startPosition + new Vector3(0f, offset, 0f);
    }
}
