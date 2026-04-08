using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

// Munadir: Post-victory handler for Level 5
// Munadir: After boss dies, shows hint to go to gate
// Munadir: When player presses H at gate, Star Wars style credits scroll plays
// Munadir: Uses ThaleahFat font to match other levels
public class Level5WinScreen : MonoBehaviour
{
    private static readonly Color PanelColor  = new Color(0.0f, 0.0f, 0.02f, 0.95f);
    private static readonly Color HeaderColor = new Color(1f, 0.85f, 0.1f, 1f);
    private static readonly Color BodyColor   = new Color(0.75f, 0.82f, 1f, 1f);

    private GameObject panel;
    private TMP_Text   scrollText;
    private RectTransform scrollRect;
    private bool isShowing = false;
    private bool creditsStarted = false;

    public static Level5WinScreen Instance { get; private set; }

    // Munadir: Font loader — tries ThaleahFat first (matches other levels)
    private static TMP_FontAsset _cachedFont;
    private static TMP_FontAsset GetFont()
    {
        if (_cachedFont != null) return _cachedFont;
        foreach (var txt in FindObjectsByType<TMP_Text>(FindObjectsSortMode.None))
        {
            if (txt.font != null && txt.font.name.Contains("Thaleah"))
            {
                _cachedFont = txt.font;
                return _cachedFont;
            }
        }
        _cachedFont = TMP_Settings.defaultFontAsset;
        if (_cachedFont == null) _cachedFont = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF - Fallback");
        return _cachedFont;
    }

    void Awake()
    {
        Instance = this;

        Canvas canvas = GetComponent<Canvas>();
        if (canvas == null) canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode   = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 200;

        if (GetComponent<CanvasScaler>() == null)
        {
            CanvasScaler cs        = gameObject.AddComponent<CanvasScaler>();
            cs.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            cs.referenceResolution = new Vector2(1920, 1080);
            cs.matchWidthOrHeight  = 0.5f;
        }

        BuildUI();
        panel.SetActive(false);
    }

    // Munadir: Called by AetherNexusLevel when boss is defeated
    public void ShowWinScreen()
    {
        if (isShowing) return;
        isShowing = true;

        GameManager.Instance?.uiManager?.DisplayObjective("VICTORY! Walk to the gate and press H!");
        GameManager.Instance?.uiManager?.ShowHint("The Dragon is defeated! Head to the gate on the right!");
    }

    // Munadir: Called when player reaches gate and presses H
    public void StartCreditsScroll()
    {
        if (creditsStarted) return;
        creditsStarted = true;
        panel.SetActive(true);
        StartCoroutine(ScrollCredits());
    }

    private IEnumerator ScrollCredits()
    {
        // Munadir: Freeze gameplay
        PlayerController pc = FindFirstObjectByType<PlayerController>();
        if (pc != null) pc.enabled = false;

        // Munadir: Start text off screen at the bottom
        scrollRect.anchoredPosition = new Vector2(0f, -1200f);

        float scrollSpeed = 40f;
        float totalDistance = 3500f;
        float traveled = 0f;

        while (traveled < totalDistance)
        {
            float delta = scrollSpeed * Time.deltaTime;
            scrollRect.anchoredPosition += new Vector2(0f, delta);
            traveled += delta;
            yield return null;
        }

        // Munadir: Hold for a moment then show "THE END"
        yield return new WaitForSeconds(2f);

        scrollText.text = "\n\n\n\nTHE END\n\n\nThank you for playing\nElemental Dominion";
        scrollText.alignment = TextAlignmentOptions.Center;
        scrollText.fontSize = 60;
        scrollRect.anchoredPosition = Vector2.zero;

        yield return new WaitForSeconds(3f);
        Time.timeScale = 0f;
    }

    private void BuildUI()
    {
        // Munadir: Full black background
        panel = new GameObject("CreditsPanel", typeof(RectTransform));
        panel.transform.SetParent(transform, false);
        Image bg = panel.AddComponent<Image>();
        bg.color = PanelColor;
        RectTransform rt = panel.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        // Munadir: Mask so text clips at screen edges
        panel.AddComponent<RectMask2D>();

        // Munadir: Scrolling lore text — POST-VICTORY lore (what happens after the boss is defeated)
        GameObject textGO = new GameObject("ScrollText", typeof(RectTransform));
        textGO.transform.SetParent(panel.transform, false);
        scrollText = textGO.AddComponent<TextMeshProUGUI>();
        scrollText.font = GetFont();
        scrollText.text =
            "<size=72><color=#FFD700>ELEMENTAL DOMINION</color></size>\n\n\n" +
            "<size=44><color=#C8A0FF>THE AFTERMATH</color></size>\n\n\n" +
            "<size=34>" +
            "With the Elemental Dragon defeated,\n" +
            "its corrupted energy dissipated\n" +
            "into the swirling skies above.\n\n" +
            "The Aether Nexus, once pulsing\n" +
            "with dark power, grew silent.\n" +
            "The ground stopped trembling.\n" +
            "The laser cannons powered down.\n\n" +
            "One by one, the five islands\n" +
            "began to heal.\n\n" +
            "The Cracked Forest's trees\n" +
            "bloomed once more.\n" +
            "The Shadow Swamp's waters cleared.\n" +
            "The Drowned Vault rose\n" +
            "from the depths.\n" +
            "The Sky Kingdom's storms calmed.\n\n" +
            "Alex stood at the edge of the Nexus,\n" +
            "watching as light returned\n" +
            "to Quaziadore.\n\n" +
            "The Elemental Armor, forged from\n" +
            "the Dragon's own essence,\n" +
            "would protect these lands\n" +
            "for generations to come.\n\n" +
            "The warriors who once fought alone\n" +
            "now stood together — guardians\n" +
            "of a world reborn.\n\n" +
            "Balance was restored.\n" +
            "Not through destruction,\n" +
            "but through courage, unity,\n" +
            "and the will to never surrender.\n\n" +
            "The five islands of Quaziadore\n" +
            "would know peace...\n" +
            "forevermore.\n\n\n" +
            "</size>" +
            "<size=44><color=#FFD700>PEACE HAS BEEN RESTORED</color></size>\n\n\n\n" +
            "<size=30>" +
            "Developed by:\n\n" +
            "Munadir\n" +
            "Humayl\n" +
            "Yoseph\n" +
            "Neelesh\n" +
            "Shaan\n\n\n" +
            "SE2250B — Software Construction\n" +
            "Western University\n" +
            "2026\n\n\n\n\n" +
            "</size>";

        scrollText.color     = BodyColor;
        scrollText.fontSize  = 34;
        scrollText.fontStyle = FontStyles.Normal;
        scrollText.alignment = TextAlignmentOptions.Center;
        scrollText.enableWordWrapping = true;
        scrollText.richText = true;

        scrollRect = textGO.GetComponent<RectTransform>();
        scrollRect.anchorMin = new Vector2(0.1f, 0f);
        scrollRect.anchorMax = new Vector2(0.9f, 1f);
        scrollRect.sizeDelta = new Vector2(0f, 2500f);
        scrollRect.anchoredPosition = new Vector2(0f, -1200f);
    }
}
