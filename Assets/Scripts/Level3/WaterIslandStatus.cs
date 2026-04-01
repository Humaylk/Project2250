using UnityEngine;

// Tracks whether the Water Island has been restored or remains corrupted after level completion.
// Acts as a world-state flag for progression toward restoring balance in the realm.
// Follows the same structure as other island status trackers (e.g., EarthIslandStatus,
// FireIslandStatus) to monitor overall game progression across all five islands.
public class WaterIslandStatus : MonoBehaviour
{
    public bool isRestored = false;

    // Called by WaterIslandLevel.FinishLevel() when the player completes Level 3
    public void MarkRestored()
    {
        isRestored = true;
        Debug.Log("Water Island: Restored! The Drowned Vault has been cleansed.");
    }
}
