using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkyTriangle : MonoBehaviour
{
    public float rotateSpeed = 150f;
    public bool solved = false;

    public float interactDistance = 2f;

    public Transform player;
    public SkyGameManager manager;

    // World-space label above this triangle showing global X/3 progress
    private TMP_Text _progressLabel;

    void Start()
    {
        if (player == null)
        {
            SkyPlayerController spc = FindFirstObjectByType<SkyPlayerController>();
            if (spc != null) player = spc.transform;
        }

        if (manager == null)
            manager = FindFirstObjectByType<SkyGameManager>();

        BuildProgressLabel();
    }

    void Update()
    {
        if (solved) return;

        // Spin continuously until solved
        transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);

        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);
        if (dist < interactDistance && Input.GetKeyDown(KeyCode.E))
        {
            solved = true;
            manager?.AddProgress();
        }
    }

    // Called by SkyGameManager whenever progress changes
    public void UpdateProgressLabel(int current, int total)
    {
        if (_progressLabel != null)
            _progressLabel.text = current + "/" + total;
    }

    // ── Build a world-space canvas above this triangle ────────────────
    void BuildProgressLabel()
    {
        // Triangle scale is 0.1 so we need inv=10 compensation on the child canvas
        float inv = 1f / transform.localScale.x; // 10

        GameObject canvasGO = new GameObject("TriangleProgressCanvas");
        canvasGO.transform.SetParent(transform, false);

        Canvas c = canvasGO.AddComponent<Canvas>();
        c.renderMode   = RenderMode.WorldSpace;
        c.sortingOrder = 10;

        CanvasScaler cs = canvasGO.AddComponent<CanvasScaler>();
        cs.dynamicPixelsPerUnit = 100f;

        canvasGO.AddComponent<GraphicRaycaster>();

        // Position the canvas above the triangle centre in local space
        RectTransform canvasRT = canvasGO.GetComponent<RectTransform>();
        canvasRT.sizeDelta  = new Vector2(200f, 60f);
        canvasRT.localScale = new Vector3(inv * 0.01f, inv * 0.01f, 1f);
        canvasRT.localPosition = new Vector3(0f, inv * 0.6f, 0f);

        // Text object
        GameObject textGO = new GameObject("ProgressText", typeof(RectTransform));
        textGO.transform.SetParent(canvasGO.transform, false);
        TMP_Text t = textGO.AddComponent<TextMeshProUGUI>();

        int cur = manager != null ? manager.GetCurrentProgress() : 0;
        int tot = manager != null ? manager.total : 3;
        t.text               = cur + "/" + tot;
        t.color              = new Color(0.15f, 0.8f, 1f, 1f); // electric cyan
        t.fontSize           = 52f;
        t.fontStyle          = FontStyles.Bold;
        t.alignment          = TextAlignmentOptions.Center;
        t.enableWordWrapping = false;

        TMP_FontAsset font = LoadFont();
        if (font != null) t.font = font;

        RectTransform rt = textGO.GetComponent<RectTransform>();
        rt.anchorMin  = Vector2.zero;
        rt.anchorMax  = Vector2.one;
        rt.offsetMin  = Vector2.zero;
        rt.offsetMax  = Vector2.zero;

        _progressLabel = t;
    }

    TMP_FontAsset LoadFont()
    {
        TMP_FontAsset f = Resources.Load<TMP_FontAsset>("Fonts & Materials/ThaleahFat_TTF SDF");
        if (f != null) return f;
        foreach (var t in FindObjectsByType<TMP_Text>(FindObjectsSortMode.None))
            if (t.font != null && t.font.name.Contains("Thaleah")) return t.font;
        return null;
    }
}
