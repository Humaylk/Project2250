using UnityEngine;

// Attached to HeroKnight in Level 3 by Level3PlayerReplacer.
//
// Movement (idle / swim) is driven directly via AnimationClip.SampleAnimation()
// in LateUpdate — this writes sprites to the SpriteRenderer AFTER the Animator
// has already run, so it always wins without needing any override controller
// for the Run state.
//
// Attacks / Hurt / Death still go through the HeroKnight state machine
// (triggers fire normally while the Animator stays in its Idle state).
public class Level3PlayerAnimator : MonoBehaviour
{
    [Header("Base Controller (HeroKnight_AnimController)")]
    public RuntimeAnimatorController baseController;

    [Header("Base Clips — HeroKnight.png")]
    [Tooltip("L3_Idle.anim  — frames 0-6")]
    public AnimationClip baseIdleClip;
    [Tooltip("L3_Attack.anim  — frames 18-23")]
    public AnimationClip baseAttack1Clip;
    [Tooltip("L3_Attack2.anim — frames 24-29")]
    public AnimationClip baseAttack2Clip;
    [Tooltip("L3_Attack3.anim — frames 30-37")]
    public AnimationClip baseAttack3Clip;
    [Tooltip("L3_Hurt.anim  — frames 45-47")]
    public AnimationClip baseHurtClip;
    [Tooltip("L3_Death.anim — frames 48-54")]
    public AnimationClip baseDeathClip;

    [Header("Swimming Clips")]
    [Tooltip("L3_Swimming.anim — HeroKnightSwimming.png")]
    public AnimationClip swimmingClip;
    [Tooltip("Swimming_Helmet.anim — HeroKnightSwimmingHelmet.png")]
    public AnimationClip swimmingHelmetClip;

    [Header("Helmet Clips — HeroKnightHelmet.png")]
    [Tooltip("L3_Idle_Helmet.anim — frames 0-6")]
    public AnimationClip helmetIdleClip;
    [Tooltip("L3_Hurt_Helmet.anim — frames 45-47")]
    public AnimationClip helmetHurtClip;
    [Tooltip("HeroDeathHelmet.anim — frames 48-54")]
    public AnimationClip helmetDeathClip;
    [Tooltip("L3_Attack_Helmet.anim — frames 18-23")]
    public AnimationClip helmetAttack1Clip;
    [Tooltip("L3_Attack2_Helmet.anim — frames 24-29")]
    public AnimationClip helmetAttack2Clip;
    [Tooltip("L3_Attack3_Helmet.anim — frames 30-37")]
    public AnimationClip helmetAttack3Clip;

    private Animator                   _animator;
    private SpriteRenderer             _sr;
    private AnimatorOverrideController _overrideCtrl;

    private bool  _helmetOn  = false;
    private bool  _isDead    = false;
    private bool  _isMoving  = false;
    private float _swimTime  = 0f;

    // Clip name keys that exist in HeroKnight_AnimController
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

        // Build override controller for idle / attack / hurt / death clips
        _overrideCtrl = new AnimatorOverrideController(baseController);
        _animator.runtimeAnimatorController = _overrideCtrl;

        if (baseIdleClip    != null) _overrideCtrl[CLIP_IDLE]    = baseIdleClip;
        if (baseAttack1Clip != null) _overrideCtrl[CLIP_ATTACK1] = baseAttack1Clip;
        if (baseAttack2Clip != null) _overrideCtrl[CLIP_ATTACK2] = baseAttack2Clip;
        if (baseAttack3Clip != null) _overrideCtrl[CLIP_ATTACK3] = baseAttack3Clip;
        if (baseHurtClip    != null) _overrideCtrl[CLIP_HURT]    = baseHurtClip;
        if (baseDeathClip   != null) _overrideCtrl[CLIP_DEATH]   = baseDeathClip;

        // Keep the state machine in Idle at all times.
        // Swimming is handled in LateUpdate via SampleAnimation — no Run state needed.
        _animator.SetBool("Grounded", true);
        _animator.SetInteger("AnimState", 0);
    }

    void Update()
    {
        if (_animator == null || _isDead) return;

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        _isMoving = Mathf.Abs(moveX) > 0.01f || Mathf.Abs(moveY) > 0.01f;

        // Flip sprite to face movement direction
        if (_sr != null)
        {
            if      (moveX > 0.01f)  _sr.flipX = false;
            else if (moveX < -0.01f) _sr.flipX = true;
        }

        // Always keep AnimState = 0 so the state machine stays in Idle.
        // Swimming is applied in LateUpdate after the Animator has already run.
        _animator.SetInteger("AnimState", 0);
    }

    void LateUpdate()
    {
        if (_animator == null || _isDead) return;

        // Only override with swimming if we are moving AND the Animator is in a
        // movement state (Idle/Run), not mid-attack or mid-hurt.
        if (_isMoving && IsInMovementState())
        {
            AnimationClip clip = _helmetOn ? swimmingHelmetClip : swimmingClip;
            if (clip != null)
            {
                _swimTime  = (_swimTime + Time.deltaTime) % clip.length;
                // SampleAnimation runs after Animator.Update, so it always wins.
                clip.SampleAnimation(gameObject, _swimTime);
            }
        }
        else if (!_isMoving)
        {
            _swimTime = 0f;   // Reset so swim restarts cleanly from frame 0
        }
    }

    // Returns true when the Animator is in a movement state (not attacking/hurt/dead).
    private bool IsInMovementState()
    {
        var info = _animator.GetCurrentAnimatorStateInfo(0);
        return info.IsName("Idle") || info.IsName("Run");
    }

    // ── Called by PlayerHealth ────────────────────────────────────────────────
    public void TriggerHurt()
    {
        if (_animator != null) _animator.SetTrigger("Hurt");
    }

    public void TriggerDeath()
    {
        _isDead = true;
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

        if (helmetIdleClip    != null) _overrideCtrl[CLIP_IDLE]    = helmetIdleClip;
        if (helmetHurtClip    != null) _overrideCtrl[CLIP_HURT]    = helmetHurtClip;
        if (helmetDeathClip   != null) _overrideCtrl[CLIP_DEATH]   = helmetDeathClip;
        if (helmetAttack1Clip != null) _overrideCtrl[CLIP_ATTACK1] = helmetAttack1Clip;
        if (helmetAttack2Clip != null) _overrideCtrl[CLIP_ATTACK2] = helmetAttack2Clip;
        if (helmetAttack3Clip != null) _overrideCtrl[CLIP_ATTACK3] = helmetAttack3Clip;

        Debug.Log("[Level3PlayerAnimator] Helmet clips applied.");
    }
}
