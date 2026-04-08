using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

// Munadir: Full-screen death overlay for Level 5
// Munadir: Listens for PlayerHealth.OnDeath event and shows retry prompt
// Munadir: Uses ThaleahFat font to match other levels
public class Level5DeathScreen : MonoBehaviour
{
    private static readonly Color PanelColor  = new Color(0.15f, 0.0f, 0.0f, 0.92f);
    private static readonly Color HeaderColor = new Color(1f, 0.15f, 0.15f, 1f);
    private static readonly Color BodyColor   = new Color(1f, 0.6f, 0.6f, 1f);

    private GameObject panel;
    private TMP_Text   retryText;
    private bool       isShowing = false;

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

    void OnEnable()
    {
        PlayerHealth.OnDeath += ShowDeathScreen;
    }

    void OnDisable()
    {
        PlayerHealth.OnDeath -= ShowDeathScreen;
    }

    void Awake()
    {
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

    void Update()
    {
        if (isShowing && Input.GetKeyDown(KeyCode.R))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void ShowDeathScreen()
    {
        isShowing = true;
        panel.SetActive(true);
        StartCoroutine(PauseAfterDelay());
    }

    private IEnumerator PauseAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        Time.timeScale = 0f;
        StartCoroutine(BlinkRetry());
    }

    private void BuildUI()
    {
        TMP_FontAsset font = GetFont();

        panel = new GameObject("DeathPanel", typeof(RectTransform));
        panel.transform.SetParent(transform, false);
        Image bg = panel.AddComponent<Image>();
        bg.color = PanelColor;
        RectTransform rt = panel.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        GameObject header = new GameObject("DeathHeader", typeof(RectTransform));
        header.transform.SetParent(panel.transform, false);
        TMP_Text headerText = header.AddComponent<TextMeshProUGUI>();
        headerText.font      = font;
        headerText.text      = "YOU DIED";
        headerText.color     = HeaderColor;
        headerText.fontSize  = 120;
        headerText.fontStyle = FontStyles.Bold;
        headerText.alignment = TextAlignmentOptions.Center;
        RectTransform hrt = header.GetComponent<RectTransform>();
        hrt.anchorMin = new Vector2(0.1f, 0.5f);
        hrt.anchorMax = new Vector2(0.9f, 0.75f);
        hrt.offsetMin = Vector2.zero;
        hrt.offsetMax = Vector2.zero;

        GameObject sub = new GameObject("DeathSub", typeof(RectTransform));
        sub.transform.SetParent(panel.transform, false);
        TMP_Text subText = sub.AddComponent<TextMeshProUGUI>();
        subText.font      = font;
        subText.text      = "The Elemental Dragon has defeated you...";
        subText.color     = BodyColor;
        subText.fontSize  = 40;
        subText.fontStyle = FontStyles.Normal;
        subText.alignment = TextAlignmentOptions.Center;
        RectTransform srt = sub.GetComponent<RectTransform>();
        srt.anchorMin = new Vector2(0.1f, 0.38f);
        srt.anchorMax = new Vector2(0.9f, 0.50f);
        srt.offsetMin = Vector2.zero;
        srt.offsetMax = Vector2.zero;

        GameObject retry = new GameObject("RetryPrompt", typeof(RectTransform));
        retry.transform.SetParent(panel.transform, false);
        retryText = retry.AddComponent<TextMeshProUGUI>();
        retryText.font      = font;
        retryText.text      = "PRESS R TO RETRY";
        retryText.color     = Color.white;
        retryText.fontSize  = 48;
        retryText.fontStyle = FontStyles.Bold;
        retryText.alignment = TextAlignmentOptions.Center;
        RectTransform rrt = retry.GetComponent<RectTransform>();
        rrt.anchorMin = new Vector2(0.1f, 0.15f);
        rrt.anchorMax = new Vector2(0.9f, 0.30f);
        rrt.offsetMin = Vector2.zero;
        rrt.offsetMax = Vector2.zero;
    }

    private IEnumerator BlinkRetry()
    {
        while (isShowing)
        {
            if (retryText != null) retryText.alpha = 1f;
            yield return new WaitForSecondsRealtime(0.6f);
            if (retryText != null) retryText.alpha = 0f;
            yield return new WaitForSecondsRealtime(0.4f);
        }
    }
}
