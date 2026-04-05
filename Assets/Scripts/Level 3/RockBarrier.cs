using UnityEngine;

// Represents the rocks at the ocean floor that block the exit doorway in Level 3.
// Player must hold E for 3 seconds near a mine to defuse it.
// Uses the DialSeconds prefab (GolemiteGames Timer asset) to show defuse progress.
public class RockBarrier : MonoBehaviour, IInteractable
{
    public bool isCleared = false;
    public float interactionRange = 2f;

    private Collider2D barrierCollider;
    private SpriteRenderer spriteRenderer;
    private Transform player;

    private bool isDefusing = false;
    private GameObject dialGO;
    private Timer dialTimer;
    private RectTransform dialRect;

    void Awake()
    {
        barrierCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        PlayerController pc = FindFirstObjectByType<PlayerController>();
        if (pc != null) player = pc.transform;

        // Find the shared defuse dial in the scene (searches inactive objects too)
        foreach (Timer t in Resources.FindObjectsOfTypeAll<Timer>())
        {
            if (t.gameObject.name == "DefuseDial")
            {
                dialGO = t.gameObject;
                dialTimer = t;
                dialRect = dialGO.GetComponent<RectTransform>();
                break;
            }
        }
    }

    void Update()
    {
        if (isCleared || player == null) return;

        bool inRange = Vector2.Distance(transform.position, player.position) <= interactionRange;
        bool holdingE = Input.GetKey(KeyCode.E);

        if (inRange && holdingE && !isDefusing)
        {
            isDefusing = true;

            if (dialGO != null)
            {
                dialGO.SetActive(true);
                dialTimer.onTimerEnd.RemoveAllListeners();
                dialTimer.onTimerEnd.AddListener(Defuse);
                dialTimer.StopTimer();
                dialTimer.StartTimer();
            }

            GameManager.Instance?.uiManager?.ShowHint("Defusing... keep holding E");
        }
        else if (isDefusing && (!inRange || !holdingE))
        {
            CancelDefuse();
        }

        // Keep the dial floating above the mine while defusing
        if (isDefusing && dialRect != null && Camera.main != null)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
            screenPos.y += 100f;
            dialRect.position = screenPos;
        }
    }

    private void CancelDefuse()
    {
        isDefusing = false;
        if (dialGO != null)
        {
            dialGO.SetActive(false);
            dialTimer.StopTimer();
        }
        GameManager.Instance?.uiManager?.ShowHint("[E] DEFUSE THE BOMB (hold)");
    }

    private void Defuse()
    {
        isCleared = true;
        isDefusing = false;

        if (dialGO != null) dialGO.SetActive(false);
        if (barrierCollider != null) barrierCollider.enabled = false;
        if (spriteRenderer != null) spriteRenderer.enabled = false;

        GameManager.Instance?.progressionSystem?.AddCollectionXP(20);
        GameManager.Instance?.uiManager?.ShowHint("Bomb defused! Head for the exit doorway!");
    }

    // Called by InteractionSystem on single E press — hold is required, so no-op
    public void Interact() { }

    public string GetInteractPrompt()
        => isCleared ? "" : "DEFUSE BOMB (hold E)";

    public bool IsCleared() => isCleared;

    public void ResetBarrier()
    {
        isCleared = false;
        isDefusing = false;
        if (dialGO != null)
        {
            dialGO.SetActive(false);
            dialTimer?.StopTimer();
        }
        if (barrierCollider != null) barrierCollider.enabled = true;
        if (spriteRenderer != null) spriteRenderer.enabled = true;
    }
}
