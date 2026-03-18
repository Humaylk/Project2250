using UnityEngine;

// <summary>
// This is the central manager for the entire game, handling game state, level transitions, and persistent data.
// It follows the Singleton design pattern to ensure only one instance exists.
// </summary>
//NOte: I forgot to save it and shutdown my pc, didnt do a pull request either cuz we tryna figure out the order of pushing
public class GameManager : MonoBehaviour
{
    // <summary>
    // Static reference to the active GameManager instance.
    // Use GameManager.Instance from other scripts to access its methods and data.
    // </summary>
    public static GameManager Instance { get; private set; }

    [Header("Core References")]
    // References to other core systems, essential for the game to function.
    //public Player player;               // Reference to the main player object.
    public LevelBase currentLevel;       // Reference to the currently loaded level's base class.
   // public ProgressionSystem progressionSystem; // Reference to the system tracking player progress and upgrades.
    //public UIManager uiManager;         // Reference to the user interface manager.

    // Tracks the current level index, starting from 0.
    public int currentLevelIndex = 0;

    void Awake()
    {
        // --- Singleton Pattern Implementation ---
        // Ensure only one instance of the GameManager exists.

        // If no instance has been assigned yet...
        if (Instance == null)
        {
            // ...this script becomes the active instance...
            Instance = this;
            // ...and this GameObject persists across different scene loads.
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // ...otherwise, an instance already exists, so destroy this duplicate.
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Automatically start the game process when the GameManager initializes.
        StartGame();
    }

    // <summary>
    // Sets up the initial game state, resets progress, and loads the first level.
    // </summary>
    public void StartGame()
    {
        Debug.Log("=== Game Started ===");
        // Reset the count of completed levels in the progression system.
      //  if (progressionSystem != null)
        //    progressionSystem.completedLevels = 0;
        // Proceed to load the initial level.
        LoadLevel();
    }

    // <summary>
    // Prepares and initializes the currently set currentLevel.
    // </summary>
    public void LoadLevel()
    {
        // If there is a level assigned, call its initialization method.
        if (currentLevel != null)
            currentLevel.InitializeLevel();
        // Update the UI to display the current objective.
      //  uiManager?.DisplayObjective("Find your way through the corrupted island.");
        Debug.Log("Level " + currentLevelIndex + " loaded.");
    }

    // <summary>
    // Increases the level index, tracks completion, and grants rewards for the completed level.
    // </summary>
    public void AdvanceLevel()
    {
        // Increment the level index.
        currentLevelIndex++;
        Debug.Log("Advancing to level " + currentLevelIndex);
        // Inform the progression system that a level was completed.
       // progressionSystem?.TrackLevelCompletion();
        // Trigger the rewards for the *newly entered* level, which acts as completing the previous.
        // Wait, the documentation implies ApplyCompletionRewards grants rewards based on the currentLevelIndex *after* it's been incremented.
        // This effectively means completing level N grants the rewards listed for case N+1.
        ApplyCompletionRewards();
    }

    // <summary>
    // Resets the player state and current level when the player dies.
    // </summary>
    public void ResetOnDeath()
    {
        Debug.Log("Player died - resetting level.");
        // If the player object exists...
     //   if (player != null)
        {
            // ...restore player health to full...
       //     player.hp = 100;
            // ...and reset player position to the origin (Vector3.zero).
         //   player.transform.position = Vector3.zero;
        }
        // Re-initialize the current level to its starting state.
        currentLevel?.InitializeLevel();
        // Update the UI to reflect death and reset health.
        //uiManager?.DisplayObjective("You died. Try again!");
        //uiManager?.UpdateHPDisplay(100);
    }

    // <summary>
    // Grants functional or cosmetic rewards to the player based on the currentLevelIndex.
    // This is typically called after the level index has been advanced.
    // </summary>
    public void ApplyCompletionRewards()
    {
        // Cannot grant rewards if the player object doesn't exist.
       // if (player == null) return;
        // Check the *new* current level index to determine rewards.
        switch (currentLevelIndex)
        {
            case 1:
                // Rewards for starting level 1 (implicitly after completing an intro or similar).
         //       uiManager?.ShowHint("Reward: Metal Sword obtained!");
           //     progressionSystem?.GrantReward("Metal Sword");
                break;
            case 2:
                // Rewards for starting level 2 (implicitly after completing level 1).
             //   uiManager?.ShowHint("Reward: +5 Max Health!");
               // progressionSystem?.GrantReward("+5 Health");
                break;
            case 3:
                // Rewards for starting level 3 (implicitly after completing level 2).
                //uiManager?.ShowHint("Reward: New Armor equipped!");
                //progressionSystem?.GrantReward("New Armor");
                break;
            case 4:
                // Rewards for starting level 4 (implicitly after completing level 3).
                //progressionSystem?.UnlockAbility("Punch");
                //uiManager?.ShowHint("Reward: Punch ability unlocked!");
                break;
            case 5:
                // Rewards for starting level 5, the final level (implicitly after completing level 4).
                //uiManager?.ShowHint("You have restored balance to the realm!");
              //  progressionSystem?.GrantReward("Elemental Armor");
                break;
        }
    }
}