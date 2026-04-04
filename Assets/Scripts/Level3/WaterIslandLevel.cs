using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

// Coordinates all gameplay logic for the Water Island level (Drowned Vault).
// The entire level takes place underwater — the oxygen timer starts immediately
// on level load and counts down for the full duration of play.
// Manages: oxygen timer, fish assassin spawning, rock-clearing objective
// tracking, and triggering level completion when the exit opening is created.
// This class is level-specific and does not extend beyond Level 3.
public class WaterIslandLevel : LevelBase
{
    [Header("Level 3 Specific References")]
    public RockBarrier[] rockBarriers;
    public Gate exitDoor;
    public EnemyHealth[] fishAssassins;
    public CountdownTimer oxygenTimer;
    public WaterIslandStatus islandStatus;
    public UIManager uiManager;

    [Header("Oxygen Timer Settings")]
    public float timerDuration = 20f;

    [Header("HUD")]
    public TextMeshProUGUI oxygenText;

    [Header("Player Spawn")]
    public Vector3 spawnPosition = new Vector3(-8f, 1f, 0f);

    private PlayerController player;
    private PlayerHealth playerHealth;
    private bool isDrowning = false;

    void Awake()
    {
        player = FindFirstObjectByType<PlayerController>();
        playerHealth = FindFirstObjectByType<PlayerHealth>();
        PlayerHealth.OnDeath += HandlePlayerDeath;
    }

    void OnDestroy()
    {
        PlayerHealth.OnDeath -= HandlePlayerDeath;
    }

    void Start()
    {
        // Re-register with GameManager in case this scene was reloaded
        if (GameManager.Instance != null)
        {
            GameManager.Instance.currentLevel = this;
            if (player != null) GameManager.Instance.player = player;
        }
        InitializeLevel();
    }

    private void HandlePlayerDeath()
    {
        if (!isActive || isComplete) return;
        isActive = false;
        StopAllCoroutines();
        DeathScreen ds = FindFirstObjectByType<DeathScreen>();
        if (ds != null) ds.Show();
        else SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    protected override void Update()
    {
        // Always keep the oxygen HUD ticking, even after win
        UpdateOxygenDisplay();

        if (!isActive || isComplete) return;

        UpdateLevel();

        if (CheckWinCondition())
        {
            isComplete = true;
            FinishLevel();
        }
        // Death is handled immediately via PlayerHealth.OnDeath event → HandlePlayerDeath()
    }

    private void UpdateOxygenDisplay()
    {
        if (oxygenTimer == null || oxygenText == null) return;
        int secondsLeft = Mathf.CeilToInt(oxygenTimer.timeRemaining);
        oxygenText.text = "Oxygen Left: " + secondsLeft + "s";
        if (secondsLeft <= 10)
        {
            float alpha = Mathf.Abs(Mathf.Sin(Time.time * 4f));
            oxygenText.color = new Color(1f, 0.2f, 0.2f, alpha);
        }
        else
        {
            oxygenText.color = new Color(0f, 0.9f, 1f, 1f);
        }
    }

    public override void InitializeLevel()
    {
        isActive = true;
        isComplete = false;
        Debug.Log("Level 3 - Drowned Vault");

        if (player != null)
            player.transform.position = spawnPosition;

        foreach (var rb in rockBarriers) rb?.ResetBarrier();
        exitDoor?.ResetGate();
        SpawnAssassins();

        // Entire level is underwater — start the oxygen timer immediately
        if (oxygenTimer != null)
        {
            oxygenTimer.StopTimer();
            oxygenTimer.ResetTimer();
            oxygenTimer.StartTimer(timerDuration);
        }

        uiManager?.DisplayObjective("Diffuse the mines to open the exit before you run out of oxygen!");
        uiManager?.ShowHint("Watch out for the killer fish! You have 20 seconds of oxygen. Open the chest and pick up the item for more time.");
    }

    public override void UpdateLevel()
    {
        if (player == null) return;

        // Once ALL mines are defused, open the exit doorway
        if (AllMinesCleared() && exitDoor != null && !exitDoor.isOpen)
        {
            exitDoor.OpenGate();
            uiManager?.UpdateObjective("Exit opened! Escape through the doorway!");
        }

        // Start drowning damage when O2 hits 0
        if (oxygenTimer != null && oxygenTimer.IsTimeUp() && !isDrowning)
        {
            isDrowning = true;
            StartCoroutine(DrowningDamage());
        }

        // Stop drowning only if timer was refilled (helmet picked up)
        if (isDrowning && oxygenTimer != null && oxygenTimer.timeRemaining > 0f)
        {
            isDrowning = false;
        }
    }

    IEnumerator DrowningDamage()
    {
        uiManager?.ShowHint("Out of oxygen! Find the helmet or reach the exit!");
        while (isDrowning)
        {
            yield return new WaitForSeconds(1f);
            if (!isDrowning) yield break;
            playerHealth?.TakeDamage(5);
            Debug.Log("Drowning! -5 HP");
        }
    }

    bool AllMinesCleared()
    {
        if (rockBarriers == null || rockBarriers.Length == 0) return false;
        foreach (var rb in rockBarriers)
            if (rb != null && !rb.IsCleared()) return false;
        return true;
    }

    // Win condition: all mines defused and exit door is open
    public override bool CheckWinCondition()
    {
        return AllMinesCleared() && exitDoor != null && exitDoor.isOpen;
    }

    // Lose condition: player HP hits zero OR oxygen timer runs out
    public override bool CheckLoseCondition()
    {
        if (playerHealth == null) return false;

        if (playerHealth.health <= 0) return true;

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
