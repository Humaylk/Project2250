using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Add this script to any GameObject in the Level 3 scene (e.g. LevelManager).
// It finds the HeroKnight already placed in the scene, sets it up as the proper
// Level 3 player (adds all required scripts, wires health bar), then deletes
// the old Player object.
public class Level3PlayerReplacer : MonoBehaviour
{
    [Header("Base Animator Controller")]
    [Tooltip("Drag in: Assets/Hero Knight - Pixel Art/Animations/HeroKnight_AnimController.controller")]
    public RuntimeAnimatorController baseController;

    [Header("Swimming Clips")]
    [Tooltip("Drag in: Assets/Level_3Resources/Animations/Swimming.anim")]
    public AnimationClip swimmingClip;
    [Tooltip("Drag in: Assets/Level_3Resources/Animations/Swimming_Helmet.anim")]
    public AnimationClip swimmingHelmetClip;

    [Header("Helmet Clips")]
    [Tooltip("Drag in: Assets/Level_3Resources/Animations/L3_Idle_Helmet.anim")]
    public AnimationClip helmetIdleClip;
    [Tooltip("Drag in: Assets/Level_3Resources/Animations/L3_Hurt_Helmet.anim")]
    public AnimationClip helmetHurtClip;
    [Tooltip("Drag in: Assets/Level_3Resources/Animations/HeroDeathHelmet.anim")]
    public AnimationClip helmetDeathClip;
    [Tooltip("Drag in: Assets/Level_3Resources/Animations/L3_Attack_Helmet.anim")]
    public AnimationClip helmetAttack1Clip;
    [Tooltip("Drag in: Assets/Level_3Resources/Animations/L3_Attack2_Helmet.anim")]
    public AnimationClip helmetAttack2Clip;
    [Tooltip("Drag in: Assets/Level_3Resources/Animations/L3_Attack3_Helmet.anim")]
    public AnimationClip helmetAttack3Clip;

    void Awake()
    {
        // ── Find HeroKnight ───────────────────────────────────────────────────
        GameObject heroKnight = GameObject.Find("HeroKnight");
        if (heroKnight == null)
        {
            Debug.LogError("[Level3PlayerReplacer] HeroKnight not found in scene!");
            return;
        }

        // ── Delete the old Player object ──────────────────────────────────────
        GameObject oldPlayer = GameObject.FindWithTag("Player");
        if (oldPlayer != null && oldPlayer != heroKnight)
        {
            Debug.Log("[Level3PlayerReplacer] Deleting old Player object.");
            Destroy(oldPlayer);
        }

        // ── Tag HeroKnight as Player ──────────────────────────────────────────
        heroKnight.tag = "Player";

        // ── Disable HeroKnight.cs — it controls run/attack animations and must
        //    be off so it doesn't fight with the swimming controller ───────────
        HeroKnight hkScript = heroKnight.GetComponent<HeroKnight>();
        if (hkScript != null)
        {
            hkScript.enabled = false;
            Debug.Log("[Level3PlayerReplacer] HeroKnight.cs disabled.");
        }

        // ── Add Level3PlayerAnimator — pass base controller + all clips ────────
        Level3PlayerAnimator l3anim = heroKnight.GetComponent<Level3PlayerAnimator>();
        if (l3anim == null) l3anim = heroKnight.AddComponent<Level3PlayerAnimator>();
        l3anim.baseController      = baseController;
        l3anim.swimmingClip        = swimmingClip;
        l3anim.swimmingHelmetClip  = swimmingHelmetClip;
        l3anim.helmetIdleClip      = helmetIdleClip;
        l3anim.helmetHurtClip      = helmetHurtClip;
        l3anim.helmetDeathClip     = helmetDeathClip;
        l3anim.helmetAttack1Clip   = helmetAttack1Clip;
        l3anim.helmetAttack2Clip   = helmetAttack2Clip;
        l3anim.helmetAttack3Clip   = helmetAttack3Clip;

        // ── Fix Rigidbody2D — top-down, no gravity ────────────────────────────
        Rigidbody2D rb = heroKnight.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.constraints  = RigidbodyConstraints2D.FreezeRotation;
        }

        // ── Centre the BoxCollider2D on the body ──────────────────────────────
        // HeroKnight's pivot is at the feet — offset Y moves the collider up
        BoxCollider2D box = heroKnight.GetComponent<BoxCollider2D>();
        if (box != null)
        {
            box.offset = new Vector2(-0.08f, 0.55f);
            box.size   = new Vector2(0.71f, 0.97f);
        }

        // ── Centre the InteractionSystem collider too ─────────────────────────
        CircleCollider2D circle = heroKnight.GetComponent<CircleCollider2D>();
        if (circle != null)
            circle.offset = new Vector2(0f, 0.55f);

        // ── PlayerHealth (105/105) ────────────────────────────────────────────
        PlayerHealth ph = heroKnight.GetComponent<PlayerHealth>();
        if (ph == null) ph = heroKnight.AddComponent<PlayerHealth>();
        ph.health    = 105;
        ph.maxHealth = 105;

        // ── PlayerController ──────────────────────────────────────────────────
        PlayerController pc = heroKnight.GetComponent<PlayerController>();
        if (pc == null) pc = heroKnight.AddComponent<PlayerController>();
        pc.speed = 5f;

        // ── PlayerAttack ──────────────────────────────────────────────────────
        if (heroKnight.GetComponent<PlayerAttack>() == null)
            heroKnight.AddComponent<PlayerAttack>();

        // ── PlayerWeapon ──────────────────────────────────────────────────────
        if (heroKnight.GetComponent<PlayerWeapon>() == null)
            heroKnight.AddComponent<PlayerWeapon>();

        // ── InteractionSystem (used by chests, gates etc in Level 3) ──────────
        if (heroKnight.GetComponent<InteractionSystem>() == null)
        {
            InteractionSystem interaction = heroKnight.AddComponent<InteractionSystem>();
            interaction.interactionRange = 1.5f;
        }

        // ── Wire health bar UI ────────────────────────────────────────────────
        // Find the existing HealthBar and HP Text in the Level 3 scene
        Image    fillImage = null;
        TMP_Text hpText    = null;

        GameObject healthBarGO = GameObject.Find("HealthBar");
        if (healthBarGO != null)
        {
            Transform fillT = healthBarGO.transform.Find("Fill");
            if (fillT != null) fillImage = fillT.GetComponent<Image>();
        }

        GameObject hpTextGO = GameObject.Find("HP Text");
        if (hpTextGO != null) hpText = hpTextGO.GetComponent<TMP_Text>();

        if (fillImage != null) ph.healthBarFill = fillImage;
        if (hpText    != null) ph.healthText    = hpText;

        // ── Level3PlayerHealth — mirrors health to the UI bar ─────────────────
        Level3PlayerHealth l3ph = heroKnight.GetComponent<Level3PlayerHealth>();
        if (l3ph == null) l3ph = heroKnight.AddComponent<Level3PlayerHealth>();
        l3ph.maxHealth = 105;
        l3ph.health    = 105;
        if (fillImage != null) l3ph.healthBarFill = fillImage;
        if (hpText    != null) l3ph.hpText        = hpText as TextMeshProUGUI;

        // ── Register with GameManager ─────────────────────────────────────────
        if (GameManager.Instance != null)
            GameManager.Instance.player = pc;

        Debug.Log("[Level3PlayerReplacer] HeroKnight is now the Level 3 player.");
    }
}
