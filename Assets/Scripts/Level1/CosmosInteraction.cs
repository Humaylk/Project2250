using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Attach to the Cosmos GameObject in Level 1.
// Uses a direct distance check every frame — no trigger collider required.
public class CosmosInteraction : MonoBehaviour
{
    public float interactDistance = 2.5f;

    [Header("Audio")]
    public AudioClip voiceClip;
    private AudioSource _audioSource;

    // ── Dialogue lines ────────────────────────────────────────────────────────
    private const string DIALOGUE_INTRO =
        "Welcome Alex. First kill the golems by pressing G to attack.";

    private const string DIALOGUE_GOLEMS_DONE =
        "Well done. For the gate to appear, you need to go to each pillar and hold E for 5 seconds to summon the elemental statues.";

    private const string DIALOGUE_ALL_DONE =
        "Congratulations, the gate is now open. Good luck.";

    private float lettersPerSecond = 40f;

    // ── Hover ─────────────────────────────────────────────────────────────────
    public float hoverAmplitude = 0.15f;   // world units up/down
    public float hoverSpeed     = 1.5f;    // cycles per second
    private Vector3 _basePosition;

    // ── State ─────────────────────────────────────────────────────────────────
    private bool panelOpen    = false;
    private bool playerNearby = false;
    private bool isTyping     = false;

    // ── UI ────────────────────────────────────────────────────────────────────
    private GameObject _canvasGO;
    private GameObject _panelGO;
    private GameObject _promptGO;
    private TMP_Text   _dialogueText;
    private Coroutine  _typeCoroutine;

    // ── Level references ──────────────────────────────────────────────────────
    private Transform          _player;
    private CrackedForestLevel _level;
    private SummoningPuzzle    _puzzle;

    // ── Font ──────────────────────────────────────────────────────────────────
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
        _basePosition = transform.position;
        _level  = FindFirstObjectByType<CrackedForestLevel>();
        _puzzle = FindFirstObjectByType<SummoningPuzzle>();

        PlayerController pc = FindFirstObjectByType<PlayerController>();
        if (pc != null) _player = pc.transform;

        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.playOnAwake  = false;
        _audioSource.spatialBlend = 0f;

        BuildUI();
        ShowPanel(false);
        ShowPrompt(false);
    }

    void Update()
    {
        // Hover
        float offsetY = Mathf.Sin(Time.time * hoverSpeed * Mathf.PI * 2f) * hoverAmplitude;
        transform.position = _basePosition + new Vector3(0f, offsetY, 0f);

        // Try to find player if not yet cached
        if (_player == null)
        {
            PlayerController pc = FindFirstObjectByType<PlayerController>();
            if (pc != null) _player = pc.transform;
        }

        if (_player != null)
        {
            float dist = Vector2.Distance(transform.position, _player.position);
            playerNearby = dist <= interactDistance;
        }
        else
        {
            playerNearby = false;
        }

        ShowPrompt(playerNearby && !panelOpen);

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (playerNearby && !panelOpen)
                OpenDialogue();
            else if (panelOpen)
            {
                if (isTyping) SkipTyping();
                else          CloseDialogue();
            }
        }
    }

    // ── Dialogue flow ─────────────────────────────────────────────────────────
    private void OpenDialogue()
    {
        panelOpen = true;
        ShowPanel(true);
        ShowPrompt(false);
        if (voiceClip != null) _audioSource.PlayOneShot(voiceClip, 2f);
        StartTypewriter(CurrentLine());
    }

    private void CloseDialogue()
    {
        if (_typeCoroutine != null) StopCoroutine(_typeCoroutine);
        isTyping  = false;
        panelOpen = false;
        ShowPanel(false);
    }

    private void SkipTyping()
    {
        if (_typeCoroutine != null) StopCoroutine(_typeCoroutine);
        isTyping = false;
        if (_dialogueText != null)
            _dialogueText.maxVisibleCharacters = int.MaxValue;
    }

    private string CurrentLine()
    {
        if (AllStatuesSummoned()) return DIALOGUE_ALL_DONE;
        if (AllGolemsDead())      return DIALOGUE_GOLEMS_DONE;
        return DIALOGUE_INTRO;
    }

    private bool AllGolemsDead()
    {
        if (_level == null || _level.golems == null || _level.golems.Length == 0)
            return false;
        foreach (EnemyHealth g in _level.golems)
            if (g != null && g.gameObject.activeSelf && g.health > 0)
                return false;
        return true;
    }

    private bool AllStatuesSummoned() => _puzzle != null && _puzzle.IsSolved;

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

    // ── Build UI ──────────────────────────────────────────────────────────────
    // The Cosmos object may have a non-1 local scale, so world-space canvases
    // that are children need their localPosition/localScale compensated so they
    // appear at the correct world-space size regardless of the parent's scale.
    private void BuildUI()
    {
        TMP_FontAsset font = GetFont();

        // Inverse of the object's X scale so child canvases appear at world size
        float inv = (Mathf.Abs(transform.localScale.x) > 0.0001f)
            ? 1f / transform.localScale.x : 1f;

        // ── Screen-space dialogue canvas ──────────────────────────────────────
        _canvasGO = new GameObject("CosmosDialogueCanvas");
        Canvas canvas = _canvasGO.AddComponent<Canvas>();
        canvas.renderMode   = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        CanvasScaler scaler = _canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight  = 0.5f;
        _canvasGO.AddComponent<GraphicRaycaster>();

        // Bottom panel
        _panelGO = new GameObject("DialoguePanel");
        _panelGO.transform.SetParent(_canvasGO.transform, false);
        _panelGO.SetActive(false);
        Image panelBg = _panelGO.AddComponent<Image>();
        panelBg.color = new Color(0.05f, 0.02f, 0.08f, 0.93f);
        RectTransform panelRT = _panelGO.GetComponent<RectTransform>();
        panelRT.anchorMin = new Vector2(0f, 0f);
        panelRT.anchorMax = new Vector2(1f, 0f);
        panelRT.pivot     = new Vector2(0.5f, 0f);
        panelRT.offsetMin = new Vector2(0, 0);
        panelRT.offsetMax = new Vector2(0, 230);

        // Orange top border
        GameObject border = new GameObject("TopBorder");
        border.transform.SetParent(_panelGO.transform, false);
        border.AddComponent<Image>().color = new Color(0.9f, 0.35f, 0.05f, 1f);
        RectTransform brt = border.GetComponent<RectTransform>();
        brt.anchorMin = new Vector2(0,1); brt.anchorMax = new Vector2(1,1);
        brt.pivot     = new Vector2(0.5f,1);
        brt.offsetMin = new Vector2(0,-4); brt.offsetMax = new Vector2(0,0);

        // Portrait box
        GameObject portBox = new GameObject("PortraitBox");
        portBox.transform.SetParent(_panelGO.transform, false);
        portBox.AddComponent<Image>().color = new Color(0.10f, 0.03f, 0.15f, 1f);
        RectTransform portRT = portBox.GetComponent<RectTransform>();
        portRT.anchorMin = new Vector2(0,0); portRT.anchorMax = new Vector2(0,1);
        portRT.pivot     = new Vector2(0,0.5f);
        portRT.offsetMin = new Vector2(12,12); portRT.offsetMax = new Vector2(212,-12);

        // Portrait ring
        GameObject portRing = new GameObject("PortraitRing");
        portRing.transform.SetParent(portBox.transform, false);
        portRing.AddComponent<Image>().color = new Color(0.9f, 0.35f, 0.05f, 1f);
        RectTransform ringRT = portRing.GetComponent<RectTransform>();
        ringRT.anchorMin = Vector2.zero; ringRT.anchorMax = Vector2.one;
        ringRT.offsetMin = new Vector2(-4,-4); ringRT.offsetMax = new Vector2(4,4);
        portRing.transform.SetAsFirstSibling();

        // Cosmos sprite inside portrait
        GameObject portImgGO = new GameObject("CosmosSprite");
        portImgGO.transform.SetParent(portBox.transform, false);
        Image portImg = portImgGO.AddComponent<Image>();
        portImg.preserveAspect = true;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && sr.sprite != null) portImg.sprite = sr.sprite;
        else portImg.color = new Color(0.2f, 0.5f, 0.9f, 0.8f);
        RectTransform portImgRT = portImgGO.GetComponent<RectTransform>();
        portImgRT.anchorMin = Vector2.zero; portImgRT.anchorMax = Vector2.one;
        portImgRT.offsetMin = new Vector2(6,6); portImgRT.offsetMax = new Vector2(-6,-6);

        // "COSMOS" name label
        GameObject nameGO = new GameObject("CosmosNameLabel");
        nameGO.transform.SetParent(_panelGO.transform, false);
        TMP_Text nameText = nameGO.AddComponent<TextMeshProUGUI>();
        nameText.text = "COSMOS"; nameText.font = font; nameText.fontSize = 20;
        nameText.fontStyle = FontStyles.Bold;
        nameText.color = new Color(1f, 0.5f, 0.1f, 1f);
        nameText.alignment = TextAlignmentOptions.Center;
        RectTransform nameRT = nameGO.GetComponent<RectTransform>();
        nameRT.anchorMin = new Vector2(0,1); nameRT.anchorMax = new Vector2(0,1);
        nameRT.pivot = new Vector2(0,0);
        nameRT.offsetMin = new Vector2(12,2); nameRT.offsetMax = new Vector2(212,26);

        // Dialogue text
        GameObject textGO = new GameObject("DialogueText");
        textGO.transform.SetParent(_panelGO.transform, false);
        _dialogueText = textGO.AddComponent<TextMeshProUGUI>();
        _dialogueText.font = font; _dialogueText.fontSize = 36;
        _dialogueText.color = new Color(0.95f, 0.92f, 0.85f, 1f);
        _dialogueText.alignment = TextAlignmentOptions.TopLeft;
        _dialogueText.enableWordWrapping = true;
        _dialogueText.maxVisibleCharacters = 0;
        RectTransform textRT = textGO.GetComponent<RectTransform>();
        textRT.anchorMin = new Vector2(0,0); textRT.anchorMax = new Vector2(1,1);
        textRT.offsetMin = new Vector2(230,18); textRT.offsetMax = new Vector2(-18,-18);

        // [ E ] Continue hint
        GameObject hintGO = new GameObject("ContinueHint");
        hintGO.transform.SetParent(_panelGO.transform, false);
        TMP_Text hintText = hintGO.AddComponent<TextMeshProUGUI>();
        hintText.text = "[ E ] Continue"; hintText.font = font; hintText.fontSize = 15;
        hintText.color = new Color(0.65f, 0.65f, 0.65f, 0.8f);
        hintText.alignment = TextAlignmentOptions.BottomRight;
        RectTransform hintRT = hintGO.GetComponent<RectTransform>();
        hintRT.anchorMin = new Vector2(1,0); hintRT.anchorMax = new Vector2(1,0);
        hintRT.pivot = new Vector2(1,0);
        hintRT.offsetMin = new Vector2(-165,6); hintRT.offsetMax = new Vector2(-10,24);

        // ── World-space "Cosmos" name label (always visible above object) ──────
        // localPosition and localScale are divided by the parent scale so they
        // appear at the correct world-space size.
        GameObject nameWorldCanvas = new GameObject("CosmosNameCanvas");
        Canvas nameWC = nameWorldCanvas.AddComponent<Canvas>();
        nameWC.renderMode   = RenderMode.WorldSpace;
        nameWC.sortingOrder = 12;
        nameWorldCanvas.transform.SetParent(transform, false);
        nameWorldCanvas.transform.localPosition = new Vector3(0, 0.8f * inv, 0);
        nameWorldCanvas.transform.localScale    = new Vector3(0.012f * inv, 0.012f * inv, 1f);

        GameObject nameLabelGO = new GameObject("NameLabel");
        nameLabelGO.transform.SetParent(nameWorldCanvas.transform, false);
        TMP_Text nameLabelText = nameLabelGO.AddComponent<TextMeshProUGUI>();
        nameLabelText.text = "Cosmos"; nameLabelText.font = font; nameLabelText.fontSize = 40;
        nameLabelText.fontStyle = FontStyles.Bold;
        nameLabelText.color = new Color(1f, 0.85f, 0.3f, 1f);
        nameLabelText.alignment = TextAlignmentOptions.Center;
        nameLabelText.enableWordWrapping = false;
        nameLabelGO.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 60);

        // ── World-space "[ E ] Talk" prompt ──────────────────────────────────
        _promptGO = new GameObject("TalkPromptCanvas");
        Canvas promptCanvas = _promptGO.AddComponent<Canvas>();
        promptCanvas.renderMode   = RenderMode.WorldSpace;
        promptCanvas.sortingOrder = 10;
        _promptGO.transform.SetParent(transform, false);
        _promptGO.transform.localPosition = new Vector3(0, 1.5f * inv, 0);
        _promptGO.transform.localScale    = new Vector3(0.012f * inv, 0.012f * inv, 1f);

        GameObject promptTextGO = new GameObject("PromptText");
        promptTextGO.transform.SetParent(_promptGO.transform, false);
        TMP_Text promptText = promptTextGO.AddComponent<TextMeshProUGUI>();
        promptText.text = "[ E ] Talk"; promptText.font = font; promptText.fontSize = 34;
        promptText.color = Color.white; promptText.alignment = TextAlignmentOptions.Center;
        promptText.enableWordWrapping = false;
        promptTextGO.GetComponent<RectTransform>().sizeDelta = new Vector2(280, 56);
    }
}
