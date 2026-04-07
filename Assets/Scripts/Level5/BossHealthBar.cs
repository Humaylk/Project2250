using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Munadir: Creates a boss health bar UI at the top of the screen
// Munadir: Updates every frame based on ElementalBoss current HP
// Munadir: Changes color based on boss phase (white -> orange -> red)
public class BossHealthBar : MonoBehaviour
{
    [Header("Reference")]
    public ElementalBoss boss;

    private Canvas     canvas;
    private Image      barBackground;
    private Image      barFill;
    private TMP_Text   bossNameText;
    private TMP_Text   bossHPText;

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
        if (boss == null) return;

        float hpPercent = (float)boss.currentHP / boss.maxHP;
        hpPercent = Mathf.Clamp01(hpPercent);

        // Munadir: Update fill amount
        if (barFill != null)
        {
            barFill.fillAmount = hpPercent;

            // Munadir: Color matches boss phase
            if (hpPercent <= 0.33f)
                barFill.color = new Color(1f, 0.1f, 0.1f, 1f);
            else if (hpPercent <= 0.66f)
                barFill.color = new Color(1f, 0.6f, 0.2f, 1f);
            else
                barFill.color = new Color(0.6f, 0.1f, 1f, 1f);
        }

        // Munadir: Update HP number text
        if (bossHPText != null)
            bossHPText.text = boss.currentHP + " / " + boss.maxHP;

        // Munadir: Hide bar when boss is dead
        if (boss.IsDefeated() && barBackground != null)
        {
            barBackground.gameObject.SetActive(false);
            if (bossNameText != null) bossNameText.gameObject.SetActive(false);
            if (bossHPText != null) bossHPText.gameObject.SetActive(false);
        }
    }

    private void BuildUI()
    {
        // Munadir: Boss name above bar
        GameObject nameGO = new GameObject("BossName", typeof(RectTransform));
        nameGO.transform.SetParent(transform, false);
        bossNameText = nameGO.AddComponent<TextMeshProUGUI>();
        bossNameText.text      = "ELEMENTAL DRAGON";
        bossNameText.color     = new Color(0.95f, 0.15f, 0.9f, 1f);
        bossNameText.fontSize  = 32;
        bossNameText.fontStyle = FontStyles.Bold;
        bossNameText.alignment = TextAlignmentOptions.Center;
        RectTransform nrt = nameGO.GetComponent<RectTransform>();
        nrt.anchorMin = new Vector2(0.25f, 0.92f);
        nrt.anchorMax = new Vector2(0.75f, 0.97f);
        nrt.offsetMin = Vector2.zero;
        nrt.offsetMax = Vector2.zero;

        // Munadir: Background bar (dark)
        GameObject bgGO = new GameObject("BarBG", typeof(RectTransform));
        bgGO.transform.SetParent(transform, false);
        barBackground = bgGO.AddComponent<Image>();
        barBackground.color = new Color(0.15f, 0.05f, 0.2f, 0.85f);
        RectTransform bgrt = bgGO.GetComponent<RectTransform>();
        bgrt.anchorMin = new Vector2(0.25f, 0.88f);
        bgrt.anchorMax = new Vector2(0.75f, 0.92f);
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
        frt.offsetMin = new Vector2(2, 2);
        frt.offsetMax = new Vector2(-2, -2);

        // Munadir: HP text on top of bar
        GameObject hpGO = new GameObject("BossHPText", typeof(RectTransform));
        hpGO.transform.SetParent(bgGO.transform, false);
        bossHPText = hpGO.AddComponent<TextMeshProUGUI>();
        bossHPText.text      = "300 / 300";
        bossHPText.color     = Color.white;
        bossHPText.fontSize  = 24;
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
