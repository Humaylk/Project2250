using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

// Displays a full-screen intro overlay at the start of Level 3.
// Pauses the entire game (timeScale = 0) until the player dismisses it.
// Uses the Thaleah pixel font for all text.
public class Level3IntroScreen : MonoBehaviour
{
    [Header("Panel")]
    public GameObject overlayPanel;

    [Header("Text Elements")]
    public TMP_Text aboutHeaderText;
    public TMP_Text aboutBodyText;
    public TMP_Text controlsHeaderText;
    public TMP_Text controlsBodyText;
    public TMP_Text pressAnyKeyText;

    [Header("Colors")]
    public Color headerColor  = new Color(1f, 0.75f, 0f, 1f);    // orange-yellow
    public Color bodyColor    = new Color(0.4f, 0.85f, 0.9f, 1f); // teal-cyan
    public Color promptColor  = new Color(1f, 1f, 1f, 1f);         // white

    private bool dismissed = false;

    void Start()
    {
        // Freeze everything until the player dismisses
        Time.timeScale = 0f;

        ApplyColors();
        ApplyText();

        if (overlayPanel != null)
            overlayPanel.SetActive(true);

        StartCoroutine(BlinkPrompt());
    }

    void Update()
    {
        if (dismissed) return;

        // Any key or mouse click dismisses the screen
        if (Input.anyKeyDown)
            Dismiss();
    }

    private void ApplyText()
    {
        if (aboutHeaderText  != null) aboutHeaderText.text  = "Objective";
        if (controlsHeaderText != null) controlsHeaderText.text = "CONTROLS";

        if (aboutBodyText != null)
            aboutBodyText.text =
                "You are now in the drowned vault.\n" +
                "Eliminate the killer fishes and defuse the bombs\n" +
                "within the time to escape alive.\n" +
                "Pick up the scuba helmet to help you.";

        if (controlsBodyText != null)
            controlsBodyText.text =
                "Press G to attack\n" +
                "Press E to open chest or pickup items\n" +
                "Hold E to defuse a mine\n" +
                "Press H to advance to next level";

        if (pressAnyKeyText != null)
            pressAnyKeyText.text = "PRESS ANY KEY TO DIVE IN";
    }

    private void ApplyColors()
    {
        if (aboutHeaderText   != null) aboutHeaderText.color   = headerColor;
        if (controlsHeaderText!= null) controlsHeaderText.color= headerColor;
        if (aboutBodyText     != null) aboutBodyText.color     = bodyColor;
        if (controlsBodyText  != null) controlsBodyText.color  = bodyColor;
        if (pressAnyKeyText   != null) pressAnyKeyText.color   = promptColor;
    }

    // Blinks the "press any key" prompt so it draws attention
    private IEnumerator BlinkPrompt()
    {
        while (!dismissed)
        {
            if (pressAnyKeyText != null)
            {
                pressAnyKeyText.alpha = 1f;
                yield return new WaitForSecondsRealtime(0.6f);
                pressAnyKeyText.alpha = 0f;
                yield return new WaitForSecondsRealtime(0.4f);
            }
            else
            {
                yield return null;
            }
        }
    }

    private void Dismiss()
    {
        dismissed = true;

        // Unfreeze the game
        Time.timeScale = 1f;

        if (overlayPanel != null)
            overlayPanel.SetActive(false);
    }
}
