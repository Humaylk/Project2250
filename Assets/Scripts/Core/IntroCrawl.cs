using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// Star Wars-style crawl that plays ONCE at the very start of Level 1.
/// When done, activates Level1IntroCanvas + Level1Intro (the controls screen).
/// Cannot be skipped. PlayerPrefs prevents it showing again after death restarts.
/// Right-click this component → "Reset Intro (Testing)" to test again.
/// </summary>
public class IntroCrawl : MonoBehaviour
{
    [Header("References")]
    public RectTransform textContainer;
    public GameObject    level1IntroCanvas;
    public GameObject    level1Intro;

    [Header("Settings")]
    public float scrollSpeed = 120f;   // canvas units per second (unscaled time)

    private const string SHOWN_KEY  = "Level1IntroCrawlShown";
    private const float  SCREEN_TOP = 560f;   // canvas units from centre to top of screen

    void Awake()
    {
        if (level1IntroCanvas != null) level1IntroCanvas.SetActive(false);
        if (level1Intro != null)       level1Intro.SetActive(false);

        if (PlayerPrefs.GetInt(SHOWN_KEY, 0) == 1)
        {
            ShowIntroScreen();
            gameObject.SetActive(false);
            return;
        }

        Time.timeScale = 0f;
    }

    void Start()
    {
        StartCoroutine(PlayCrawl());
    }

    private IEnumerator PlayCrawl()
    {
        if (textContainer != null)
            textContainer.anchoredPosition = new Vector2(0f, -540f);

        // Wait one frame so TMP can calculate the real rendered text height.
        yield return null;

        // Use the actual text height so we stop the moment the last word
        // clears the top of the screen — no dead wait on a blank canvas.
        TextMeshProUGUI tmp = textContainer != null
            ? textContainer.GetComponentInChildren<TextMeshProUGUI>()
            : null;

        float textHeight = (tmp != null && tmp.preferredHeight > 0f)
            ? tmp.preferredHeight
            : 1400f;   // fallback if TMP hasn't laid out yet

        // Scroll until the bottom edge of the actual text clears the screen top.
        // (pivot is top-centre, so bottom = anchoredPosition.y - textHeight)
        while (textContainer != null &&
               textContainer.anchoredPosition.y - textHeight < SCREEN_TOP)
        {
            textContainer.anchoredPosition += Vector2.up * scrollSpeed * Time.unscaledDeltaTime;
            yield return null;
        }

        PlayerPrefs.SetInt(SHOWN_KEY, 1);
        PlayerPrefs.Save();

        ShowIntroScreen();
        gameObject.SetActive(false);
    }

    private void ShowIntroScreen()
    {
        Time.timeScale = 1f;
        if (level1IntroCanvas != null) level1IntroCanvas.SetActive(true);
        if (level1Intro != null)       level1Intro.SetActive(true);
    }

    [ContextMenu("Reset Intro (Testing)")]
    void ResetForTesting()
    {
        PlayerPrefs.DeleteKey(SHOWN_KEY);
        PlayerPrefs.Save();
        Debug.Log("[IntroCrawl] Cleared — crawl will show again on next Play.");
    }
}
