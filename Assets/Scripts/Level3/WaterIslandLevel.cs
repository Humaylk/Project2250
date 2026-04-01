using UnityEngine;

// Coordinates all gameplay logic for the Water Island level (Drowned Vault).
// The entire level takes place underwater — the oxygen timer starts immediately
// on level load and counts down for the full duration of play.
// Manages: oxygen timer, fish assassin spawning, rock-clearing objective
// tracking, and triggering level completion when the exit opening is created.
// This class is level-specific and does not extend beyond Level 3.
public class WaterIslandLevel : LevelBase
{
    [Header("Level 3 Specific References")]
    public RockBarrier rockBarrier;
    public Gate exitDoor;
    public EnemyHealth[] fishAssassins;
    public Timer oxygenTimer;
    public WaterIslandStatus islandStatus;
    public UIManager uiManager;

    [Header("Oxygen Timer Settings")]
    public float timerDuration = 45f;

    [Header("Player Spawn")]
    public Vector3 spawnPosition = new Vector3(-8f, 1f, 0f);

    private PlayerController player;
    private PlayerHealth playerHealth;
    private float lastTimerDisplayTime = -1f;

    void Awake()
    {
        player = FindFirstObjectByType<PlayerController>();
        playerHealth = FindFirstObjectByType<PlayerHealth>();
    }

    public override void InitializeLevel()
    {
        isActive = true;
        isComplete = false;
        Debug.Log("=== Drowned Vault - Level 3 Initialized ===");

        if (player != null)
            player.transform.position = spawnPosition;

        rockBarrier?.ResetBarrier();
        exitDoor?.ResetGate();
        SpawnAssassins();

        // Entire level is underwater — start the oxygen timer immediately
        if (oxygenTimer != null)
        {
            oxygenTimer.StopTimer();
            oxygenTimer.ResetTimer();
            oxygenTimer.StartTimer(timerDuration);
        }

        uiManager?.DisplayObjective("Clear the rocks to open the exit before you run out of oxygen!");
        uiManager?.ShowHint("Watch out for fish assassins! You have 45 seconds of oxygen.");
    }

    public override void UpdateLevel()
    {
        if (player == null) return;

        // Display the remaining oxygen countdown roughly once per second
        if (oxygenTimer != null && oxygenTimer.isRunning)
        {
            if (Time.time >= lastTimerDisplayTime + 1f)
            {
                lastTimerDisplayTime = Time.time;
                int secondsLeft = Mathf.CeilToInt(oxygenTimer.timeRemaining);
                uiManager?.ShowHint("Oxygen: " + secondsLeft + "s remaining!");
            }
        }

        // Once rocks are cleared, open the exit doorway
        if (rockBarrier != null && rockBarrier.IsCleared() && exitDoor != null && !exitDoor.isOpen)
        {
            exitDoor.OpenGate();
            uiManager?.UpdateObjective("Exit opened! Escape through the doorway!");
        }
    }

    // Win condition: rocks cleared and exit door is open
    public override bool CheckWinCondition()
    {
        return rockBarrier != null && rockBarrier.IsCleared() &&
               exitDoor != null && exitDoor.isOpen;
    }

    // Lose condition: player HP hits zero OR oxygen timer runs out
    public override bool CheckLoseCondition()
    {
        if (playerHealth == null) return false;

        if (playerHealth.health <= 0) return true;

        if (oxygenTimer != null && oxygenTimer.IsTimeUp())
        {
            uiManager?.ShowHint("You drowned! Out of oxygen.");
            return true;
        }

        return false;
    }

    public override void FinishLevel()
    {
        isComplete = true;
        Debug.Log("Level 3 - Drowned Vault COMPLETE!");

        // Mark Water Island as restored in world state
        islandStatus?.MarkRestored();

        // Award XP for combat and clearing the rock puzzle
        GameManager.Instance?.progressionSystem?.AddCombatXP(20);
        GameManager.Instance?.progressionSystem?.AddPuzzleXP(30);

        uiManager?.DisplayObjective("LEVEL COMPLETE! Water Island restored!");
        uiManager?.ShowHint("You earned New Armor! Press H at the exit to advance.");

        GameManager.Instance?.AdvanceLevel();
    }

    // Activates all fish assassin GameObjects at level start
    public void SpawnAssassins()
    {
        if (fishAssassins == null) return;
        foreach (EnemyHealth fa in fishAssassins)
        {
            if (fa != null)
                fa.gameObject.SetActive(true);
        }
    }

    public void ResetLevel() => InitializeLevel();
}
