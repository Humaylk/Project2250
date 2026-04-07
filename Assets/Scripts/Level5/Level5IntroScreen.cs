using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

// Builds and displays a full-screen intro overlay for Level 5 - Aether Nexus.
// Attach to a Level5IntroCanvas GameObject — builds every UI element at runtime.
public class Level5IntroScreen : MonoBehaviour
{
    // Arcane / aether color theme derived from the Aether Nexus scene
    private static readonly Color PanelColor  = new Color(0.06f, 0.01f, 0.12f, 0.93f); // deep dark purple
    private static readonly Color HeaderColor = new Color(0.95f, 0.15f, 0.90f, 1f);    // vivid magenta-pink
    private static readonly Color BodyColor   = new Color(0.78f, 0.55f, 1.0f,  1f);    // soft lavender
    private static readonly Color PromptColor = new Color(1f,    1f,    1f,    1f);    // white

    private Canvas     canvas;
    private GameObject panel;
    private TMP_Text   aboutHeaderText;
    private TMP_Text   aboutBodyText;
    private TMP_Text   controlsHeaderText;
    private TMP_Text   controlsBodyText;
    private TMP_Text   pressAnyKeyText;

    private bool dismissed = false;

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
        // Full-screen dark panel
        panel = CreateObject("Panel", transform);
        Image bg = panel.AddComponent<Image>();
        bg.color = PanelColor;
        StretchFull(panel.GetComponent<RectTransform>());

        // OBJECTIVE header
        aboutHeaderText = CreateText("AboutHeaderText", panel.transform,
            "OBJECTIVE", HeaderColor, 72, FontStyles.Bold,
            new Vector2(0.1f, 0.72f), new Vector2(0.9f, 0.84f));

        // Objective body
        aboutBodyText = CreateText("AboutBodyText", panel.transform,
            "You have entered the Aether Nexus — the final island.\n" +
            "The Elemental Dragon awaits. Defeat it before time runs out.\n" +
            "Dodge the laser cannons in the corners.\n" +
            "This is your last stand — free the realm!",
            BodyColor, 40, FontStyles.Normal,
            new Vector2(0.1f, 0.50f), new Vector2(0.9f, 0.72f));

        // CONTROLS header
        controlsHeaderText = CreateText("ControlsHeaderText", panel.transform,
            "CONTROLS", HeaderColor, 72, FontStyles.Bold,
            new Vector2(0.1f, 0.36f), new Vector2(0.9f, 0.50f));

        // Controls body
        controlsBodyText = CreateText("ControlsBodyText", panel.transform,
            "Press G to attack\n" +
            "Press F to use Fire ability\n" +
            "Press P for Heavy Attack\n" +
            "Press H to advance to the next level",
            BodyColor, 40, FontStyles.Normal,
            new Vector2(0.1f, 0.16f), new Vector2(0.9f, 0.36f));

        // Press any key prompt
        pressAnyKeyText = CreateText("PressAnyKeyText", panel.transform,
            "PRESS ANY KEY TO FACE THE DRAGON", PromptColor, 44, FontStyles.Bold,
            new Vector2(0.1f, 0.04f), new Vector2(0.9f, 0.14f));
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
