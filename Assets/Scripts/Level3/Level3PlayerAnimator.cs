using System.Collections.Generic;
using UnityEngine;

// Attached to HeroKnight in Level 3 by Level3PlayerReplacer.
// Uses AnimatorOverrideController to keep the full HeroKnight state machine
// (idle / attack / hurt / death all work) while swapping clips for
// swimming and helmet variants.
public class Level3PlayerAnimator : MonoBehaviour
{
    [Header("Base Controller (HeroKnight_AnimController)")]
    public RuntimeAnimatorController baseController;

    [Header("Swimming Clips")]
    [Tooltip("Assets/Level_3Resources/Animations/Swimming.anim")]
    public AnimationClip swimmingClip;
    [Tooltip("Assets/Level_3Resources/Animations/Swimming_Helmet.anim")]
    public AnimationClip swimmingHelmetClip;

    [Header("Helmet Clips")]
    [Tooltip("Assets/Level_3Resources/Animations/L3_Idle_Helmet.anim")]
    public AnimationClip helmetIdleClip;
    [Tooltip("Assets/Level_3Resources/Animations/L3_Hurt_Helmet.anim")]
    public AnimationClip helmetHurtClip;
    [Tooltip("Assets/Level_3Resources/Animations/HeroDeathHelmet.anim")]
    public AnimationClip helmetDeathClip;
    [Tooltip("Assets/Level_3Resources/Animations/L3_Attack_Helmet.anim (frames 18-23)")]
    public AnimationClip helmetAttack1Clip;
    [Tooltip("Assets/Level_3Resources/Animations/L3_Attack2_Helmet.anim (frames 24-29)")]
    public AnimationClip helmetAttack2Clip;
    [Tooltip("Assets/Level_3Resources/Animations/L3_Attack3_Helmet.anim (frames 30-37)")]
    public AnimationClip helmetAttack3Clip;

    private Animator                  _animator;
    private SpriteRenderer            _sr;
    private AnimatorOverrideController _overrideCtrl;

    private bool _isMoving  = false;
    private bool _helmetOn  = false;

    // Clip name keys used in HeroKnight_AnimController
    private const string CLIP_IDLE    = "HeroKnight_Idle";
    private const string CLIP_RUN     = "HeroKnight_Run";
    private const string CLIP_HURT    = "HeroKnight_Hurt";
    private const string CLIP_DEATH   = "HeroKnight_Death";
    private const string CLIP_ATTACK1 = "HeroKnight_Attack1";
    private const string CLIP_ATTACK2 = "HeroKnight_Attack2";
    private const string CLIP_ATTACK3 = "HeroKnight_Attack3";

    void Start()
    {
        _animator = GetComponent<Animator>();
        _sr       = GetComponent<SpriteRenderer>();

        if (_animator == null || baseController == null)
        {
            Debug.LogError("[Level3PlayerAnimator] Missing Animator or baseController!");
            return;
        }

        // Build override controller on top of the full HeroKnight state machine
        _overrideCtrl = new AnimatorOverrideController(baseController);
        _animator.runtimeAnimatorController = _overrideCtrl;

        // Always use swimming clip for the Run state (we're always underwater)
        if (swimmingClip != null)
            _overrideCtrl[CLIP_RUN] = swimmingClip;

        // Force idle state
        _animator.SetBool("Grounded", true);
        _animator.SetInteger("AnimState", 0);
    }

    void Update()
    {
        if (_animator == null) return;

        float moveX   = Input.GetAxisRaw("Horizontal");
        float moveY   = Input.GetAxisRaw("Vertical");
        bool isMoving = Mathf.Abs(moveX) > 0.01f || Mathf.Abs(moveY) > 0.01f;

        // Flip sprite
        if (_sr != null)
        {
            if      (moveX > 0.01f)  _sr.flipX = false;
            else if (moveX < -0.01f) _sr.flipX = true;
        }

        // AnimState drives idle (0) vs swimming/run (1)
        int animState = isMoving ? 1 : 0;
        _animator.SetInteger("AnimState", animState);
        _isMoving = isMoving;
    }

    // ── Called by PlayerHealth ────────────────────────────────────────────────
    public void TriggerHurt()
    {
        if (_animator != null) _animator.SetTrigger("Hurt");
    }

    public void TriggerDeath()
    {
        if (_animator != null)
        {
            _animator.SetBool("noBlood", false);
            _animator.SetTrigger("Death");
        }
    }

    // ── Called by HelmetPickup ────────────────────────────────────────────────
    public void EquipHelmet()
    {
        _helmetOn = true;

        if (_overrideCtrl == null) return;

        // Swap run (swimming) clip to helmet swimming version
        if (swimmingHelmetClip != null)
            _overrideCtrl[CLIP_RUN] = swimmingHelmetClip;

        // Swap idle/hurt/death/attack clips to helmet versions
        if (helmetIdleClip   != null) _overrideCtrl[CLIP_IDLE]    = helmetIdleClip;
        if (helmetHurtClip   != null) _overrideCtrl[CLIP_HURT]    = helmetHurtClip;
        if (helmetDeathClip  != null) _overrideCtrl[CLIP_DEATH]   = helmetDeathClip;
        if (helmetAttack1Clip != null) _overrideCtrl[CLIP_ATTACK1] = helmetAttack1Clip;
        if (helmetAttack2Clip != null) _overrideCtrl[CLIP_ATTACK2] = helmetAttack2Clip;
        if (helmetAttack3Clip != null) _overrideCtrl[CLIP_ATTACK3] = helmetAttack3Clip;

        Debug.Log("[Level3PlayerAnimator] Helmet clips applied via override controller.");
    }
}
