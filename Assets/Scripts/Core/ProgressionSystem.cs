using UnityEngine;
using System.Collections.Generic;

// <summary>
// This system manages the persistent progression of the player throughout the game.
// It tracks completed levels, collected cores, total XP gained, XP breakdown, and unlocked abilities.
// This component is typically attached to the same GameObject as the GameManager or accessed through it.
// </summary>
public class ProgressionSystem : MonoBehaviour
{
    [Header("Progress Tracking")]
    // <summary>
    // Total number of levels the player has successfully finished.
    // </summary>
    public int completedLevels = 0;

    // <summary>
    // Total number of unique elemental cores collected by the player (corresponds to level completion).
    // </summary>
    public int coreCount = 0;

    [Header("XP System")]
    // <summary>
    // The player's grand total experience points earned across all sources.
    // </summary>
    public int totalXP = 0;

    // <summary>
    // XP earned specifically through combat activities (defeating enemies).
    // </summary>
    public int combatXP = 0;

    // <summary>
    // XP earned specifically through puzzle-solving activities.
    // </summary>
    public int puzzleXP = 0;

    // <summary>
    // XP earned specifically through collecting items.
    // </summary>
    public int collectionXP = 0;

    [Header("Unlocked Abilities")]
    // <summary>
    // A list of strings representing the unique abilities the player has unlocked.
    // Used to check if specific interactions or combat moves (like 'Punch') are available.
    // </summary>
    public List<string> unlockedAbilities = new List<string>();

    [Header("XP Gain Settings")]
    // Default XP amounts awarded for standard actions.
    public int xpPerKill = 10;
    public int xpPerPuzzle = 30;
    public int xpPerItem = 5;

    // <summary>
    // Awards Experience Points earned through combat.
    // </summary>
    // <param name="amount">Optional specific XP amount. If less than 0 (default), the standard xpPerKill amount is used.</param>
    public void AddCombatXP(int amount = -1)
    {
        // Determine the actual amount to gain (specific value or default).
        int gain = amount < 0 ? xpPerKill : amount;

        // Add to combat-specific total and grand total.
        combatXP += gain;
        totalXP += gain;

        // Log the gain for debugging purposes.
        Debug.Log("Combat XP +" + gain + " | Total: " + totalXP);
    }

    // <summary>
    // Awards Experience Points earned through completing puzzles.
    // </summary>
    // <param name="amount">Optional specific XP amount. If less than 0 (default), the standard xpPerPuzzle amount is used.</param>
    public void AddPuzzleXP(int amount = -1)
    {
        // Determine the actual amount to gain.
        int gain = amount < 0 ? xpPerPuzzle : amount;

        // Add to puzzle-specific total and grand total.
        puzzleXP += gain;
        totalXP += gain;

        Debug.Log("Puzzle XP +" + gain + " | Total: " + totalXP);
    }

    // <summary>
    // Awards Experience Points earned through collection activities.
    // </summary>
    // <param name="amount">Optional specific XP amount. If less than 0 (default), the standard xpPerItem amount is used.</param>
    public void AddCollectionXP(int amount = -1)
    {
        // Determine the actual amount to gain.
        int gain = amount < 0 ? xpPerItem : amount;

        // Add to collection-specific total and grand total.
        collectionXP += gain;
        totalXP += gain;

        Debug.Log("Collection XP +" + gain + " | Total: " + totalXP);
    }

    // <summary>
    // Increments the counters tracking level completion and core collection.
    // Typically called by the GameManager when the player exits a level successfully.
    // </summary>
    public void TrackLevelCompletion()
    {
        completedLevels++;
        coreCount++; // Assuming one core per level completion.
        Debug.Log("Levels completed: " + completedLevels + " | Cores: " + coreCount);
    }

    // <summary>
    // Adds a new ability string to the unlockedAbilities list if it hasn't been unlocked already.
    // </summary>
    // <param name="abilityName">The unique string name of the ability to unlock (e.g., "Punch").</param>
    public void UnlockAbility(string abilityName)
    {
        // Check if the list already contains this ability to prevent duplicates.
        if (!unlockedAbilities.Contains(abilityName))
        {
            unlockedAbilities.Add(abilityName);
            Debug.Log("Ability unlocked: " + abilityName);
        }
    }

    // <summary>
    // Checks if the player has unlocked a specific ability.
    // </summary>
    // <param name="abilityName">The unique string name of the ability to check.</param>
    // <returns>True if the player has the ability, false otherwise.</returns>
    public bool HasAbility(string abilityName)
    {
        // Return true if the list contains the specified string.
        return unlockedAbilities.Contains(abilityName);
    }

    // <summary>
    // Placeholder function for granting rewards (like cosmetic items or stats) upon level completion.
    // Currently only logs the reward. Functional implementation depends on other game systems.
    // </summary>
    // <param name="rewardDescription">A description of the reward being granted.</param>
    public void GrantReward(string rewardDescription)
    {
        Debug.Log("Reward granted: " + rewardDescription);
        // Add specific logic here later (e.g., equipping armor, increasing stats).
    }
}