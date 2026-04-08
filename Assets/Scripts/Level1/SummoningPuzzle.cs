using UnityEngine;

/// <summary>
/// Tracks all 3 summoning pillars. Fires OnAllSummoned when every statue is summoned.
/// </summary>
public class SummoningPuzzle : MonoBehaviour
{
    [Header("Assign all 3 summoning pillars")]
    public SummoningPillar[] pillars = new SummoningPillar[3];

    public bool IsSolved { get; private set; } = false;
    public System.Action OnAllSummoned;

    void Start()
    {
        foreach (var p in pillars)
            if (p != null)
                p.OnSummoned += OnPillarSummoned;
    }

    void OnDestroy()
    {
        foreach (var p in pillars)
            if (p != null)
                p.OnSummoned -= OnPillarSummoned;
    }

    void OnPillarSummoned(SummoningPillar pillar)
    {
        if (IsSolved) return;

        int count = 0;
        foreach (var p in pillars)
            if (p != null && p.IsSummoned) count++;

        int total = pillars.Length;

        if (count == total)
        {
            IsSolved = true;
            Debug.Log("[SummoningPuzzle] All statues summoned! Puzzle complete.");
            OnAllSummoned?.Invoke();
        }
    }

    public void ResetPuzzle()
    {
        IsSolved = false;
        // Statues are re-hidden via SummoningPillar.Start() on scene reload
    }
}
