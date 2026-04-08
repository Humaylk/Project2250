using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Munadir: Creates a boss health bar at the BOTTOM-CENTER of the screen
// Munadir: Positioned below HP text to avoid overlapping objective
// Munadir: Updates every frame, changes color per phase, shows HP numbers
// Munadir: Auto-finds boss if not assigned in Inspector
public class BossHealthBar : MonoBehaviour
{
    [Header("Reference")]
    public ElementalBoss boss;

    private Canvas     canvas;
    private Image      barBackground;
    private Image      barFill;
    private TMP_Text   bossNameText;
    private TMP_Text   bossHPText;

    private static TMP_FontAsset _cachedFont;
    private static TMP_FontAsset GetFont()
    {
        if (_cachedFont == null) _cachedFont = TMP_Settings.defaultFontAsset;
        if (_cachedFont == null) _cachedFont = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF - Fallback");
        return _cachedFont;
    }

    void Awake()
    {
        canvas = GetComponent<Canvas>();
        if (canvas == null) canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode   = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 50;

        if (GetComponent<CanvasScaler>() == null)
        {
            CanvasScaler cs        = gameObject.AddComponent<CanvasScaler>();
            cs.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            cs.referenceResolution = new Vector2(1920, 1080);
            cs.matchWidthOrHeight  = 0.5f;
        }

        BuildUI();
    }

    void Update()
    {
        // Munadir: Auto-find boss if not assigned in Inspector
        if (boss == null)
        {
            boss = FindFirstObjectByType<ElementalBoss>();
            if (boss == null) return;
        }

        float hpPercent = (float)boss.currentHP / boss.maxHP;
        hpPercent = Mathf.Clamp01(hpPercent);

        if (barFill != null)
        {
            barFill.fillAmount = hpPercent;

            // Munadir: Color matches boss phase
            if (hpPercent <= 0.33f)
                barFill.color = new Color(1f, 0.1f, 0.1f, 1f);      // Red
            else if (hpPercent <= 0.66f)
                barFill.color = new Color(1f, 0.6f, 0.2f, 1f);      // Orange
            else
                barFill.color = new Color(0.6f, 0.1f, 1f, 1f);      // Purple
        }

        // Munadir: Show actual HP numbers on the bar
        if (bossHPText != null)
            bossHPText.text = boss.currentHP + " / " + boss.maxHP;

        // Munadir: Hide everything when boss is dead
        if (boss.IsDefeated())
        {
            if (barBackground != null) barBackground.gameObject.SetActive(false);
            if (bossNameText != null) bossNameText.gameObject.SetActive(false);
        }
    }

    private void BuildUI()
    {
        TMP_FontAsset font = GetFont();

        // Munadir: Boss name - positioned at bottom center, above the bar
        GameObject nameGO = new GameObject("BossName", typeof(RectTransform));
        nameGO.transform.SetParent(transform, false);
        bossNameText = nameGO.AddComponent<TextMeshProUGUI>();
        bossNameText.font      = font;
        bossNameText.text      = "ELEMENTAL DRAGON";
        bossNameText.color     = new Color(0.95f, 0.15f, 0.9f, 1f);
        bossNameText.fontSize  = 28;
        bossNameText.fontStyle = FontStyles.Bold;
        bossNameText.alignment = TextAlignmentOptions.Center;
        RectTransform nrt = nameGO.GetComponent<RectTransform>();
        nrt.anchorMin = new Vector2(0.25f, 0.12f);
        nrt.anchorMax = new Vector2(0.75f, 0.17f);
        nrt.offsetMin = Vector2.zero;
        nrt.offsetMax = Vector2.zero;

        // Munadir: Background bar (dark) - BOTTOM of screen to avoid overlapping objective
        GameObject bgGO = new GameObject("BarBG", typeof(RectTransform));
        bgGO.transform.SetParent(transform, false);
        barBackground = bgGO.AddComponent<Image>();
        barBackground.color = new Color(0.15f, 0.05f, 0.2f, 0.9f);
        RectTransform bgrt = bgGO.GetComponent<RectTransform>();
        bgrt.anchorMin = new Vector2(0.2f, 0.06f);
        bgrt.anchorMax = new Vector2(0.8f, 0.11f);
        bgrt.offsetMin = Vector2.zero;
        bgrt.offsetMax = Vector2.zero;

        // Munadir: Fill bar (colored)
        GameObject fillGO = new GameObject("BarFill", typeof(RectTransform));
        fillGO.transform.SetParent(bgGO.transform, false);
        barFill = fillGO.AddComponent<Image>();
        barFill.color = new Color(0.6f, 0.1f, 1f, 1f);
        barFill.type = Image.Type.Filled;
        barFill.fillMethod = Image.FillMethod.Horizontal;
        barFill.fillAmount = 1f;
        RectTransform frt = fillGO.GetComponent<RectTransform>();
        frt.anchorMin = Vector2.zero;
        frt.anchorMax = Vector2.one;
        frt.offsetMin = new Vector2(3, 3);
        frt.offsetMax = new Vector2(-3, -3);

        // Munadir: HP numbers displayed on the bar (e.g. "245 / 300")
        GameObject hpGO = new GameObject("BossHPText", typeof(RectTransform));
        hpGO.transform.SetParent(bgGO.transform, false);
        bossHPText = hpGO.AddComponent<TextMeshProUGUI>();
        bossHPText.font      = font;
        bossHPText.text      = "300 / 300";
        bossHPText.color     = Color.white;
        bossHPText.fontSize  = 22;
        bossHPText.outlineWidth = 0.3f;
        bossHPText.outlineColor = Color.black;
        bossHPText.fontStyle = FontStyles.Bold;
        bossHPText.alignment = TextAlignmentOptions.Center;
        RectTransform hrt = hpGO.GetComponent<RectTransform>();
        hrt.anchorMin = Vector2.zero;
        hrt.anchorMax = Vector2.one;
        hrt.offsetMin = Vector2.zero;
        hrt.offsetMax = Vector2.zero;
    }
}
