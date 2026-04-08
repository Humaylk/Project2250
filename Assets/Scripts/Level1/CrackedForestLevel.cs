using UnityEngine;

public class CrackedForestLevel : LevelBase
{
    [Header("Level 1 References")]
    public SummoningPuzzle summoningPuzzle;
    public EnemyHealth[] golems;
    public Gate gate;
    public UIManager uiManager;

    private PlayerController player;
    private PlayerHealth playerHealth;

    void Awake()
    {
        player       = FindFirstObjectByType<PlayerController>();
        playerHealth = FindFirstObjectByType<PlayerHealth>();
    }

    void Start()
    {
        // Subscribe directly to the puzzle event so the gate shows
        // the moment all 3 statues are summoned, regardless of isActive.
        if (summoningPuzzle != null)
            summoningPuzzle.OnAllSummoned += OnAllStatuesSummoned;

        gate?.HideGate();    // invisible at level start
        isActive = true;
    }

    private void OnAllStatuesSummoned()
    {
        gate?.ShowAndOpenGate();
    }

    void OnDestroy()
    {
        if (summoningPuzzle != null)
            summoningPuzzle.OnAllSummoned -= OnAllStatuesSummoned;
    }

    public override void InitializeLevel()
    {
        isActive   = true;
        isComplete = false;
        Debug.Log("=== Cracked Forest - Level 1 Initialized ===");

        gate?.HideGate();    // invisible until all 3 statues are summoned
        SpawnEnemies();

    }

    public override void UpdateLevel()
    {
        // Gate is now handled via OnAllStatuesSummoned event — nothing needed here.
    }

    public override bool CheckWinCondition()
    {
        return summoningPuzzle != null && summoningPuzzle.IsSolved && gate != null && gate.isOpen;
    }

    public override bool CheckLoseCondition()
    {
        if (playerHealth == null) return false;
        return playerHealth.health <= 0;
    }

    public override void FinishLevel()
    {
        isComplete = true;
        Debug.Log("Level 1 - Cracked Forest COMPLETE!");
        GameManager.Instance?.progressionSystem?.AddPuzzleXP(30);

    }

    public void SpawnEnemies()
    {
        if (golems == null) return;
        foreach (EnemyHealth g in golems)
            if (g != null)
                g.gameObject.SetActive(true);
    }

    public void ResetLevel() => InitializeLevel();
}
