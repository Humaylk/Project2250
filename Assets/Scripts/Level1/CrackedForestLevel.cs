using UnityEngine;

public class CrackedForestLevel : LevelBase
{
    [Header("Level 1 Specific References")]
    public RotatingPillar[] pillars = new RotatingPillar[3];
    public EnemyHealth[] golems;
    public BeamPuzzle puzzle;
    public Gate gate;
    public UIManager uiManager;

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
        Debug.Log("=== Cracked Forest - Level 1 Initialized ===");

        if (player != null)
            player.transform.position = new Vector3(-6f, 0f, 0f);

        puzzle?.ResetPuzzle();
        gate?.ResetGate();
        SpawnEnemies();

        uiManager?.DisplayObjective("Rotate the three stone pillars to align them!");
        uiManager?.ShowHint("Press E near a pillar to rotate it. Watch out for Golems!");
    }

    public override void UpdateLevel()
    {
        if (puzzle != null && puzzle.IsSolved() && gate != null && !gate.isOpen)
        {
            gate.OpenGate();
            uiManager?.UpdateObjective("Puzzle solved! Head to the gate!");
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
        Debug.Log("Level 1 - Cracked Forest COMPLETE!");
        GameManager.Instance?.progressionSystem?.AddPuzzleXP(30);
    
        // Munadir: Show level complete message to player
        uiManager?.DisplayObjective("LEVEL COMPLETE! Earth island restored!");
        uiManager?.ShowHint("You obtained the Metal Sword! Press H at the gate to advance.");
    
        GameManager.Instance?.AdvanceLevel();
    }

    public void SpawnEnemies()
    {
        if (golems == null) return;
        foreach (EnemyHealth g in golems)
        {
            if (g != null)
                g.gameObject.SetActive(true);
        }
    }

    public void ResetLevel() => InitializeLevel();
}