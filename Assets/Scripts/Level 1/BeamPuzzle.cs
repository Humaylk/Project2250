using UnityEngine;
public class BeamPuzzle : MonoBehaviour
{

    [Header("Assign all 3 pillars here")]
    public RotatingPillar[] pillars = new RotatingPillar[3];

    private bool _isSolved = false;
    public bool IsSolved() => _isSolved;
    
    private void Start()
    {
        for (int i = 0; i < pillars.Length; i++)
        {
            if (pillars[i] != null)
                pillars[i].OnPillarRotated += OnAnyPillarRotated;
            else
                Debug.LogError($"[BeamPuzzle] Pillar slot [{i}] is not assigned in the Inspector!");
        }

        UIManager.Instance?.SetObjective("Rotate the three stone pillars to align them.");
    }

    private void OnDestroy()
    {
        for (int i = 0; i < pillars.Length; i++)
        {
            if (pillars[i] != null)
                pillars[i].OnPillarRotated -= OnAnyPillarRotated;
        }
    }

    private void OnAnyPillarRotated(RotatingPillar rotatedPillar)
    {
        if (_isSolved) return;

        Debug.Log($"[BeamPuzzle] {rotatedPillar.pillarID} rotated — checking all pillars");
        CheckAllPillars();
    }
    
    private void CheckAllPillars()
    {
        int alignedCount = 0;

        for (int i = 0; i < pillars.Length; i++)
        {
            if (pillars[i] != null && pillars[i].IsAligned())
                alignedCount++;

            Debug.Log($"  Pillar[{i}] ({pillars[i]?.pillarID}): aligned = {pillars[i]?.IsAligned()}");
        }
        UIManager.Instance?.ShowHint($"{alignedCount} / {pillars.Length} pillars aligned", 2f);

        if (alignedCount == pillars.Length)
        {
            _isSolved = true;
            Debug.Log("[BeamPuzzle] All pillars aligned! Puzzle solved.");
            UIManager.Instance?.SetObjective("The gateway is opening!");
            UIManager.Instance?.ShowMessage("All pillars aligned! The gateway opens!", 4f);

            OnPuzzleSolved?.Invoke();
        }
    }
    public System.Action OnPuzzleSolved;
    public void ResetPuzzle()
    {
        _isSolved = false;

        for (int i = 0; i < pillars.Length; i++)
            pillars[i]?.ResetPillar();

        UIManager.Instance?.SetObjective("Rotate the three stone pillars to align them.");
        Debug.Log("[BeamPuzzle] Puzzle reset.");
    }
}