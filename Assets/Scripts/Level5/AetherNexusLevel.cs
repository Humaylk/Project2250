using UnityEngine;

// Munadir: Level 5 — Aether Nexus boss level extending LevelBase
// Munadir: Owns the boss, laser system, timer, and ability manager
// Munadir: Win = boss defeated. Lose = timer runs out OR player dies.
public class AetherNexusLevel : LevelBase
{
    [Header("Level 5 References")]
    public ElementalBoss boss;
    public LaserSystem laserSystem;
    public BattleTimer battleTimer;
    public AbilityManager abilityManager;
    public UIManager uiManager;

    [Header("Settings")]
    public float battleDuration = 180f; // 3 minutes

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
        Debug.Log("=== Aether Nexus - Level 5 Initialized ===");

        if (player != null)
            player.transform.position = new Vector3(-5f, 0f, 0f);

        // Start the battle timer
        if (battleTimer != null)
            battleTimer.StartTimer(battleDuration);

        // Start lasers
        if (laserSystem != null)
            laserSystem.StartLasers();

        // Initialize boss
        if (boss != null)
            boss.Initialize();

        uiManager?.DisplayObjective("Defeat the Elemental Dragon before time runs out!");
        uiManager?.QueueDialogue("The Dragon awakens... it will not let you leave alive.");
        uiManager?.QueueDialogue("Use G to attack. F for Fire ability. P for Heavy Attack.");
        uiManager?.QueueDialogue("Dodge the laser cannons in the corners!");
    }

    public override void UpdateLevel()
    {
        // Timer display handled by BattleTimer directly
    }

    public override bool CheckWinCondition()
    {
        return boss != null && boss.IsDefeated();
    }

    public override bool CheckLoseCondition()
    {
        // Lose if timer runs out OR player dies
        if (battleTimer != null && battleTimer.IsTimeUp())
            return true;
        if (playerHealth != null && playerHealth.health <= 0)
            return true;
        return false;
    }

    public override void FinishLevel()
    {
        isComplete = true;
        isActive = false;

        // Stop lasers
        if (laserSystem != null)
            laserSystem.StopLasers();

        if (CheckWinCondition())
        {
            Debug.Log("BOSS DEFEATED - GAME COMPLETE!");
            // Award final progression
            GameManager.Instance?.progressionSystem?.AddCombatXP(100);
            GameManager.Instance?.progressionSystem?.GrantReward("Elemental Armor");
            uiManager?.DisplayObjective("YOU WIN! Balance has been restored to the realm!");
            uiManager?.ShowHint("Elemental Armor unlocked! Alex has saved Quaziadore.");
        }
        else
        {
            Debug.Log("Level 5 failed - resetting.");
            GameManager.Instance?.ResetOnDeath();
        }
    }
}