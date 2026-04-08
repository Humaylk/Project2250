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

    public override void InitializeLevel()
    {
        isActive   = true;
        isComplete = false;
        Debug.Log("=== Cracked Forest - Level 1 Initialized ===");

        gate?.ResetGate();
        SpawnEnemies();

    }

    public override void UpdateLevel()
    {
        if (summoningPuzzle != null && summoningPuzzle.IsSolved && gate != null && !gate.isOpen)
        {
            gate.OpenGate();
        }
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
