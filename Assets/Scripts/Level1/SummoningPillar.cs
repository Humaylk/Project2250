using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attach to each pillar. Assign the matching statue GameObject (hidden at start).
/// Player holds E for 3 seconds near the pillar to summon the statue.
/// </summary>
public class SummoningPillar : MonoBehaviour
{
    [Header("Identity")]
    public string pillarName = "Fire Statue";

    [Header("Statue")]
    [Tooltip("The fire/water/air statue GameObject — will be hidden at start and revealed on summon")]
    public GameObject statue;

    [Header("Summoning")]
    public float holdDuration = 5f;

    // Events
    public System.Action<SummoningPillar> OnSummoned;

    // State
    public bool IsSummoned { get; private set; } = false;
    private bool _playerNearby = false;
    private float _holdTimer = 0f;
    private bool _isSummoning = false;

    // Cached statue renderer for fade-in
    private SpriteRenderer _statueRenderer;

    // Progress bar UI (built at runtime)
    private GameObject _progressBarRoot;
    private Image _progressFill;

    void Start()
    {
        // Cache the statue's SpriteRenderer and start fully transparent
        if (statue != null)
        {
            _statueRenderer = statue.GetComponent<SpriteRenderer>();
            statue.SetActive(true); // keep active so we can fade it in
            if (_statueRenderer != null)
            {
                Color c = _statueRenderer.color;
                c.a = 0f;
                _statueRenderer.color = c;
            }
        }

        BuildProgressBar();
    }

    void Update()
    {
        if (!_playerNearby || IsSummoned) return;

        if (Input.GetKey(KeyCode.E))
        {
            _isSummoning = true;
            _holdTimer += Time.deltaTime;
            float t = Mathf.Clamp01(_holdTimer / holdDuration);
            ShowProgress(t);
            FadeStatue(t); // statue appears gradually as player holds E

            if (_holdTimer >= holdDuration)
                CompleteSummon();
        }
        else
        {
            if (_isSummoning)
            {
                // Released early — reset timer and fade statue back out
                _holdTimer = 0f;
                _isSummoning = false;
                ShowProgress(0f);
                FadeStatue(0f);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        _playerNearby = true;

        if (!IsSummoned)
        {
            SetProgressBarVisible(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        _playerNearby = false;
        _holdTimer = 0f;
        _isSummoning = false;
        ShowProgress(0f);
        SetProgressBarVisible(false);
    }

    void CompleteSummon()
    {
        IsSummoned = true;
        _holdTimer = 0f;
        _isSummoning = false;
        ShowProgress(1f);
        SetProgressBarVisible(false);
        FadeStatue(1f); // fully visible

        OnSummoned?.Invoke(this);
    }

    void FadeStatue(float alpha)
    {
        if (_statueRenderer == null) return;
        Color c = _statueRenderer.color;
        c.a = alpha;
        _statueRenderer.color = c;
    }

    // ── Progress bar (world-space UI above the pillar) ────────────────────
    void BuildProgressBar()
    {
        _progressBarRoot = new GameObject("SummonProgress_" + pillarName);
        _progressBarRoot.transform.SetParent(transform);
        _progressBarRoot.transform.localPosition = new Vector3(0f, 1.5f, 0f);
        _progressBarRoot.transform.localScale    = new Vector3(0.01f, 0.01f, 1f);

        Canvas canvas = _progressBarRoot.AddComponent<Canvas>();
        canvas.renderMode    = RenderMode.WorldSpace;
        canvas.sortingOrder  = 2;
        RectTransform rootRT = _progressBarRoot.GetComponent<RectTransform>();
        rootRT.sizeDelta = new Vector2(120f, 18f);

        // Background
        GameObject bg = new GameObject("BG");
        bg.transform.SetParent(_progressBarRoot.transform, false);
        Image bgImg = bg.AddComponent<Image>();
        bgImg.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);
        RectTransform bgRT = bg.GetComponent<RectTransform>();
        bgRT.anchorMin = Vector2.zero;
        bgRT.anchorMax = Vector2.one;
        bgRT.offsetMin = Vector2.zero;
        bgRT.offsetMax = Vector2.zero;

        // Fill
        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(_progressBarRoot.transform, false);
        _progressFill = fill.AddComponent<Image>();
        _progressFill.color = new Color(0.9f, 0.7f, 0.1f, 1f); // golden glow
        RectTransform fillRT = fill.GetComponent<RectTransform>();
        fillRT.anchorMin = Vector2.zero;
        fillRT.anchorMax = new Vector2(0f, 1f);
        fillRT.offsetMin = Vector2.zero;
        fillRT.offsetMax = Vector2.zero;
        fillRT.pivot     = new Vector2(0f, 0.5f);
        fillRT.sizeDelta = new Vector2(0f, 0f);

        SetProgressBarVisible(false);
    }

    void ShowProgress(float t)
    {
        if (_progressFill == null) return;
        RectTransform rt = _progressFill.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(120f * Mathf.Clamp01(t), 0f);
    }

    void SetProgressBarVisible(bool visible)
    {
        if (_progressBarRoot != null)
            _progressBarRoot.SetActive(visible);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 1.5f);
    }
}
