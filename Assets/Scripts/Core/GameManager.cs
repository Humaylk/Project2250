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
    public int currentMaxHealth = 100;

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

        currentLevelIndex = 1;
        currentMaxHealth = 105;

        if (progressionSystem != null)
            progressionSystem.completedLevels = 1;

        LoadLevel();
    }

    public void LoadLevel()
    {
            if (currentLevel != null)
                currentLevel.InitializeLevel();

            switch (currentLevelIndex)
            {
                case 0:
                    uiManager?.DisplayObjective("Activate all pillars and escape the forest.");
                    break;
                case 1:
                    uiManager?.DisplayObjective("Talk to the Dragon");
                    break;
                case 2:
                    uiManager?.DisplayObjective("Clear the underwater path before time runs out.");
                    break;
                default:
                    uiManager?.DisplayObjective("Find your way through the corrupted island.");
                    break;
            }

            Debug.Log("Level " + currentLevelIndex + " loaded.");
    }

    public void AdvanceLevel()
    {
        currentLevelIndex++;
        Debug.Log("Advancing to level " + currentLevelIndex);
        progressionSystem?.TrackLevelCompletion();
        ApplyCompletionRewards();
    }

    public void ResetOnDeath()
    {
        
        Debug.Log("Player died - reloading level.");
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);

    }

    public void ApplyCompletionRewards()
    {
        if (player == null) return;

        switch (currentLevelIndex)
        {
            case 1:
                PlayerWeapon pw = player.GetComponent<PlayerWeapon>();
                if (pw != null) pw.UpgradeWeapon();

                currentMaxHealth += 5;
                uiManager?.ShowHint("Reward: Metal Sword obtained! Max HP is now " + currentMaxHealth);
                progressionSystem?.GrantReward("Metal Sword");
                break;

            case 2:
                currentMaxHealth += 5;
                uiManager?.ShowHint("Reward: +5 Max Health! Max HP is now " + currentMaxHealth);
                progressionSystem?.GrantReward("+5 Health");
                break;

            case 3:
                currentMaxHealth += 5;
                uiManager?.ShowHint("Reward: New Armor equipped! Max HP is now " + currentMaxHealth);
                progressionSystem?.GrantReward("New Armor");
                break;

            case 4:
                currentMaxHealth += 5;
                progressionSystem?.UnlockAbility("HeavyAttack");
                uiManager?.ShowHint("Reward: HeavyAttack ability unlocked! Max HP is now " + currentMaxHealth);
                break;

            case 5:
                currentMaxHealth += 5;
                uiManager?.ShowHint("You have restored balance to the realm! Max HP is now " + currentMaxHealth);
                progressionSystem?.GrantReward("Elemental Armor");
                break;
        }
    }
}