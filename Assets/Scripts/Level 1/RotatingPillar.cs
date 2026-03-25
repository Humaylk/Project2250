using UnityEngine;

// Munadir: Added UIManager hint integration when player enters/exits pillar trigger zone
// Munadir: Reduced trigger radius to 0.8 to fix controls breaking after E press
public class RotatingPillar : MonoBehaviour
{
    [Header("Identity")]
    public string pillarID = "Pillar_A";

    [Header("Rotation")]
    [Tooltip("Degrees added per E press.")]
    public float rotationStep = 90f;
    [Tooltip("The angle this pillar must reach to count as solved.")]
    public float targetAngle = 90f;
    [Tooltip("How many degrees off the target is still accepted.")]
    public float angleTolerance = 5f;
    [Tooltip("Visual spin speed in degrees/second.")]
    public float rotationSpeed = 120f;

    public System.Action<RotatingPillar> OnPillarRotated;

    private float _currentAngle = 0f;
    private float _targetVisualAngle = 0f;
    private bool _isSpinning = false;
    private bool _playerNearby = false;

    private void Update()
    {
        // Munadir: E key only fires when player is nearby and pillar isnt already spinning
        if (_playerNearby && !_isSpinning && Input.GetKeyDown(KeyCode.E))
            Rotate();

        SmoothSpin();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _playerNearby = true;
            // Munadir: Show hint via UIManager when player enters trigger zone
            GameManager.Instance?.uiManager?.ShowHint("[E] Rotate " + pillarID);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Munadir: Clear nearby flag so E key stops working outside trigger zone
            _playerNearby = false;
            GameManager.Instance?.uiManager?.ClearMessages();
        }
    }

    public void Rotate()
    {
        _currentAngle = (_currentAngle + rotationStep) % 360f;
        _targetVisualAngle = _currentAngle;
        _isSpinning = true;
        Debug.Log("[" + pillarID + "] Rotating to " + _currentAngle + "degrees");
    }

    public bool IsAligned()
    {
        float diff = Mathf.Abs(Mathf.DeltaAngle(_currentAngle, targetAngle));
        return diff <= angleTolerance;
    }

    public void ResetPillar()
    {
        _currentAngle = 0f;
      //Humayl: Removed for smoother gameplay  //_targetVisualAngle = 0f;
        _isSpinning = false;
        transform.rotation = Quaternion.identity;
        Debug.Log("[" + pillarID + "] Reset to 0 degrees.");
    }

    private void SmoothSpin()
    {
        if (!_isSpinning) return;

        float current = transform.eulerAngles.z;
        float next = Mathf.MoveTowardsAngle(current, _targetVisualAngle, rotationSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0f, 0f, next);

        if (Mathf.Abs(Mathf.DeltaAngle(next, _targetVisualAngle)) < 0.5f)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, _targetVisualAngle);
            _isSpinning = false;

            // Munadir: Fire event so BeamPuzzle knows this pillar was rotated
            OnPillarRotated?.Invoke(this);

            // Munadir: Show alignment status via UIManager
            if (IsAligned())
                GameManager.Instance?.uiManager?.ShowHint(pillarID + " is aligned!");
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Munadir: Visualize trigger radius in Scene view - set to 0.8 to fix input bug
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 0.8f);
    }
}