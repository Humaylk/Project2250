using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Core References")]
    public PlayerController player;
    public LevelBase currentLevel;
    public ProgressionSystem progressionSystem;
    public UIManager uiManager;

    public int currentLevelIndex = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        Debug.Log("=== Game Started ===");
        if (progressionSystem != null)
            progressionSystem.completedLevels = 0;
        LoadLevel();
    }

    public void LoadLevel()
    {
        if (currentLevel != null)
            currentLevel.InitializeLevel();
        Debug.Log("Level " + currentLevelIndex + " loaded.");
    }

    public void AdvanceLevel()
    {
        currentLevelIndex++;
        Debug.Log("Advancing to level " + currentLevelIndex);
        progressionSystem?.TrackLevelCompletion();
        ApplyCompletionRewards();

        string next = GetNextScene(SceneManager.GetActiveScene().name);
        if (next != null)
            SceneManager.LoadScene(next);
        else
            Debug.Log("No next scene defined for: " + SceneManager.GetActiveScene().name);
    }

    private string GetNextScene(string current)
    {
        switch (current)
        {
            case "Level1_CrackedForest": return "Level2_EmberDepths";
            case "Level2_EmberDepths":  return "Level3_DrownedVault";
            case "Level3_DrownedVault": return "Level4_Sky";
            case "Level4_Sky":          return "Level5_AetherNexus";
            default:                    return null;
        }
    }

    public void ResetOnDeath()
    {
        Debug.Log("Player died - resetting level.");

        PlayerHealth ph = player?.GetComponent<PlayerHealth>();
        if (ph != null) ph.health = 100;

        currentLevel?.InitializeLevel();
        uiManager?.DisplayObjective("You died. Try again!");
        uiManager?.UpdateHPDisplay(100);
    }

    public void ApplyCompletionRewards()
    {
        if (player == null) return;
        switch (currentLevelIndex)
        {
            case 1:
                PlayerWeapon pw = player.GetComponent<PlayerWeapon>();
                if (pw != null) pw.UpgradeWeapon();
                uiManager?.ShowHint("Reward: Metal Sword obtained!");
                progressionSystem?.GrantReward("Metal Sword");
                break;
            case 2:
                uiManager?.ShowHint("Reward: +5 Max Health!");
                progressionSystem?.GrantReward("+5 Health");
                break;
            case 3:
                uiManager?.ShowHint("Reward: New Armor equipped!");
                progressionSystem?.GrantReward("New Armor");
                break;
            case 4:
                progressionSystem?.UnlockAbility("Punch");
                uiManager?.ShowHint("Reward: Punch ability unlocked!");
                break;
            case 5:
                uiManager?.ShowHint("You have restored balance to the realm!");
                progressionSystem?.GrantReward("Elemental Armor");
                break;
        }
    }
}