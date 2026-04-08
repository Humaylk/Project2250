using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

// Munadir: Post-victory handler for Level 5
// Munadir: After boss dies, shows hint to go to gate
// Munadir: When player presses H at gate, Star Wars style credits scroll plays
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

    // Munadir: Load TMP font — required when creating TMP text at runtime
    private static TMP_FontAsset _cachedFont;
    private static TMP_FontAsset GetFont()
    {
        if (_cachedFont == null) _cachedFont = TMP_Settings.defaultFontAsset;
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
    // Munadir: Shows hint to walk to gate - doesn't freeze the game yet
    public void ShowWinScreen()
    {
        if (isShowing) return;
        isShowing = true;

        // Munadir: Show gate hint via UIManager
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

        // Munadir: Scrolling lore text
        GameObject textGO = new GameObject("ScrollText", typeof(RectTransform));
        textGO.transform.SetParent(panel.transform, false);
        scrollText = textGO.AddComponent<TextMeshProUGUI>();
        scrollText.font = GetFont();
        scrollText.text =
            "<size=72><color=#FFD700>ELEMENTAL DOMINION</color></size>\n\n\n" +
            "<size=44><color=#C8A0FF>CHAPTER V — THE AETHER NEXUS</color></size>\n\n\n" +
            "<size=34>" +
            "Long ago, the five islands of Quaziadore\n" +
            "existed in perfect harmony, each guarded\n" +
            "by an elemental force of nature.\n\n" +
            "But when the Elemental Dragon awakened\n" +
            "from its ancient slumber, corruption\n" +
            "spread across every island — twisting\n" +
            "creatures and shattering the balance.\n\n" +
            "A lone warrior named Alex set forth\n" +
            "on a journey across the five islands,\n" +
            "facing golems in the Cracked Forest,\n" +
            "wolves in the Shadow Swamp,\n" +
            "assassins in the Drowned Vault,\n" +
            "and sky beasts above the clouds.\n\n" +
            "With each island restored, Alex grew\n" +
            "stronger — earning new weapons, armor,\n" +
            "and the elemental abilities needed\n" +
            "to face the Dragon itself.\n\n" +
            "In the Aether Nexus, the final battle\n" +
            "raged beneath swirling purple skies.\n" +
            "Laser cannons fired. The Dragon roared.\n" +
            "But Alex stood firm.\n\n" +
            "With sword, fire, and heavy strikes,\n" +
            "the Elemental Dragon was defeated.\n" +
            "Balance was restored to Quaziadore.\n\n" +
            "The Elemental Armor, forged from the\n" +
            "Dragon's own power, now protects\n" +
            "the islands forevermore.\n\n\n" +
            "</size>" +
            "<size=44><color=#FFD700>PEACE HAS RETURNED</color></size>\n\n\n\n" +
            "<size=30>" +
            "Developed by:\n\n" +
            "Munadir\n" +
            "Humayl\n" +
            "Yoseph\n" +
            "Neelash\n" +
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
