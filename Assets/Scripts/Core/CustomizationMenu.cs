using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CustomizationMenu : MonoBehaviour
{
    [Header("Panel")]
    public GameObject menuPanel;

    [Header("Color Buttons")]
    public Button[] colorButtons;
    public Color[] colorOptions = {
        Color.blue,
        Color.white,
        new Color(0.55f, 0.27f, 0.07f),
        Color.black
    };

    [Header("Style Buttons")]
    public Button[] styleButtons;
    public TMP_Text styleNameText;
    public TMP_Text lockedText;

    [Header("Player")]
    public SpriteRenderer playerSprite;

    private PlayerAppearance appearance;
    private bool menuOpen = false;

 // standard unity lifecycle method for initialization, runs once on startup.
void Start()
{
    // create/initialize a new player appearance data object.
    appearance = new PlayerAppearance();

    // loop through available color buttons (ensuring we don't go out of bounds for buttons or options).
    for (int i = 0; i < colorButtons.Length && i < colorOptions.Length; i++)
    {
        // capture the current index (important for closure, ensures each button applies its specific color).
        int index = i;
        // add a listener to the button's onClick event, calling ApplyColor with the captured index when clicked.
        colorButtons[i].onClick.AddListener(() => ApplyColor(index));
        // set the visual color of the button's Image component to match the corresponding color option.
        colorButtons[i].GetComponent<Image>().color = colorOptions[i];
    }

    // loop through available style buttons to set them up in a similar manner.
    for (int i = 0; i < styleButtons.Length; i++)
    {
        // capture the index for closure.
        int index = i;
        // add a listener to apply the specific style when the button is clicked.
        styleButtons[i].onClick.AddListener(() => ApplyStyle(index));
    }

    // if the menu panel reference exists, ensure it is deactivated (hidden) initially.
    if (menuPanel != null)
        menuPanel.SetActive(false);

    // call RefreshUI to update all UI elements with their initial values/states.
    RefreshUI();
}
    }
// Every frame, check if the Tab key is pressed to open or close the menu.
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            ToggleMenu();
    }

    // Opens or closes the customization menu.
    public void ToggleMenu()
    {
        // Flip the boolean state tracking whether the menu is open or closed.
        menuOpen = !menuOpen;

        // If the menu panel exists, activate or deactivate it based on the new menu state.
        if (menuPanel != null)
            menuPanel.SetActive(menuOpen);

        // Pause time (`timeScale = 0`) if the menu is open, or resume normal time (`timeScale = 1`) if closed.
        // This is a common way to pause gameplay during menus.
        Time.timeScale = menuOpen ? 0f : 1f;

        // Update all UI elements to reflect the current menu state and player settings.
        RefreshUI();
    }

    // Applies a selected color to the player's appearance.
    private void ApplyColor(int colorIndex)
    {
        // If player appearance data is missing or the color index is invalid, exit the method.
        if (appearance == null || colorIndex >= colorOptions.Length) return;

        // Update the player's outfit color in their appearance data.
        appearance.outfitColor = colorOptions[colorIndex];

        // Immediately apply this new color to the actual player sprite.
        ApplyToSprite();
    }

    // Applies a selected outfit style to the player's appearance, checking if it's unlocked first.
    private void ApplyStyle(int styleIndex)
    {
        // If player appearance data is missing, exit the method.
        if (appearance == null) return;

        // Check if the selected style index is actually unlocked for the player.
        if (!appearance.IsStyleUnlocked(styleIndex))
        {
            // If locked: if a text element exists, display a message that this style is locked.
            if (lockedText != null)
                lockedText.text = PlayerAppearance.StyleNames[styleIndex] + " is locked!";
            
            // Do not apply the locked style; exit the method.
            return;
        }

        // If unlocked: update the player's outfit style in their appearance data.
        appearance.outfitStyle = styleIndex;

        // Apply the visual change to the player sprite and update the UI.
        ApplyToSprite();
        RefreshUI();
    }

    // Directly updates the color of the player sprite based on the current appearance settings.
    private void ApplyToSprite()
    {
        // If the player sprite renderer exists, update its color to match the appearance color.
        if (playerSprite != null)
            playerSprite.color = appearance.outfitColor;
    }

    // Updates all relevant UI text and button elements based on the current player appearance and style unlock status.
    private void RefreshUI()
    {
        // If player appearance data is missing, exit the method.
        if (appearance == null) return;

        // If text elements exist, update the style name display and clear any locked messages.
        if (styleNameText != null)
            styleNameText.text = "Style: " + PlayerAppearance.StyleNames[appearance.outfitStyle];
        if (lockedText != null)
            lockedText.text = "";

        // Loop through all style selection buttons and up-to StyleNames array length (whichever is smaller)
        for (int i = 0; i < styleButtons.Length && i < PlayerAppearance.StyleNames.Length; i++)
        {
            // Skip if the current button reference is null.
            if (styleButtons[i] == null) continue;

            // Determine if the style associated with this button is unlocked.
            bool unlocked = appearance.IsStyleUnlocked(i);

            // Set the button's interactability based on whether the style is unlocked.
            // Players should only be able to click buttons for unlocked styles.
            styleButtons[i].interactable = unlocked;

            // Update the button's child text component (if it has one) to show style name and potentially '(Locked)'.
            TMP_Text btnText = styleButtons[i].GetComponentInChildren<TMP_Text>();
            if (btnText != null)
                btnText.text = unlocked ? PlayerAppearance.StyleNames[i] : PlayerAppearance.StyleNames[i] + " (Locked)";
        }
    }
}