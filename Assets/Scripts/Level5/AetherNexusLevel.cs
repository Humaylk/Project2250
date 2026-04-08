using UnityEngine;
using TMPro;

// Munadir: Level 5 — Aether Nexus boss level extending LevelBase
// Munadir: Owns the boss, laser system, timer, and ability manager
// Munadir: Win = boss defeated. Lose = timer runs out OR player dies.
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

    private static TMP_FontAsset _cachedFont;
    private static TMP_FontAsset GetFont()
    {
        if (_cachedFont == null) _cachedFont = TMP_Settings.defaultFontAsset;
        if (_cachedFont == null) _cachedFont = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF - Fallback");
        return _cachedFont;
    }

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

        // Munadir: Create timer display at top-right of screen
        CreateTimerUI();

        if (battleTimer != null)
            battleTimer.StartTimer(battleDuration);

        if (laserSystem != null)
            laserSystem.StartLasers();

        if (boss != null)
            boss.Initialize();

        // Munadir: Delay objective text so it overrides GameManager's default message
        StartCoroutine(SetObjectiveDelayed());
    }

    private System.Collections.IEnumerator SetObjectiveDelayed()
    {
        yield return new WaitForSeconds(0.5f);
        // Munadir: Clear the default white objective text — intro screen already shows objective/controls
        uiManager?.DisplayObjective("");
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
                    // Munadir: We handle H key ourselves to trigger credits instead
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
                    // Munadir: No gate in scene — H key works from anywhere
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

            // Munadir: Flash red when under 30 seconds
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

            // Munadir: Show win screen hint (walk to gate and press H)
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

    private void CreateTimerUI()
    {
        // Munadir: Check if timer UI already exists
        if (timerText != null) return;

        // Munadir: Find existing canvas or create one
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
}
