using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Munadir: Level 5 — Aether Nexus boss level extending LevelBase
// Munadir: Owns the boss, laser system, timer, and ability manager
// Munadir: Auto-wires Neelash's HealthBar and OxygenText for consistent visuals
// Munadir: After boss dies, opens gate and H key triggers Star Wars credits
public class AetherNexusLevel : LevelBase
{
    [Header("Level 5 References")]
    public ElementalBoss boss;
    public LaserSystem laserSystem;
    public BattleTimer battleTimer;
    public AbilityManager abilityManager;
    public UIManager uiManager;

    [Header("Settings")]
    public float battleDuration = 180f; // 3 minutes

    private PlayerController player;
    private PlayerHealth playerHealth;
    private TMP_Text timerText;
    private bool hasShownWin = false;
    private bool gateOpened = false;
    private bool creditsTriggered = false;
    private Gate exitGate;

    void Awake()
    {
        player = FindFirstObjectByType<PlayerController>();
        playerHealth = FindFirstObjectByType<PlayerHealth>();
    }

    public override void InitializeLevel()
    {
        isActive = true;
        isComplete = false;
        hasShownWin = false;
        gateOpened = false;
        creditsTriggered = false;
        Debug.Log("=== Aether Nexus - Level 5 Initialized ===");

        if (player != null)
            player.transform.position = new Vector3(-5f, 0f, 0f);

        // Munadir: Auto-wire Neelash's HealthBar to PlayerHealth
        WireHealthBar();

        // Munadir: Use existing OxygenText for timer display (same style as other levels)
        WireTimerToOxygenText();

        // Munadir: Hide old UIManager HPText since we use the visual HealthBar now
        HideOldHPText();

        if (battleTimer != null)
            battleTimer.StartTimer(battleDuration);

        if (laserSystem != null)
            laserSystem.StartLasers();

        if (boss != null)
            boss.Initialize();

        // Munadir: Clear default objective text — intro screen already shows objective/controls
        StartCoroutine(SetObjectiveDelayed());
    }

    private System.Collections.IEnumerator SetObjectiveDelayed()
    {
        yield return new WaitForSeconds(0.5f);
        // Munadir: Clear the default white objective text — intro screen already shows objective/controls
        uiManager?.DisplayObjective("");
    }

    // Munadir: Find Neelash's HealthBar in Canvas and wire it to PlayerHealth
    private void WireHealthBar()
    {
        if (playerHealth == null) return;

        // Munadir: Find the HealthBar's Fill image and HP Text
        GameObject healthBar = GameObject.Find("HealthBar");
        if (healthBar != null)
        {
            // Munadir: Find Fill image for the bar
            Transform fillTransform = healthBar.transform.Find("Fill");
            if (fillTransform != null)
            {
                Image fillImage = fillTransform.GetComponent<Image>();
                if (fillImage != null)
                    playerHealth.healthBarFill = fillImage;
            }

            // Munadir: Find HP Text for the numbers
            Transform hpTextTransform = healthBar.transform.Find("HP Text");
            if (hpTextTransform != null)
            {
                TMP_Text hpText = hpTextTransform.GetComponent<TMP_Text>();
                if (hpText != null)
                    playerHealth.healthText = hpText;
            }

            Debug.Log("HealthBar wired to PlayerHealth successfully.");
        }
    }

    // Munadir: Use the existing OxygenText for the battle timer (same font/style as other levels)
    private void WireTimerToOxygenText()
    {
        GameObject oxygenGO = GameObject.Find("OxygenText");
        if (oxygenGO != null)
        {
            timerText = oxygenGO.GetComponent<TMP_Text>();
            if (timerText != null)
            {
                timerText.text = "TIME: 03:00";
                timerText.gameObject.SetActive(true);
                Debug.Log("Timer wired to OxygenText.");
            }
        }
        else
        {
            // Munadir: Fallback — create timer UI if OxygenText not found
            CreateTimerUI();
        }
    }

    // Munadir: Hide the old red "HP:150" text from UIManager
    private void HideOldHPText()
    {
        GameObject oldHP = GameObject.Find("HPText");
        if (oldHP != null)
            oldHP.SetActive(false);
    }

    // Munadir: Override Update to keep gate detection running after boss defeat
    // Munadir: LevelBase.Update stops calling UpdateLevel once isComplete = true
    protected override void Update()
    {
        base.Update();

        // Munadir: Post-completion gate detection (runs even after isComplete = true)
        if (hasShownWin && !creditsTriggered)
        {
            // Munadir: Open gate once after boss dies
            if (!gateOpened)
            {
                gateOpened = true;
                if (battleTimer != null) battleTimer.StopTimer();
                if (laserSystem != null) laserSystem.StopLasers();
                if (timerText != null) timerText.gameObject.SetActive(false);

                exitGate = FindFirstObjectByType<Gate>();
                if (exitGate != null)
                {
                    exitGate.OpenGate();
                    // Munadir: Disable Gate script so it doesn't call AdvanceLevel on H key
                    exitGate.enabled = false;
                }
            }

            // Munadir: H key near gate triggers credits (or anywhere if no gate in scene)
            if (Input.GetKeyDown(KeyCode.H))
            {
                bool canTrigger = false;

                if (exitGate != null && player != null)
                {
                    float dist = Vector2.Distance(player.transform.position, exitGate.transform.position);
                    canTrigger = dist <= 3f;
                }
                else
                {
                    canTrigger = true;
                }

                if (canTrigger)
                {
                    creditsTriggered = true;
                    Level5WinScreen winScreen = FindFirstObjectByType<Level5WinScreen>();
                    if (winScreen != null)
                        winScreen.StartCreditsScroll();
                }
            }
        }
    }

    public override void UpdateLevel()
    {
        // Munadir: Update timer display every frame
        if (battleTimer != null && timerText != null)
        {
            timerText.text = "TIME: " + battleTimer.GetFormattedTime();

            if (battleTimer.timeRemaining <= 30f)
                timerText.color = new Color(1f, 0.2f, 0.2f, 1f);
            else
                timerText.color = Color.white;
        }
    }

    public override bool CheckWinCondition()
    {
        return boss != null && boss.IsDefeated();
    }

    public override bool CheckLoseCondition()
    {
        if (battleTimer != null && battleTimer.IsTimeUp())
            return true;
        if (playerHealth != null && playerHealth.health <= 0)
            return true;
        return false;
    }

    public override void FinishLevel()
    {
        if (CheckWinCondition() && !hasShownWin)
        {
            hasShownWin = true;
            Debug.Log("BOSS DEFEATED - GAME COMPLETE!");
            GameManager.Instance?.progressionSystem?.AddCombatXP(100);
            GameManager.Instance?.progressionSystem?.GrantReward("Elemental Armor");

            Level5WinScreen winScreen = FindFirstObjectByType<Level5WinScreen>();
            if (winScreen != null)
                winScreen.ShowWinScreen();
        }
        else if (!CheckWinCondition())
        {
            isActive = false;
            if (laserSystem != null)
                laserSystem.StopLasers();

            Debug.Log("Level 5 failed.");
            if (battleTimer != null && battleTimer.IsTimeUp())
            {
                uiManager?.DisplayObjective("TIME'S UP! The Dragon wins...");
                if (playerHealth != null && playerHealth.health > 0)
                    playerHealth.TakeDamage(9999);
            }
        }
    }

    // Munadir: Fallback timer UI if OxygenText not in scene
    private void CreateTimerUI()
    {
        if (timerText != null) return;

        Canvas existingCanvas = FindFirstObjectByType<Canvas>();
        Transform canvasParent = existingCanvas != null ? existingCanvas.transform : null;

        if (canvasParent == null)
        {
            GameObject canvasGO = new GameObject("TimerCanvas");
            Canvas c = canvasGO.AddComponent<Canvas>();
            c.renderMode = RenderMode.ScreenSpaceOverlay;
            c.sortingOrder = 60;
            canvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasParent = canvasGO.transform;
        }

        GameObject timerGO = new GameObject("BattleTimerText", typeof(RectTransform));
        timerGO.transform.SetParent(canvasParent, false);
        timerText = timerGO.AddComponent<TextMeshProUGUI>();
        timerText.font = GetFont();
        timerText.text = "TIME: 03:00";
        timerText.color = Color.white;
        timerText.fontSize = 36;
        timerText.fontStyle = TMPro.FontStyles.Bold;
        timerText.alignment = TextAlignmentOptions.TopRight;

        RectTransform rt = timerGO.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.75f, 0.88f);
        rt.anchorMax = new Vector2(0.98f, 0.97f);
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    // Munadir: Font loader — tries ThaleahFat first, falls back to default
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
        return _cachedFont;
    }
}
