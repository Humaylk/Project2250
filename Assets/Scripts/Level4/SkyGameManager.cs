using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class SkyGameManager : MonoBehaviour
{
    public int total = 3;
    private int current = 0;

    // Legacy Inspector refs kept so existing scene YAML doesn't break
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI progressText;
    public TextMeshProUGUI timerText;

    public float timeLeft = 20f;
    public bool levelComplete = false;

    // ── Built-at-runtime UI ───────────────────────────────────────────
    private TMP_FontAsset _font;
    private TMP_Text      _popupText;
    private TMP_Text      _progressHUD;
    private TMP_Text      _timerHUD;
    private GameObject    _gatePopupGO;

    // ─────────────────────────────────────────────────────────────────
    void Start()
    {
        _font = LoadFont();
        BuildHUD();
        HideOldUI();
        StartCoroutine(ShowObjectivePopup());
    }

    void Update()
    {
        if (levelComplete) return;

        timeLeft -= Time.deltaTime;
        if (_timerHUD != null)
            _timerHUD.text = "Time: " + Mathf.CeilToInt(Mathf.Max(0, timeLeft));

        if (timeLeft <= 0)
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // ── Public API (called by SkyTriangle) ────────────────────────────
    public void AddProgress()
    {
        if (levelComplete) return;
        current++;
        UpdateProgressHUD();
        if (current >= total)
            EndLevel();
    }

    public int GetCurrentProgress() { return current; }

    // ── Internal ──────────────────────────────────────────────────────
    void EndLevel()
    {
        levelComplete = true;
        FreezeEnemies();
        ShowGateOpenedPopup();
    }

    void FreezeEnemies()
    {
        foreach (var g in FindObjectsByType<GolemAI_Level4>(FindObjectsSortMode.None))
            g.enabled = false;
    }

    void UpdateProgressHUD()
    {
        if (_progressHUD != null)
            _progressHUD.text = current + "/" + total;

        // Update the world-space label on every triangle
        foreach (var tri in FindObjectsByType<SkyTriangle>(FindObjectsSortMode.None))
            tri.UpdateProgressLabel(current, total);
    }

    // ── Font loader ───────────────────────────────────────────────────
    TMP_FontAsset LoadFont()
    {
        TMP_FontAsset f = Resources.Load<TMP_FontAsset>("Fonts & Materials/ThaleahFat_TTF SDF");
        if (f != null) return f;
        foreach (var t in FindObjectsByType<TMP_Text>(FindObjectsSortMode.None))
            if (t.font != null && t.font.name.Contains("Thaleah")) return t.font;
        return null;
    }

    // ── Hide old serialized UI so it doesn't conflict ─────────────────
    void HideOldUI()
    {
        if (titleText    != null) titleText.gameObject.SetActive(false);
        if (progressText != null) progressText.gameObject.SetActive(false);
        if (timerText    != null) timerText.gameObject.SetActive(false);
    }

    // ── Build runtime HUD canvas ──────────────────────────────────────
    void BuildHUD()
    {
        GameObject canvasGO = new GameObject("Level4HUD");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode   = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 50;
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight  = 0.5f;
        canvasGO.AddComponent<GraphicRaycaster>();

        // Progress counter — top-centre
        _progressHUD = MakeText(canvasGO.transform, "ProgressHUD",
            current + "/" + total,
            new Color(0.15f, 0.8f, 1f, 1f), 52, FontStyles.Bold,
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
            new Vector2(0f, -20f), 200, 70);

        // Timer — top-right
        _timerHUD = MakeText(canvasGO.transform, "TimerHUD",
            "Time: " + Mathf.CeilToInt(timeLeft),
            new Color(1f, 0.85f, 0.2f, 1f), 42, FontStyles.Bold,
            new Vector2(1f, 1f), new Vector2(1f, 1f),
            new Vector2(-20f, -20f), 200, 60);
        _timerHUD.alignment = TextAlignmentOptions.Right;
    }

    // ── Objective popup — centred, fades after 5 s ────────────────────
    IEnumerator ShowObjectivePopup()
    {
        GameObject canvasGO = new GameObject("ObjectivePopupCanvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode   = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 60;
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight  = 0.5f;

        // Dark backing panel
        GameObject panelGO = new GameObject("PopupPanel");
        panelGO.transform.SetParent(canvasGO.transform, false);
        Image bg = panelGO.AddComponent<Image>();
        bg.color = new Color(0.04f, 0.01f, 0.12f, 0.85f);
        RectTransform prt = panelGO.GetComponent<RectTransform>();
        prt.anchorMin = new Vector2(0.15f, 0.38f);
        prt.anchorMax = new Vector2(0.85f, 0.62f);
        prt.offsetMin = prt.offsetMax = Vector2.zero;

        // Popup text
        _popupText = MakeText(panelGO.transform, "PopupText",
            "Solve all planet configurations before time runs out!\nKILL GOLEMS BEFORE COMPLETING ALL 3",
            new Color(0.72f, 0.55f, 0.95f, 1f), 38, FontStyles.Bold,
            Vector2.zero, Vector2.one,
            new Vector2(20f, 10f), -40, -20);

        // Hold for 3.5 s then fade over 1.5 s
        yield return new WaitForSeconds(3.5f);

        float elapsed = 0f;
        float fadeDuration = 1.5f;
        Image[] images = canvasGO.GetComponentsInChildren<Image>();
        TMP_Text[] texts = canvasGO.GetComponentsInChildren<TMP_Text>();

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = 1f - (elapsed / fadeDuration);
            foreach (var img in images)  { Color c = img.color;  c.a = alpha * (img == bg ? 0.85f : 1f); img.color  = c; }
            foreach (var txt in texts)   { Color c = txt.color;  c.a = alpha;                              txt.color  = c; }
            yield return null;
        }

        Destroy(canvasGO);
    }

    // ── Gate opened popup ─────────────────────────────────────────────
    void ShowGateOpenedPopup()
    {
        GameObject canvasGO = new GameObject("GateOpenedCanvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode   = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 60;
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight  = 0.5f;

        GameObject panelGO = new GameObject("GatePanel");
        panelGO.transform.SetParent(canvasGO.transform, false);
        Image bg = panelGO.AddComponent<Image>();
        bg.color = new Color(0.04f, 0.01f, 0.12f, 0.88f);
        RectTransform prt = panelGO.GetComponent<RectTransform>();
        prt.anchorMin = new Vector2(0.2f, 0.42f);
        prt.anchorMax = new Vector2(0.8f, 0.58f);
        prt.offsetMin = prt.offsetMax = Vector2.zero;

        MakeText(panelGO.transform, "GateText",
            "The gate has now opened! Press H to advance.",
            new Color(0.15f, 1f, 0.45f, 1f), 42, FontStyles.Bold,
            Vector2.zero, Vector2.one,
            new Vector2(20f, 10f), -40, -20);

        _gatePopupGO = canvasGO;
    }

    // ── Helper: create anchored TMP text ─────────────────────────────
    TMP_Text MakeText(Transform parent, string name, string content,
        Color color, float fontSize, FontStyles style,
        Vector2 anchorMin, Vector2 anchorMax,
        Vector2 offsetMin, float offsetMaxX, float offsetMaxY)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        TMP_Text t = go.AddComponent<TextMeshProUGUI>();
        t.text               = content;
        t.color              = color;
        t.fontSize           = fontSize;
        t.fontStyle          = style;
        t.alignment          = TextAlignmentOptions.Center;
        t.enableWordWrapping = true;
        if (_font != null) t.font = _font;

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.pivot     = anchorMin == anchorMax ? anchorMin : new Vector2(0.5f, 0.5f);
        rt.offsetMin = offsetMin;
        rt.offsetMax = new Vector2(offsetMaxX, offsetMaxY);
        return t;
    }
}
