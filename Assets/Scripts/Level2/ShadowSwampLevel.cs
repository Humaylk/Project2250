using UnityEngine;

// Level 2 - Shadow Swamp
// The player must collect 3 ancient orbs scattered around the swamp
// while fending off wolf enemies. Collecting all orbs opens the gate.
public class ShadowSwampLevel : LevelBase
{
    [Header("Level 2 Specific References")]
    public OrbPuzzle puzzle;
    public Gate gate;
    public EnemyHealth[] wolves;
    public UIManager uiManager;

    [Header("Player Spawn")]
    public Vector3 spawnPosition = new Vector3(-6f, 0f, 0f);

    private PlayerController player;
    private PlayerHealth playerHealth;

    void Awake()
    {
        player = FindFirstObjectByType<PlayerController>();
        playerHealth = FindFirstObjectByType<PlayerHealth>();
    }

    public override void InitializeLevel()
    {
        isActive = true;
        isComplete = false;
        Debug.Log("=== Shadow Swamp - Level 2 Initialized ===");

        if (player != null)
            player.transform.position = spawnPosition;

        puzzle?.ResetPuzzle();
        gate?.ResetGate();
        SpawnEnemies();

        uiManager?.DisplayObjective("Collect the 3 ancient orbs hidden in the swamp!");
        uiManager?.ShowHint("Walk over an orb to collect it. Watch out for wolves!");
    }

    public override void UpdateLevel()
    {
        if (puzzle != null && puzzle.IsSolved() && gate != null && !gate.isOpen)
        {
            gate.OpenGate();
            uiManager?.UpdateObjective("All orbs collected! Head to the gate!");
        }
    }

    public override bool CheckWinCondition()
    {
        return puzzle != null && puzzle.IsSolved() && gate != null && gate.isOpen;
    }

    public override bool CheckLoseCondition()
    {
        if (playerHealth == null) return false;
        return playerHealth.health <= 0;
    }

    public override void FinishLevel()
    {
        isComplete = true;
        Debug.Log("Level 2 - Shadow Swamp COMPLETE!");
        GameManager.Instance?.progressionSystem?.AddPuzzleXP(30);

        uiManager?.DisplayObjective("LEVEL COMPLETE! Shadow Swamp purified!");
        uiManager?.ShowHint("You gained +5 Max Health! Press H at the gate to advance.");

        GameManager.Instance?.AdvanceLevel();
    }

    public void SpawnEnemies()
    {
        if (wolves == null) return;
        foreach (EnemyHealth w in wolves)
        {
            if (w != null)
                w.gameObject.SetActive(true);
        }
    }

    public void ResetLevel() => InitializeLevel();
}
