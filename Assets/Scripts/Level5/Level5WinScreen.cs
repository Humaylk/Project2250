using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

// Munadir: Full-screen victory overlay for Level 5
// Munadir: Shown when boss is defeated - displays congratulations message
// Munadir: Press any key to return to Level 1
public class Level5WinScreen : MonoBehaviour
{
    private static readonly Color PanelColor  = new Color(0.02f, 0.0f, 0.08f, 0.93f);
    private static readonly Color HeaderColor = new Color(1f, 0.85f, 0.1f, 1f);
    private static readonly Color BodyColor   = new Color(0.8f, 0.9f, 1f, 1f);

    private GameObject panel;
    private TMP_Text   promptText;
    private bool isShowing = false;
    private bool canDismiss = false;

    public static Level5WinScreen Instance { get; private set; }

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

    void Update()
    {
        if (canDismiss && Input.anyKeyDown)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("Level1_CrackedForest");
        }
    }

    public void ShowWinScreen()
    {
        if (isShowing) return;
        isShowing = true;
        panel.SetActive(true);
        StartCoroutine(EnableDismissAfterDelay());
    }

    private IEnumerator EnableDismissAfterDelay()
    {
        // Munadir: Show victory for 3 seconds before allowing dismiss
        yield return new WaitForSeconds(2f);
        Time.timeScale = 0f;
        canDismiss = true;
        StartCoroutine(BlinkPrompt());
    }

    private void BuildUI()
    {
        panel = new GameObject("WinPanel", typeof(RectTransform));
        panel.transform.SetParent(transform, false);
        Image bg = panel.AddComponent<Image>();
        bg.color = PanelColor;
        RectTransform rt = panel.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        // Munadir: VICTORY header
        GameObject header = new GameObject("WinHeader", typeof(RectTransform));
        header.transform.SetParent(panel.transform, false);
        TMP_Text headerText = header.AddComponent<TextMeshProUGUI>();
        headerText.text      = "VICTORY";
        headerText.color     = HeaderColor;
        headerText.fontSize  = 120;
        headerText.fontStyle = FontStyles.Bold;
        headerText.alignment = TextAlignmentOptions.Center;
        RectTransform hrt = header.GetComponent<RectTransform>();
        hrt.anchorMin = new Vector2(0.1f, 0.60f);
        hrt.anchorMax = new Vector2(0.9f, 0.85f);
        hrt.offsetMin = Vector2.zero;
        hrt.offsetMax = Vector2.zero;

        // Munadir: Congratulations text
        GameObject body = new GameObject("WinBody", typeof(RectTransform));
        body.transform.SetParent(panel.transform, false);
        TMP_Text bodyText = body.AddComponent<TextMeshProUGUI>();
        bodyText.text = "The Elemental Dragon has been defeated!\n\n" +
                        "Balance has been restored to the realm.\n" +
                        "Elemental Armor unlocked!\n\n" +
                        "Alex has saved Quaziadore.";
        bodyText.color     = BodyColor;
        bodyText.fontSize  = 40;
        bodyText.fontStyle = FontStyles.Normal;
        bodyText.alignment = TextAlignmentOptions.Center;
        bodyText.enableWordWrapping = true;
        RectTransform brt = body.GetComponent<RectTransform>();
        brt.anchorMin = new Vector2(0.1f, 0.25f);
        brt.anchorMax = new Vector2(0.9f, 0.58f);
        brt.offsetMin = Vector2.zero;
        brt.offsetMax = Vector2.zero;

        // Munadir: Press any key prompt
        GameObject prompt = new GameObject("WinPrompt", typeof(RectTransform));
        prompt.transform.SetParent(panel.transform, false);
        promptText = prompt.AddComponent<TextMeshProUGUI>();
        promptText.text      = "PRESS ANY KEY TO CONTINUE";
        promptText.color     = Color.white;
        promptText.fontSize  = 44;
        promptText.fontStyle = FontStyles.Bold;
        promptText.alignment = TextAlignmentOptions.Center;
        RectTransform prt = prompt.GetComponent<RectTransform>();
        prt.anchorMin = new Vector2(0.1f, 0.08f);
        prt.anchorMax = new Vector2(0.9f, 0.20f);
        prt.offsetMin = Vector2.zero;
        prt.offsetMax = Vector2.zero;
    }

    private IEnumerator BlinkPrompt()
    {
        while (isShowing)
        {
            if (promptText != null) promptText.alpha = 1f;
            yield return new WaitForSecondsRealtime(0.6f);
            if (promptText != null) promptText.alpha = 0f;
            yield return new WaitForSecondsRealtime(0.4f);
        }
    }
}
