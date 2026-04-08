using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth_Level4 : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    public TextMeshProUGUI healthText;       // "100/100" top-bar text
    public TextMeshProUGUI healthTextAlt;    // "HP:100" bottom text (optional)
    public Image healthBarFill;             // Fill image whose fillAmount we drive
    public DeathScreen deathScreen;

    // ── Gravity timer ──────────────────────────────────────────────────
    private const float TimerDuration = 20f;
    private float _timeLeft = TimerDuration;
    private bool _timerRunning = false;
    private bool _isDead = false;

    private TMP_Text _timerLabel;
    private static readonly Color TimerNormal  = new Color(0f,  0.87f, 1f,  1f);  // cyan  (same as Level 3 oxygen)
    private static readonly Color TimerWarning = new Color(1f,  0.2f,  0.2f, 1f); // red

    private Animator _animator;

    // ── Font helper ────────────────────────────────────────────────────
    static TMP_FontAsset LoadFont()
    {
        TMP_FontAsset f = Resources.Load<TMP_FontAsset>("Fonts & Materials/ThaleahFat_TTF SDF");
        if (f != null) return f;
        foreach (var t in FindObjectsByType<TMP_Text>(FindObjectsSortMode.None))
            if (t.font != null && t.font.name.Contains("Thaleah")) return t.font;
        return null;
    }

    void Start()
    {
        currentHealth = maxHealth;
        _animator = GetComponentInChildren<Animator>();

        if (deathScreen == null)
            deathScreen = FindFirstObjectByType<DeathScreen>();

        BuildTimerHUD();
        UpdateUI();

        _timerRunning = true;
    }

    void Update()
    {
        if (_isDead || !_timerRunning) return;

        _timeLeft -= Time.deltaTime;
        _timeLeft = Mathf.Max(0f, _timeLeft);
        UpdateTimerHUD();

        if (_timeLeft <= 0f)
        {
            _timerRunning = false;
            Die();
        }
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);
        UpdateUI();

        DamageFlashCanvas.Instance?.Flash();

        if (_animator != null)
            _animator.SetTrigger("Hurt");

        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        if (_isDead) return;
        _isDead = true;
        _timerRunning = false;

        currentHealth = 0;
        UpdateUI();

        if (_animator != null)
        {
            _animator.SetBool("noBlood", false);
            _animator.SetTrigger("Death");
        }

        if (deathScreen != null)
            deathScreen.Show();
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
            );
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.name.StartsWith("Square"))
            Die();
    }

    void UpdateUI()
    {
        if (healthText != null)
            healthText.text = currentHealth + "/" + maxHealth;

        if (healthTextAlt != null)
            healthTextAlt.text = "HP:" + currentHealth;

        if (healthBarFill != null)
            healthBarFill.fillAmount = (float)currentHealth / maxHealth;
    }

    // ── Timer HUD ──────────────────────────────────────────────────────
    void BuildTimerHUD()
    {
        // Overlay canvas — sorts above the existing HUD canvas (order 50)
        GameObject canvasGO = new GameObject("Level4TimerCanvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode   = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 51;
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight  = 0.5f;
        canvasGO.AddComponent<GraphicRaycaster>();

        // Text anchored to top-left, below the health bar
        GameObject textGO = new GameObject("TimerText", typeof(RectTransform));
        textGO.transform.SetParent(canvasGO.transform, false);

        TMP_Text t = textGO.AddComponent<TextMeshProUGUI>();
        t.text               = "Timer: " +Mathf.CeilToInt(_timeLeft) + "s";
        t.color              = TimerNormal;
        t.fontSize           = 72f;
        t.fontStyle          = FontStyles.Bold;
        t.alignment          = TextAlignmentOptions.Left;
        t.enableWordWrapping = false;

        TMP_FontAsset font = LoadFont();
        if (font != null) t.font = font;

        // Position: top-left, offset down so it sits just below the health bar
        RectTransform rt = textGO.GetComponent<RectTransform>();
        rt.anchorMin        = new Vector2(0f, 1f);
        rt.anchorMax        = new Vector2(0f, 1f);
        rt.pivot            = new Vector2(0f, 1f);
        rt.anchoredPosition = new Vector2(20f, -75f);
        rt.sizeDelta        = new Vector2(550f, 85f);

        _timerLabel = t;
    }

    void UpdateTimerHUD()
    {
        if (_timerLabel == null) return;

        int secs = Mathf.CeilToInt(_timeLeft);
        _timerLabel.text = "Timer: " +secs + "s";

        if (_timeLeft <= 10f)
        {
            // Flash red below 10 s (same as Level 3 oxygen)
            float alpha = Mathf.PingPong(Time.time * 3f, 1f);
            _timerLabel.color = new Color(TimerWarning.r, TimerWarning.g, TimerWarning.b, alpha);
        }
        else
        {
            _timerLabel.color = TimerNormal;
        }
    }
}
