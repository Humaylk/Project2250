using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

// Displays a full-screen intro overlay at the start of Level 4.
// Pauses the entire game (timeScale = 0) until the player dismisses it.
// Text objects are found automatically by name — no manual wiring needed.
public class Level4IntroScreen : MonoBehaviour
{
    [Header("Panel")]
    public GameObject overlayPanel;

    [Header("Text Elements (auto-found by name if left empty)")]
    public TMP_Text aboutHeaderText;
    public TMP_Text aboutBodyText;
    public TMP_Text controlsHeaderText;
    public TMP_Text controlsBodyText;
    public TMP_Text pressAnyKeyText;

    [Header("Sky Theme Colors")]
    public Color headerColor = new Color(0.45f, 0.85f, 1f,  1f); // sky blue
    public Color bodyColor   = new Color(0.85f, 0.95f, 1f,  1f); // cloud white-blue
    public Color promptColor = new Color(1f,    1f,    1f,  1f);  // white

    private bool dismissed = false;

    void Awake()
    {
        // Auto-find any TMP_Text children by their GameObject name
        // so the fields don't need to be manually assigned in the Inspector.
        TMP_Text[] all = GetComponentsInChildren<TMP_Text>(true);
        foreach (TMP_Text t in all)
        {
            string n = t.gameObject.name.ToLower();
            if (aboutHeaderText    == null && n.Contains("aboutheader"))    aboutHeaderText    = t;
            if (aboutBodyText      == null && n.Contains("aboutbody"))      aboutBodyText      = t;
            if (controlsHeaderText == null && n.Contains("controlsheader")) controlsHeaderText = t;
            if (controlsBodyText   == null && n.Contains("controlsbody"))   controlsBodyText   = t;
            if (pressAnyKeyText    == null && n.Contains("pressanykey"))    pressAnyKeyText    = t;
        }
    }

    void Start()
    {
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

        if (Input.anyKeyDown)
            Dismiss();
    }

    private void ApplyText()
    {
        if (aboutHeaderText    != null) aboutHeaderText.text    = "Objective";
        if (controlsHeaderText != null) controlsHeaderText.text = "CONTROLS";

        if (aboutBodyText != null)
            aboutBodyText.text =
                "You have entered the Sky Realm.\n" +
                "Collect all 3 energy cores to unlock the portal.\n" +
                "Watch out for the Golem guardians.\n" +
                "Complete before the timer runs out!";

        if (controlsBodyText != null)
            controlsBodyText.text =
                "Press G to attack\n" +
                "Collect glowing triangles for energy cores\n" +
                "Press H to advance to next level";

        if (pressAnyKeyText != null)
            pressAnyKeyText.text = "PRESS ANY KEY TO BEGIN";
    }

    private void ApplyColors()
    {
        if (aboutHeaderText    != null) aboutHeaderText.color    = headerColor;
        if (controlsHeaderText != null) controlsHeaderText.color = headerColor;
        if (aboutBodyText      != null) aboutBodyText.color      = bodyColor;
        if (controlsBodyText   != null) controlsBodyText.color   = bodyColor;
        if (pressAnyKeyText    != null) pressAnyKeyText.color    = promptColor;
    }

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
        Time.timeScale = 1f;

        if (overlayPanel != null)
            overlayPanel.SetActive(false);
    }
}
