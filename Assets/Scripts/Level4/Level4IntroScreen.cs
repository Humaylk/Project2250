using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

// Builds and displays a full-screen intro overlay identical in layout to
// the Level 3 intro screen. Attach this script to the Level4IntroCanvas
// GameObject — it creates every UI element at runtime so no manual
// Inspector wiring is needed.
public class Level4IntroScreen : MonoBehaviour
{
    // Colors matching Level 3 style over Level 4's deep-purple background
    private static readonly Color PanelColor   = new Color(0.06f, 0.02f, 0.14f, 0.93f);
    private static readonly Color HeaderColor  = new Color(1f,    0.75f, 0f,    1f);   // orange-yellow
    private static readonly Color BodyColor    = new Color(0.4f,  0.85f, 0.9f,  1f);   // teal-cyan
    private static readonly Color PromptColor  = new Color(1f,    1f,    1f,    1f);   // white

    private Canvas        canvas;
    private GameObject    panel;
    private TMP_Text      aboutHeaderText;
    private TMP_Text      aboutBodyText;
    private TMP_Text      controlsHeaderText;
    private TMP_Text      controlsBodyText;
    private TMP_Text      pressAnyKeyText;

    private bool dismissed = false;

    void Awake()
    {
        canvas = GetComponent<Canvas>();
        if (canvas == null) canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        if (GetComponent<CanvasScaler>() == null)
        {
            CanvasScaler cs = gameObject.AddComponent<CanvasScaler>();
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

    // ---------------------------------------------------------------
    // Build every UI element the same way Level 3's canvas is laid out
    // ---------------------------------------------------------------
    private void BuildUI()
    {
        // --- Full-screen dark panel ---
        panel = CreateObject("Panel", transform);
        Image bg = panel.AddComponent<Image>();
        bg.color = PanelColor;
        StretchFull(panel.GetComponent<RectTransform>());

        // --- OBJECTIVE header ---
        aboutHeaderText = CreateText("AboutHeaderText", panel.transform,
            "OBJECTIVE", HeaderColor, 72, FontStyles.Bold,
            new Vector2(0.1f, 0.72f), new Vector2(0.9f, 0.84f));

        // --- Objective body ---
        aboutBodyText = CreateText("AboutBodyText", panel.transform,
            "You have entered the Sky Realm.\n" +
            "Collect all 3 energy cores to unlock the portal.\n" +
            "Watch out for the Golem guardians.\n" +
            "Complete before the timer runs out!",
            BodyColor, 40, FontStyles.Normal,
            new Vector2(0.1f, 0.50f), new Vector2(0.9f, 0.72f));

        // --- CONTROLS header ---
        controlsHeaderText = CreateText("ControlsHeaderText", panel.transform,
            "CONTROLS", HeaderColor, 72, FontStyles.Bold,
            new Vector2(0.1f, 0.36f), new Vector2(0.9f, 0.50f));

        // --- Controls body ---
        controlsBodyText = CreateText("ControlsBodyText", panel.transform,
            "Press G to attack\n" +
            "Collect glowing triangles for energy cores\n" +
            "Press H to advance to next level",
            BodyColor, 40, FontStyles.Normal,
            new Vector2(0.1f, 0.16f), new Vector2(0.9f, 0.36f));

        // --- Press any key prompt ---
        pressAnyKeyText = CreateText("PressAnyKeyText", panel.transform,
            "PRESS ANY KEY TO BEGIN", PromptColor, 44, FontStyles.Bold,
            new Vector2(0.1f, 0.04f), new Vector2(0.9f, 0.14f));
    }

    // Creates a child GameObject
    private static GameObject CreateObject(string name, Transform parent)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        return go;
    }

    // Creates a TMP_Text anchored by min/max in 0-1 canvas space
    private static TMP_Text CreateText(string name, Transform parent,
        string content, Color color, float fontSize, FontStyles style,
        Vector2 anchorMin, Vector2 anchorMax)
    {
        GameObject go = CreateObject(name, parent);
        TMP_Text t    = go.AddComponent<TextMeshProUGUI>();

        t.text      = content;
        t.color     = color;
        t.fontSize  = fontSize;
        t.fontStyle = style;
        t.alignment = TextAlignmentOptions.Center;
        t.enableWordWrapping = true;

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin  = anchorMin;
        rt.anchorMax  = anchorMax;
        rt.offsetMin  = Vector2.zero;
        rt.offsetMax  = Vector2.zero;

        return t;
    }

    // Stretches a RectTransform to fill its parent
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
