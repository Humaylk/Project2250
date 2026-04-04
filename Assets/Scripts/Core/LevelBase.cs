using UnityEngine;

// <summary>
// The abstract base class that defines the core lifecycle and required functionality for every level (island) in the game.
// All specific level classes (e.g., CrackedForestLevel) must extend this class and implement its abstract methods.
// </summary>
public abstract class LevelBase : MonoBehaviour
{
    [Header("Level State")]
    // <summary>
    // Indicates if this level is currently the active level being played.
    // When false, its lifecycle methods (UpdateLevel, etc.) will not run.
    // </summary>
    public bool isActive = false;

    // <summary>
    // Indicates if the level's win condition has been met and the level is finished.
    // </summary>
    public bool isComplete = false;

    // --- Abstract Methods (Must be implemented by child classes) ---

    // <summary>
    // Sets up the level to its initial state (spawning player, enemies, resetting puzzles).
    // Called when the level is first loaded or reset.
    // </summary>
    public abstract void InitializeLevel();

    // <summary>
    // Contains the unique, level-specific logic that runs every frame.
    // This is called by the protected virtual Update() method.
    // </summary>
    public abstract void UpdateLevel();

    // <summary>
    // Checks if the player has met the necessary criteria to win the level (e.g., solved puzzle, collected core).
    // Returns true if the level is won, false otherwise.
    // </summary>
    public abstract bool CheckWinCondition();

    // <summary>
    // Checks if the player has failed the level (e.g., health reached 0, timer ran out).
    // Returns true if the level is lost, false otherwise.
    // </summary>
    public abstract bool CheckLoseCondition();

    // <summary>
    // Handles the final steps for completing a level (playing completion cutscene, granting XP, advancing to the next level).
    // Called once the CheckWinCondition() returns true.
    // </summary>
    public abstract void FinishLevel();

    // --- Protected Lifecycle Method ---

    // <summary>
    // Standard Unity Update method that runs every frame. It manages the basic level state flow.
    // </summary>
    protected virtual void Update()
    {
        // If the level is not active or is already complete, do not process anything.
        if (!isActive || isComplete) return;

        // Call the unique, level-specific update logic.
        UpdateLevel();

        // Check if the win condition has been met.
        if (CheckWinCondition())
        {
            // If won, set the complete flag and call FinishLevel().
            isComplete = true;
            FinishLevel();
        }
        // Otherwise, check if the lose condition has been met.
        else if (CheckLoseCondition())
        {
            // If lost, trigger the game reset through the global GameManager.
            GameManager.Instance?.ResetOnDeath();
        }
    }

    // --- Protected Helper Method ---

    // <summary>
    // Resets the level state back to its starting conditions.
    // </summary>
    protected void RestartLevel()
    {
        // Reset the completion status...
        isComplete = false;
        // ...and call InitializeLevel() to reset objects and spawn points.
        InitializeLevel();
    }
}