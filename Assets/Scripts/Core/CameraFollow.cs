using UnityEngine;

/// <summary>
/// Smooth dynamic camera that follows the player with:
///   - Configurable smoothing speed
///   - Dead zone (camera only moves when player leaves the zone)
///   - Optional map boundary clamping so camera never shows outside the map
/// Attach this script to the Main Camera.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [Tooltip("Drag the player (HeroKnight) here")]
    public Transform target;

    [Header("Smoothing")]
    [Tooltip("Higher = snappier follow. Lower = floatier follow")]
    public float smoothSpeed = 5f;

    [Header("Dead Zone")]
    [Tooltip("Camera only moves when player exits this box (in world units)")]
    public Vector2 deadZoneSize = new Vector2(1.5f, 1f);

    [Header("Look Ahead")]
    [Tooltip("Camera shifts ahead in the direction the player is moving")]
    public float lookAheadDistance = 1.5f;
    public float lookAheadSpeed    = 3f;

    [Header("Map Boundaries (optional)")]
    [Tooltip("Enable to clamp camera so it never shows past the map edges")]
    public bool  useBoundaries = false;
    public float minX = -20f;
    public float maxX =  20f;
    public float minY = -15f;
    public float maxY =  15f;

    // ── private state ──────────────────────────────────────────────────────
    private Vector3    _currentVelocity;
    private Vector2    _lookAheadOffset;
    private Camera     _cam;
    private float      _halfW, _halfH;

    void Start()
    {
        _cam = GetComponent<Camera>();

        if (target == null)
        {
            // Auto-find the player if not assigned
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null) target = player.transform;
        }

        if (_cam != null)
        {
            _halfH = _cam.orthographicSize;
            _halfW = _halfH * _cam.aspect;
        }

        // Snap to player on start — no slow pan from (0,0)
        if (target != null)
            transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 camPos    = transform.position;
        Vector3 targetPos = target.position;

        // ── Dead zone check ────────────────────────────────────────────────
        float dx = targetPos.x - camPos.x;
        float dy = targetPos.y - camPos.y;

        float halfDZx = deadZoneSize.x * 0.5f;
        float halfDZy = deadZoneSize.y * 0.5f;

        Vector3 desiredPos = camPos;

        if (Mathf.Abs(dx) > halfDZx)
            desiredPos.x = targetPos.x - Mathf.Sign(dx) * halfDZx;

        if (Mathf.Abs(dy) > halfDZy)
            desiredPos.y = targetPos.y - Mathf.Sign(dy) * halfDZy;

        // ── Look-ahead offset ──────────────────────────────────────────────
        Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        Vector2 targetLookAhead = moveInput * lookAheadDistance;
        _lookAheadOffset = Vector2.Lerp(_lookAheadOffset, targetLookAhead, Time.deltaTime * lookAheadSpeed);

        desiredPos.x += _lookAheadOffset.x;
        desiredPos.y += _lookAheadOffset.y;

        // ── Smooth follow ──────────────────────────────────────────────────
        Vector3 smoothed = Vector3.SmoothDamp(camPos, desiredPos, ref _currentVelocity, 1f / smoothSpeed);
        smoothed.z = transform.position.z; // keep Z fixed

        // ── Boundary clamping ──────────────────────────────────────────────
        if (useBoundaries)
        {
            smoothed.x = Mathf.Clamp(smoothed.x, minX + _halfW, maxX - _halfW);
            smoothed.y = Mathf.Clamp(smoothed.y, minY + _halfH, maxY - _halfH);
        }

        transform.position = smoothed;
    }

    // ── Draw dead zone gizmo in Scene view ────────────────────────────────
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(deadZoneSize.x, deadZoneSize.y, 0f));

        if (useBoundaries)
        {
            Gizmos.color = Color.red;
            Vector3 center = new Vector3((minX + maxX) * 0.5f, (minY + maxY) * 0.5f, 0f);
            Vector3 size   = new Vector3(maxX - minX, maxY - minY, 0f);
            Gizmos.DrawWireCube(center, size);
        }
    }
}
