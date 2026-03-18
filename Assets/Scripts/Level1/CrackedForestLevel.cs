using UnityEngine;

// <summary>
// This class implements the specific logic, setup, and conditions for Level 1,
// which corresponds to the 'Cracked Forest (Earth)' island from the Quaziadore world map blueprint.
// It extends the abstract LevelBase class, adhering to the standard level lifecycle defined in your diagrams.
// The gameplay loop for this level involves rotating pillars, solving the Beam Puzzle, and exiting through the Gate.
// </summary>
public class CrackedForestLevel : LevelBase
{
    [Header("Level 1 Specific References")]
    // <summary>
    // An array containing references to the three 'RotatingPillar' objects the player must interact with
    // using the InteractionSystem (typically via the 'E' key) to align the puzzle.
    // </summary>
    public RotatingPillar[] pillars = new RotatingPillar[3];

    // <summary>
    // An array of references to the 'Golem' Enemy objects present in this Earth-themed level.
    // They are activated when the level initializes.
    // </summary>
    public Enemy[] golems;

    // <summary>
    // Reference to the 'BeamPuzzle' logical component that continuously evaluates the state of the pillars.
    // </summary>
    public BeamPuzzle puzzle;

    // <summary>
    // Reference to the 'Gate' object, which serves as the level's exit and the directional connection back to the Aether Nexus.
    // It remains sealed (blocking collision) until the puzzle is solved.
    // </summary>
    public Gate gate;

    // <summary>
    // Reference to the 'UIManager' used to provide objectives, hints, and feedback to the player.
    // </summary>
    public UIManager uiManager;

    // --- Private Field for Cached Reference ---

    // A cached reference to the Player GameObject present in the scene, used for positioning and health checks.
    private Player player;

    void Awake()
    {
        // Cache the reference to the Player script/object in the scene for performance.
        player = FindObjectOfType<Player>();
    }

    // --- Abstract Lifecycle Method Overrides ---

    // <summary>
    // Overrides the InitializeLevel method from LevelBase. Sets up the level to its 'Phase 1' initial state:
    // activates the level, resets the completion flag, positions the player at the start point,
    // resets all puzzles and gates, spawns enemies, and updates the UIManager with initial objectives and hints.
    // </summary>
    public override void InitializeLevel()
    {
        // Standard activation sequence from LevelBase.
        isActive = true;
        isComplete = false;
        
        Debug.Log("=== Cracked Forest - Level 1 Initialized ===");

        // 1. Position the player at the level's starting location (the spawn point indicator on your diagram).
        if (player != null)
        {
            player.transform.position = Vector3.zero;
        }

        // 2. Clear previous state by resetting the puzzle (aligning beams) and sealing the gate.
        puzzle?.ResetPuzzle();
        gate?.ResetGate();

        // 3. Spawns the Golem enemies into the scene.
        SpawnEnemies();

        // 4. Update the UIManager to provide clear direction to the player for the core gameplay loop.
        uiManager?.DisplayObjective("Rotate the three stone pillars to align them!");
        uiManager?.ShowHint("Press E near a pillar to rotate it. Watch out for Golems!");
    }

    // <summary>
    // Overrides the UpdateLevel method from LevelBase. Contains the unique, level-specific logic that runs every frame.
    // This method is responsible for checking and processing the pillar puzzle progression.
    // </summary>
    public override void UpdateLevel()
    {
        UpdatePuzzle();
    }

    // <summary>
    // Overrides the CheckWinCondition method from LevelBase. Defines the precise criteria required to 'win' this level.
    // Completion means the 'Beam Puzzle' is solved AND the 'Gate' exit state is now open (player can pass).
    // This logic satisfies thedirectional flow shown in the diagram architecture.
    // </summary>
    // <returns>True if both the puzzle is solved and the exit gate is open, false otherwise.</returns>
    public override bool CheckWinCondition()
    {
        // Require both logical systems to be in their final 'success' state.
        // We ensure references are not null before checking their boolean properties.
        return puzzle != null && puzzle.isSolved && gate != null && gate.isOpen;
    }

    // <summary>
    // Overrides the CheckLoseCondition method from LevelBase. Defines how the player 'loses' this level.
    // For Phase 1 implementation, loss occurs solely when the player's health drops to 0 or below.
    // </summary>
    // <returns>True if the player has lost the level, false otherwise.</returns>
    public override bool CheckLoseCondition()
    {
        // Check standard combat fail state.
        return player != null && player.hp <= 0;
    }

    // <summary>
    // Overrides the FinishLevel method from LevelBase. Handles the necessary steps to complete the level successfully:
    // marks the level as complete, logs the accomplishment, grants the appropriate rewards through the ProgressionSystem,
    // displays the success state in the UI, and tells the GameManager to advance the player (typically back to the Aether Nexus).
    // </summary>
    public override void FinishLevel()
    {
        // Standard completion sequence from LevelBase.
        isComplete = true;
        Debug.Log("Level 1 - Cracked Forest COMPLETE!");

        // 1. Hook into the ProgressionSystem to fulfill the game's reward loop (XP for completion).
        // For Level 1 puzzle completion, we award 30 Puzzle XP, matching the ProgressionSystem's default puzzle value.
        GameManager.Instance?.progressionSystem?.AddPuzzleXP(30);

        // 2. Provide final visual feedback to the player.
        uiManager?.DisplayObjective("Earth island restored!");

        // 3. Use the global GameManager Singleton to proceed with the core loop (AdvanceLevel handles rewards and state transition).
        GameManager.Instance?.AdvanceLevel();
    }

    // --- Level-Specific Helper Methods ---

    // <summary>
    // Activates (spawns) the specific Golem enemy objects defined for Level 1, allowing them to engage with the player.
    // </summary>
    public void SpawnEnemies()
    {
        // Safety check to ensure the array exists.
        if (golems == null) return;

        // Iterate through all golems and activate their GameObjects.
        foreach (Enemy g in golems)
        {
            if (g != null)
                g.gameObject.SetActive(true);
        }
    }

    // <summary>
    // Core gameplay loop logic for Level 1. This method runs every frame during active gameplay.
    // It checks the state of the BeamPuzzle, and if solved, opens the exit gate and updates the UI objective.
    // </summary>
    public void UpdatePuzzle()
    {
        // If the puzzle reference is missing, we cannot continue.
        if (puzzle == null) return;

        // 1. Evaluate if all pillars are correctly aligned using the puzzle component's function.
        bool allAligned = puzzle.CheckAllAligned();

        // 2. If the puzzle is solved AND the gate exit hasn't opened yet...
        if (allAligned && gate != null && !gate.isOpen)
        {
            // ...trigger the state change: Open the Gate.
            // This is the functional link between puzzle solution and level unlock state.
            gate.OpenGate();

            // ...and update the UIManager to direct the player toward the newly unlocked exit.
            uiManager?.UpdateObjective("Puzzle solved! Head to the gate!");
        }
    }

    // <summary>
    // Standard utility method (also defined in LevelBase's non-overridden form) to reset the level state.
    // Currently just a wrapper that calls InitializeLevel() again.
    // </summary>
    public void ResetLevel() => InitializeLevel();
}