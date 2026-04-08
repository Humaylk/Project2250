using UnityEngine;

/// <summary>
/// Generates Level 2's maze walls at runtime.
/// Attach to an empty GameObject called "MazeWalls" in the scene.
///
/// MAP LAYOUT  (X: -21 to 13,  Y: -9 to 9)
/// -----------------------------------------------
///  TOP LANE    Y= 3.5 to 5.5   (above upper divider)
///  MIDDLE LANE Y=-2.5 to 2.5   (main corridor, player spawn)
///  BOTTOM LANE Y=-5.5 to -3.5  (below lower divider)
///
///  Upper divider Y=3  — gaps at X[-16,-13]  [-7,-4]  [0,2]
///  Lower divider Y=-3 — gaps at X[-13,-10]  [-4,-1]  [2,5]  ← staggered → forces zigzag
///
///  Slalom walls inside middle zone force player to weave:
///    X=-14  solid Y=1..3   → player ducks south
///    X= -8  solid Y=-2..0  → player rises north
///    X= -2  solid Y=1..3   → player ducks south again
///
///  Dragon lair  X=5..13, Y=-4..4   — entrance gap at X=5, Y=-1..1
///  Volcano zone X=9..13, Y=4..9    — separated by vertical wall at X=9
/// </summary>
public class MazeWalls : MonoBehaviour
{
    [Header("Wall Appearance")]
    public Color wallColor    = new Color(0.18f, 0.10f, 0.04f, 1f); // dark stone
    public int   sortingOrder = 5;

    // { centerX, centerY, scaleX, scaleY }
    private static readonly float[,] WallDefs =
    {
        // ── TOP LANE CEILING  (Y=5.5, X=-20 to X=5) ─────────────────────────
        { -7.5f,  5.5f, 25f, 1f },

        // ── UPPER-MID DIVIDER  Y=3  (gaps: [-16,-13]  [-7,-4]  [0,2]) ───────
        { -18f,   3f,  4f, 1f },   // solid  X=-20 to -16
        { -10f,   3f,  6f, 1f },   // solid  X=-13 to  -7
        {  -2f,   3f,  4f, 1f },   // solid  X= -4 to   0
        {   3f,   3f,  2f, 1f },   // solid  X=  2 to   4

        // Top-lane internal verticals (prevent running straight across top lane)
        { -10f, 4.25f, 1f, 2.5f },   // between gap-1 and gap-2
        {  -3f, 4.25f, 1f, 2.5f },   // between gap-2 and gap-3

        // ── SLALOM WALLS inside middle zone ───────────────────────────────────
        // X=-14  solid Y=1..3  → player must duck to Y<−0.3 to pass
        { -14f,  2f, 1f, 2f },
        // X=-8   solid Y=-2..0 → player naturally passes above (Y≈0)
        {  -8f, -1f, 1f, 2f },
        // X=-2   solid Y=1..3  → player must duck south again
        {  -2f,  2f, 1f, 2f },

        // ── LOWER-MID DIVIDER  Y=-3  (gaps: [-13,-10]  [-4,-1]  [2,5]) ──────
        { -16.5f, -3f,  7f, 1f },   // solid  X=-20 to -13
        {  -7f,   -3f,  6f, 1f },   // solid  X=-10 to  -4
        {   0.5f, -3f,  3f, 1f },   // solid  X= -1 to   2
        // gap [2,5] leads from bottom lane into middle zone → then dragon area

        // Bottom-lane internal verticals
        {  -7f, -4.25f, 1f, 2.5f },
        {   0f, -4.25f, 1f, 2.5f },

        // ── BOTTOM LANE FLOOR  (Y=-5.5, X=-20 to X=5) ───────────────────────
        { -7.5f, -5.5f, 25f, 1f },

        // ── DRAGON AREA  X=4..13, Y=-4..4  (entrance gap Y=-1..1 at X=5) ────
        {  5f,  2.5f, 1f, 3f },    // left wall upper  Y= 1..4
        {  5f, -2.5f, 1f, 3f },    // left wall lower  Y=-4..-1
        {  8.5f,  4f, 9f, 1f },    // top wall         X= 4..13
        {  8.5f, -4f, 9f, 1f },    // bottom wall      X= 4..13

        // ── VOLCANO SEPARATOR  X=9, Y=4..9  (keeps players from fire zone) ──
        {  9f, 6.5f, 1f, 5f },
    };

    void Awake() => Build();

    void Build()
    {
        // Clear previously generated children (e.g. on level restart)
        for (int i = transform.childCount - 1; i >= 0; i--)
            Destroy(transform.GetChild(i).gameObject);

        // 1×1 white texture → coloured by SpriteRenderer
        var tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        var spr = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);

        int n = WallDefs.GetLength(0);
        for (int i = 0; i < n; i++)
        {
            float cx = WallDefs[i, 0];
            float cy = WallDefs[i, 1];
            float sx = WallDefs[i, 2];
            float sy = WallDefs[i, 3];

            var go = new GameObject("MW_" + i);
            go.transform.SetParent(transform, false);
            go.transform.localPosition = new Vector3(cx, cy, 0f);
            go.transform.localScale    = new Vector3(sx, sy, 1f);

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite       = spr;
            sr.color        = wallColor;
            sr.sortingOrder = sortingOrder;

            var col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = false;
        }
    }
}
