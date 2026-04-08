using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

// Munadir: Builds and displays a full-screen intro overlay for Level 5 - Aether Nexus.
// Munadir: Uses ThaleahFat font to match other levels
public class Level5IntroScreen : MonoBehaviour
{
    private static readonly Color PanelColor  = new Color(0.06f, 0.01f, 0.12f, 0.93f);
    private static readonly Color HeaderColor = new Color(0.95f, 0.15f, 0.90f, 1f);
    private static readonly Color BodyColor   = new Color(0.78f, 0.55f, 1.0f,  1f);
    private static readonly Color PromptColor = new Color(1f,    1f,    1f,    1f);

    private Canvas     canvas;
    private GameObject panel;
    private TMP_Text   pressAnyKeyText;
    private bool dismissed = false;

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
        canvas = GetComponent<Canvas>();
        if (canvas == null) canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode   = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        if (GetComponent<CanvasScaler>() == null)
        {
            CanvasScaler cs        = gameObject.AddComponent<CanvasScaler>();
            cs.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            cs.referenceResolution = new Vector2(1920, 1080);
            cs.matchWidthOrHeight  = 0.5f;
        }

        if (GetComponent<GraphicRaycaster>() == null)
            gameObject.AddComponent<GraphicRaycaster>();

        BuildUI();
    }

    void Start()
    {
        Time.timeScale = 0f;
        StartCoroutine(BlinkPrompt());
    }

    void Update()
    {
        if (!dismissed && Input.anyKeyDown)
            Dismiss();
    }

    private void BuildUI()
    {
        panel = CreateObject("Panel", transform);
        Image bg = panel.AddComponent<Image>();
        bg.color = PanelColor;
        StretchFull(panel.GetComponent<RectTransform>());

        CreateText("TitleText", panel.transform,
            "LEVEL 5 — AETHER NEXUS", HeaderColor, 80, FontStyles.Bold,
            new Vector2(0.1f, 0.82f), new Vector2(0.9f, 0.95f));

        CreateText("ObjectiveHeader", panel.transform,
            "OBJECTIVE", HeaderColor, 56, FontStyles.Bold,
            new Vector2(0.1f, 0.72f), new Vector2(0.9f, 0.82f));

        CreateText("ObjectiveBody", panel.transform,
            "The Elemental Dragon guards the final island.\n" +
            "Defeat it before time runs out to restore balance.\n" +
            "Dodge the laser cannons firing from the corners!",
            BodyColor, 36, FontStyles.Normal,
            new Vector2(0.1f, 0.54f), new Vector2(0.9f, 0.72f));

        CreateText("ControlsHeader", panel.transform,
            "CONTROLS", HeaderColor, 56, FontStyles.Bold,
            new Vector2(0.1f, 0.44f), new Vector2(0.9f, 0.54f));

        CreateText("ControlsBody", panel.transform,
            "G  —  Sword Attack  (melee combo)\n" +
            "F  —  Fireball  (ranged, 3s cooldown)\n" +
            "P  —  Heavy Attack  (close range, 2s cooldown)\n" +
            "Arrow Keys / WASD  —  Move",
            BodyColor, 36, FontStyles.Normal,
            new Vector2(0.1f, 0.20f), new Vector2(0.9f, 0.44f));

        pressAnyKeyText = CreateText("PressAnyKeyText", panel.transform,
            "PRESS ANY KEY TO FACE THE DRAGON", PromptColor, 44, FontStyles.Bold,
            new Vector2(0.1f, 0.04f), new Vector2(0.9f, 0.16f));
    }

    private static GameObject CreateObject(string name, Transform parent)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        return go;
    }

    private static TMP_Text CreateText(string name, Transform parent,
        string content, Color color, float fontSize, FontStyles style,
        Vector2 anchorMin, Vector2 anchorMax)
    {
        GameObject go = CreateObject(name, parent);
        TMP_Text t    = go.AddComponent<TextMeshProUGUI>();
        t.font               = GetFont();
        t.text               = content;
        t.color              = color;
        t.fontSize           = fontSize;
        t.fontStyle          = style;
        t.alignment          = TextAlignmentOptions.Center;
        t.enableWordWrapping = true;

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        return t;
    }

    private static void StretchFull(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    private IEnumerator BlinkPrompt()
    {
        while (!dismissed)
        {
            if (pressAnyKeyText != null) pressAnyKeyText.alpha = 1f;
            yield return new WaitForSecondsRealtime(0.6f);
            if (pressAnyKeyText != null) pressAnyKeyText.alpha = 0f;
            yield return new WaitForSecondsRealtime(0.4f);
        }
    }

    private void Dismiss()
    {
        dismissed = true;
        Time.timeScale = 1f;
        if (panel != null) panel.SetActive(false);
    }
}
