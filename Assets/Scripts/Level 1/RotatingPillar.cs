using UnityEngine;

public class RotatingPillar : MonoBehaviour
{
    [Header("Identity")]
    public string pillarID = "Pillar_A";
    [Header("Rotation")]
    [Tooltip("Degrees added per E press.")]
    public float rotationStep    = 90f;
    [Tooltip("The angle this pillar must reach to count as solved.")]
    public float targetAngle     = 90f;
    [Tooltip("How many degrees off the target is still accepted.")]
    public float angleTolerance  = 5f;
    [Tooltip("Visual spin speed in degrees/second.")]
    public float rotationSpeed   = 120f;
    
    public System.Action<RotatingPillar> OnPillarRotated;
    private float _currentAngle      = 0f;
    private float _targetVisualAngle = 0f;
    private bool  _isSpinning        = false;
    private bool  _playerNearby      = false;
    
    private void Update()
    {
        if (_playerNearby && !_isSpinning && Input.GetKeyDown(KeyCode.E))
            Rotate();
        SmoothSpin();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _playerNearby = true;
            UIManager.Instance?.ShowHint($"[E] Rotate {pillarID}", 3f);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            _playerNearby = false;
    }
    
    public void Rotate()
    {
        _currentAngle      = (_currentAngle + rotationStep) % 360f;
        _targetVisualAngle = _currentAngle;
        _isSpinning        = true;
        Debug.Log($"[{pillarID}] Rotating to {_currentAngle}°...");
    }

    public bool IsAligned()
    {
        float diff = Mathf.Abs(Mathf.DeltaAngle(_currentAngle, targetAngle));
        return diff <= angleTolerance;
    }

    public void ResetPillar()
    {
        _currentAngle      = 0f;
        _targetVisualAngle = 0f;
        _isSpinning        = false;
        transform.rotation = Quaternion.identity;
        Debug.Log($"[{pillarID}] Reset to 0°.");
    }
    

    private void SmoothSpin()
    {
        if (!_isSpinning) return;
        float current = transform.eulerAngles.z;
        float next    = Mathf.MoveTowardsAngle(current, _targetVisualAngle, rotationSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0f, 0f, next);
        
        if (Mathf.Abs(Mathf.DeltaAngle(next, _targetVisualAngle)) < 0.5f)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, _targetVisualAngle);
            _isSpinning = false;
            OnPillarRotated?.Invoke(this); 

            if (IsAligned())
                UIManager.Instance?.ShowHint($"{pillarID} locked in!", 2f);
        }
    }



    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 1.5f);
    }
}