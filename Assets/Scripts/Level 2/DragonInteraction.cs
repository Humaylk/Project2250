using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DragonInteraction : MonoBehaviour
{
    [HideInInspector] public bool hasAllItems  = false;
    [HideInInspector] public Gate gateToReveal = null;

    // ── Dialogue lines ────────────────────────────────────────────────────────
    // Quest dialogue is split into two pages — press E to go to the next one
    private string[] questDialoguePages = new string[]
    {
        "Brave traveller... My power has been stripped from me and the gate remains missing. " +
        "My armor is lost and scattered everywhere in this temple. " +
        "Seek out the ancient sword, same goes for the armor pieces. " +
        "I need the ancient power of the sword and the armor, come back to me and only then can I make the gate appear to clear your path.",

        "However be aware, the fire that powers this temple, that runs in my blood " +
        "is the same fire that will harm you. Be careful and good luck."
    };

    private string completeDialogue =
        "Great work, the gate has now appeared. I wish you luck passing through to the next level.";

    private string alreadyDoneDialogue =
        "The gate is open. Go, brave one. Your destiny awaits beyond.";

    // ── Typewriter speed ──────────────────────────────────────────────────────
    private float lettersPerSecond = 40f;

    // ── State ─────────────────────────────────────────────────────────────────
    private bool panelOpen         = false;
    private bool questDone         = false;
    private bool playerNearby      = false;
    private bool isTyping          = false;
    private int  _pageIndex        = 0;
    private bool _multiPage        = false;
    private bool _pendingGateReveal = false; // reveal gate when this dialogue closes

    // ── Built UI references ───────────────────────────────────────────────────
    private GameObject _canvasGO;
    private GameObject _panelGO;
    private GameObject _promptGO;
    private TMP_Text   _dialogueText;
    private Coroutine  _typeCoroutine;

    // ── Font cache ────────────────────────────────────────────────────────────
    private static TMP_FontAsset _font;
    private static TMP_FontAsset GetFont()
    {
        if (_font != null) return _font;
        foreach (var t in FindObjectsByType<TMP_Text>(FindObjectsSortMode.None))
            if (t.font != null) { _font = t.font; return _font; }
        _font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF - Fallback");
        return _font;
    }

    // ─────────────────────────────────────────────────────────────────────────
    void Start()
    {
        BuildUI();
        ShowPanel(false);
        ShowPrompt(false);
    }

    void Update()
    {
        ShowPrompt(playerNearby && !panelOpen);

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (playerNearby && !panelOpen)
                OpenDialogue();
            else if (panelOpen)
            {
                if (isTyping)
                    SkipTyping();                  // skip to full text on current page
                else if (_multiPage && _pageIndex < questDialoguePages.Length - 1)
                    AdvancePage();                 // go to next page
                else
                    CloseDialogue();               // last page or single-line — close
            }
        }
    }

    // ── Trigger detection ─────────────────────────────────────────────────────
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsPlayer(other)) playerNearby = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!IsPlayer(other)) return;
        playerNearby = false;
        if (panelOpen) CloseDialogue();
    }

    private bool IsPlayer(Collider2D col)
    {
        return col.CompareTag("Player")
            || col.GetComponent<HeroKnight>()               != null
            || col.GetComponentInParent<HeroKnight>()       != null
            || col.GetComponent<PlayerController>()         != null
            || col.GetComponentInParent<PlayerController>() != null;
    }

    // ── Dialogue flow ─────────────────────────────────────────────────────────
    private void OpenDialogue()
    {
        panelOpen  = true;
        _pageIndex = 0;
        ShowPanel(true);
        ShowPrompt(false);

        if (questDone)
        {
            _multiPage = false;
            StartTypewriter(alreadyDoneDialogue);
        }
        else if (hasAllItems)
        {
            _multiPage          = false;
            questDone           = true;
            _pendingGateReveal  = true; // gate appears when player closes this dialogue
            StartTypewriter(completeDialogue);
        }
        else
        {
            _multiPage = true;
            StartTypewriter(questDialoguePages[_pageIndex]);
        }
    }

    private void AdvancePage()
    {
        _pageIndex++;
        StartTypewriter(questDialoguePages[_pageIndex]);
    }

    private void CloseDialogue()
    {
        if (_typeCoroutine != null) StopCoroutine(_typeCoroutine);
        isTyping   = false;
        panelOpen  = false;
        _multiPage = false;
        ShowPanel(false);

        // Reveal the gate after the player has heard the dragon's completion dialogue
        if (_pendingGateReveal)
        {
            _pendingGateReveal = false;
            if (gateToReveal != null)
            {
                gateToReveal.ShowAndOpenGate();
                Debug.Log("[DragonInteraction] Gate revealed!");
            }
        }
    }

    private void SkipTyping()
    {
        if (_typeCoroutine != null) StopCoroutine(_typeCoroutine);
        isTyping = false;
        if (_dialogueText != null)
            _dialogueText.maxVisibleCharacters = int.MaxValue;
    }

    // ── Typewriter ────────────────────────────────────────────────────────────
    private void StartTypewriter(string line)
    {
        if (_typeCoroutine != null) StopCoroutine(_typeCoroutine);
        _typeCoroutine = StartCoroutine(TypewriterRoutine(line));
    }

    private IEnumerator TypewriterRoutine(string line)
    {
        isTyping = true;
        _dialogueText.text = line;
        _dialogueText.maxVisibleCharacters = 0;

        float delay = 1f / lettersPerSecond;
        for (int i = 0; i <= line.Length; i++)
        {
            _dialogueText.maxVisibleCharacters = i;
            yield return new WaitForSeconds(delay);
        }
        isTyping = false;
    }

    // ── UI helpers ────────────────────────────────────────────────────────────
    private void ShowPanel(bool show)  { if (_panelGO  != null) _panelGO.SetActive(show);  }
    private void ShowPrompt(bool show) { if (_promptGO != null) _promptGO.SetActive(show); }

    // ── Build entire UI in code ───────────────────────────────────────────────
    private void BuildUI()
    {
        TMP_FontAsset font = GetFont();

        // ── Canvas ────────────────────────────────────────────────────────────
        _canvasGO = new GameObject("DragonDialogueCanvas");
        Canvas canvas = _canvasGO.AddComponent<Canvas>();
        canvas.renderMode   = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        CanvasScaler scaler = _canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight  = 0.5f;
        _canvasGO.AddComponent<GraphicRaycaster>();

        // ── Bottom dialogue panel ─────────────────────────────────────────────
        _panelGO = new GameObject("DialoguePanel");
        _panelGO.transform.SetParent(_canvasGO.transform, false);
        _panelGO.SetActive(false); // hidden until player talks to dragon
        Image panelBg  = _panelGO.AddComponent<Image>();
        panelBg.color  = new Color(0.05f, 0.02f, 0.08f, 0.93f);
        RectTransform panelRT = _panelGO.GetComponent<RectTransform>();
        panelRT.anchorMin = new Vector2(0f, 0f);
        panelRT.anchorMax = new Vector2(1f, 0f);
        panelRT.pivot     = new Vector2(0.5f, 0f);
        panelRT.offsetMin = new Vector2(0, 0);
        panelRT.offsetMax = new Vector2(0, 230);

        // Orange border strip along the top of the panel
        GameObject border   = new GameObject("TopBorder");
        border.transform.SetParent(_panelGO.transform, false);
        Image borderImg     = border.AddComponent<Image>();
        borderImg.color     = new Color(0.9f, 0.35f, 0.05f, 1f);
        RectTransform brt   = border.GetComponent<RectTransform>();
        brt.anchorMin = new Vector2(0, 1); brt.anchorMax = new Vector2(1, 1);
        brt.pivot     = new Vector2(0.5f, 1);
        brt.offsetMin = new Vector2(0, -4); brt.offsetMax = new Vector2(0, 0);

        // ── Portrait area (bottom-left) ───────────────────────────────────────
        GameObject portBox     = new GameObject("PortraitBox");
        portBox.transform.SetParent(_panelGO.transform, false);
        Image portBg           = portBox.AddComponent<Image>();
        portBg.color           = new Color(0.10f, 0.03f, 0.15f, 1f);
        RectTransform portRT   = portBox.GetComponent<RectTransform>();
        portRT.anchorMin = new Vector2(0, 0); portRT.anchorMax = new Vector2(0, 1);
        portRT.pivot     = new Vector2(0, 0.5f);
        portRT.offsetMin = new Vector2(12, 12); portRT.offsetMax = new Vector2(212, -12);

        // Portrait border ring
        GameObject portRing    = new GameObject("PortraitRing");
        portRing.transform.SetParent(portBox.transform, false);
        Image ringImg          = portRing.AddComponent<Image>();
        ringImg.color          = new Color(0.9f, 0.35f, 0.05f, 1f);
        RectTransform ringRT   = portRing.GetComponent<RectTransform>();
        ringRT.anchorMin = Vector2.zero; ringRT.anchorMax = Vector2.one;
        ringRT.offsetMin = new Vector2(-4, -4); ringRT.offsetMax = new Vector2(4, 4);
        portRing.transform.SetAsFirstSibling();

        // Dragon sprite inside portrait
        GameObject portImgGO   = new GameObject("DragonSprite");
        portImgGO.transform.SetParent(portBox.transform, false);
        Image portImg          = portImgGO.AddComponent<Image>();
        portImg.preserveAspect = true;
        SpriteRenderer sr      = GetComponent<SpriteRenderer>();
        if (sr != null && sr.sprite != null)
            portImg.sprite = sr.sprite;
        else
            portImg.color = new Color(0.55f, 0.08f, 0.05f, 0.7f);
        RectTransform portImgRT = portImgGO.GetComponent<RectTransform>();
        portImgRT.anchorMin = Vector2.zero; portImgRT.anchorMax = Vector2.one;
        portImgRT.offsetMin = new Vector2(6, 6); portImgRT.offsetMax = new Vector2(-6, -6);

        // ── Dragon name label (above portrait) ───────────────────────────────
        GameObject nameGO    = new GameObject("DragonNameLabel");
        nameGO.transform.SetParent(_panelGO.transform, false);
        TMP_Text nameText    = nameGO.AddComponent<TextMeshProUGUI>();
        nameText.text        = "DRAGON";
        nameText.font        = font;
        nameText.fontSize    = 20;
        nameText.fontStyle   = FontStyles.Bold;
        nameText.color       = new Color(1f, 0.5f, 0.1f, 1f);
        nameText.alignment   = TextAlignmentOptions.Center;
        RectTransform nameRT = nameGO.GetComponent<RectTransform>();
        nameRT.anchorMin = new Vector2(0, 1); nameRT.anchorMax = new Vector2(0, 1);
        nameRT.pivot     = new Vector2(0, 0);
        nameRT.offsetMin = new Vector2(12, 2); nameRT.offsetMax = new Vector2(212, 26);

        // ── Dialogue text box (right of portrait) ────────────────────────────
        GameObject textGO      = new GameObject("DialogueText");
        textGO.transform.SetParent(_panelGO.transform, false);
        _dialogueText          = textGO.AddComponent<TextMeshProUGUI>();
        _dialogueText.font     = font;
        _dialogueText.fontSize = 36;
        _dialogueText.color    = new Color(0.95f, 0.92f, 0.85f, 1f);
        _dialogueText.alignment          = TextAlignmentOptions.TopLeft;
        _dialogueText.enableWordWrapping = true;
        _dialogueText.maxVisibleCharacters = 0;
        RectTransform textRT   = textGO.GetComponent<RectTransform>();
        textRT.anchorMin = new Vector2(0, 0); textRT.anchorMax = new Vector2(1, 1);
        textRT.offsetMin = new Vector2(230, 18);  // starts right of portrait
        textRT.offsetMax = new Vector2(-18, -18);

        // ── "Press E" hint (bottom-right of panel) ───────────────────────────
        GameObject hintGO    = new GameObject("ContinueHint");
        hintGO.transform.SetParent(_panelGO.transform, false);
        TMP_Text hintText    = hintGO.AddComponent<TextMeshProUGUI>();
        hintText.text        = "[ E ] Continue";
        hintText.font        = font;
        hintText.fontSize    = 15;
        hintText.color       = new Color(0.65f, 0.65f, 0.65f, 0.8f);
        hintText.alignment   = TextAlignmentOptions.BottomRight;
        RectTransform hintRT = hintGO.GetComponent<RectTransform>();
        hintRT.anchorMin = new Vector2(1, 0); hintRT.anchorMax = new Vector2(1, 0);
        hintRT.pivot     = new Vector2(1, 0);
        hintRT.offsetMin = new Vector2(-165, 6); hintRT.offsetMax = new Vector2(-10, 24);

        // Scale compensation — child world-space canvases need inv of parent scale
        float inv = (Mathf.Abs(transform.localScale.x) > 0.0001f)
            ? 1f / transform.localScale.x : 1f;

        // ── World-space "Dragon" name label (always visible) ──────────────────
        GameObject nameWorldCanvas = new GameObject("DragonNameCanvas");
        Canvas nameWC = nameWorldCanvas.AddComponent<Canvas>();
        nameWC.renderMode   = RenderMode.WorldSpace;
        nameWC.sortingOrder = 12;
        nameWorldCanvas.transform.SetParent(transform, false);
        nameWorldCanvas.transform.localPosition = new Vector3(0, -2.2f * inv, 0);
        nameWorldCanvas.transform.localScale    = new Vector3(0.012f * inv, 0.012f * inv, 1f);

        GameObject nameLabelGO = new GameObject("NameLabel");
        nameLabelGO.transform.SetParent(nameWorldCanvas.transform, false);
        TMP_Text nameLabelText = nameLabelGO.AddComponent<TextMeshProUGUI>();
        nameLabelText.text = "Master Dragon";
        nameLabelText.font = font;
        nameLabelText.fontSize = 40;
        nameLabelText.fontStyle = FontStyles.Bold;
        nameLabelText.color = new Color(1f, 0.55f, 0.1f, 1f);
        nameLabelText.alignment = TextAlignmentOptions.Center;
        nameLabelText.enableWordWrapping = false;
        nameLabelGO.GetComponent<RectTransform>().sizeDelta = new Vector2(400, 60);

        // ── World-space "[ E ] Talk" prompt above dragon ──────────────────────
        _promptGO = new GameObject("TalkPromptCanvas");
        Canvas promptCanvas   = _promptGO.AddComponent<Canvas>();
        promptCanvas.renderMode   = RenderMode.WorldSpace;
        promptCanvas.sortingOrder = 10;
        _promptGO.transform.SetParent(transform, false);
        _promptGO.transform.localPosition = new Vector3(0, -2.9f * inv, 0);
        _promptGO.transform.localScale    = new Vector3(0.012f * inv, 0.012f * inv, 1f);

        GameObject promptTextGO    = new GameObject("PromptText");
        promptTextGO.transform.SetParent(_promptGO.transform, false);
        TMP_Text promptText        = promptTextGO.AddComponent<TextMeshProUGUI>();
        promptText.text            = "[ E ] Talk";
        promptText.font            = font;
        promptText.fontSize        = 34;
        promptText.color           = Color.white;
        promptText.alignment       = TextAlignmentOptions.Center;
        promptText.enableWordWrapping = false;
        RectTransform promptRT     = promptTextGO.GetComponent<RectTransform>();
        promptRT.sizeDelta         = new Vector2(280, 56);
    }
}
