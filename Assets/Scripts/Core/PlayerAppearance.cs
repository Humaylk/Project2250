using UnityEngine;
using System;

// <summary>
// A serializable class that stores the data related to the player's visual appearance and customization state.
// Being [Serializable] allows this data to be viewed and edited in the Unity Inspector and easily saved/loaded.
// </summary>
[Serializable]
public class PlayerAppearance
{
    [Header("Current Cosmetics")]
    // The current color applied to the player's outfit sprite renderer.
    public Color outfitColor = Color.blue;
    // The index reference for the currently equipped outfit style from the StyleNames array.
    public int outfitStyle = 0;

    [Header("Progression/Unlock Flags")]
    // These booleans act as flags to track which equipment rewards the player has obtained.
    // They are used to determine which cosmetic styles are available for selection.
    public bool hasMetalSword = false;
    public bool hasNewArmor = false;
    public bool hasElementalArmor = false;

    // <summary>
    // A static, read-only list defining the distinct visual styles available for the player's appearance.
    // The indices of this array correspond to the 'outfitStyle' integer.
    // </summary>
    public static readonly string[] StyleNames = {
        "Adventurer",      // Default style
        "Hooded Rogue",    // Default style
        "Iron Plate",      // Unlocked by hasNewArmor
        "Elemental Set"    // Unlocked by hasElementalArmor
    };

    // <summary>
    // Checks if a specific outfit style is unlocked and available for the player to equip.
    // This logic ties visual customization choices directly to game progression flags.
    // </summary>
    // <param name="styleIndex">The index of the style in the StyleNames array to check.</param>
    // <returns>True if the style is unlocked (or default), false if it is locked.</returns>
    public bool IsStyleUnlocked(int styleIndex)
    {
        // Evaluate the lock status based on the provided index.
        switch (styleIndex)
        {
            // First two styles (Adventurer and Hooded Rogue) are default and always unlocked.
            case 0: return true;
            case 1: return true;
            
            // The "Iron Plate" style requires the 'hasNewArmor' progression flag to be true.
            case 2: return hasNewArmor;
            
            // The "Elemental Set" style requires the 'hasElementalArmor' flag to be true.
            case 3: return hasElementalArmor;
            
            // If an invalid index is provided, default to locked.
            default: return false;
        }
    }
}