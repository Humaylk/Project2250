using UnityEngine;

// Tracks how many orbs the player has collected.
// Fires OnPuzzleSolved when all orbs are collected.
public class OrbPuzzle : MonoBehaviour
{
    [Header("Assign all orbs here")]
    public CollectibleOrb[] orbs;

    private int _collectedCount = 0;
    private bool _isSolved = false;

    public bool IsSolved() => _isSolved;

    public System.Action OnPuzzleSolved;

    private void Start()
    {
        for (int i = 0; i < orbs.Length; i++)
        {
            if (orbs[i] != null)
                orbs[i].OnOrbCollected += OnAnyOrbCollected;
            else
                Debug.LogError($"[OrbPuzzle] Orb slot [{i}] is not assigned in the Inspector!");
        }
    }

    private void OnDestroy()
    {
        for (int i = 0; i < orbs.Length; i++)
        {
            if (orbs[i] != null)
                orbs[i].OnOrbCollected -= OnAnyOrbCollected;
        }
    }

    private void OnAnyOrbCollected(CollectibleOrb orb)
    {
        if (_isSolved) return;

        _collectedCount++;
        int remaining = orbs.Length - _collectedCount;
        Debug.Log($"[OrbPuzzle] {orb.orbID} collected — {_collectedCount}/{orbs.Length}");

        if (remaining > 0)
            GameManager.Instance?.uiManager?.ShowHint($"{remaining} orb(s) remaining!");

        if (_collectedCount >= orbs.Length)
        {
            _isSolved = true;
            Debug.Log("[OrbPuzzle] All orbs collected! Puzzle solved.");
            OnPuzzleSolved?.Invoke();
        }
    }

    public void ResetPuzzle()
    {
        _isSolved = false;
        _collectedCount = 0;

        for (int i = 0; i < orbs.Length; i++)
            orbs[i]?.ResetOrb();

        Debug.Log("[OrbPuzzle] Puzzle reset.");
    }
}
