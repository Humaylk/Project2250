using UnityEngine;

// Shifts the visual sprite independently of the GameObject's transform.
// The Animator still drives the SpriteRenderer on the root (which is hidden);
// this component copies every frame's sprite to a child at the desired offset.
[RequireComponent(typeof(SpriteRenderer))]
public class SpriteOffset : MonoBehaviour
{
    [Tooltip("World-space offset of the displayed sprite from the pivot point")]
    public Vector2 offset = new Vector2(-0.3f, -0.15f);

    private SpriteRenderer _source;   // driven by Animator (hidden)
    private SpriteRenderer _display;  // visible child at offset

    void Awake()
    {
        _source = GetComponent<SpriteRenderer>();

        GameObject child = new GameObject("_SpriteDisplay");
        child.transform.SetParent(transform, false);
        child.transform.localPosition = new Vector3(offset.x, offset.y, 0f);

        _display = child.AddComponent<SpriteRenderer>();
        _display.sortingLayerID = _source.sortingLayerID;
        _display.sortingOrder   = _source.sortingOrder;
        _display.material       = _source.material;

        _source.enabled = false;   // hide the root renderer
    }

    void LateUpdate()
    {
        if (_source == null || _display == null) return;
        _display.sprite   = _source.sprite;
        _display.flipX    = _source.flipX;
        _display.flipY    = _source.flipY;
        _display.color    = _source.color;
        _display.material = _source.material;
    }
}
