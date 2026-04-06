using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Swaps the player's animations in Level 3 using AnimatorOverrideController.
// Idle -> PlayerIdle.anim
// Moving -> Player Swimming.anim (or Player Swimming Helmet.anim after pickup)
// Attacking -> PlayerSlash.anim
public class Level3PlayerAppearance : MonoBehaviour
{
    [Header("No Helmet Clips")]
    public AnimationClip idleClip;          // PlayerIdle.anim
    public AnimationClip swimmingClip;      // Player Swimming.anim
    public AnimationClip attackClip;        // PlayerSlash.anim

    [Header("Helmet Clips")]
    public AnimationClip swimmingHelmetClip;// Player Swimming Helmet.anim
    public AnimationClip idleHelmetClip;    // L3_Idle_Helmet.anim
    public AnimationClip attackHelmetClip;  // L3_Attack_Helmet.anim

    private Animator animator;
    private AnimatorOverrideController overrideController;
    private RuntimeAnimatorController originalController;
    private bool hasHelmet = false;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        if (animator == null) return;

        originalController = animator.runtimeAnimatorController;
        // Delay by one frame to avoid Unity Editor Animator graph NullRef bug
        StartCoroutine(ApplyOverridesNextFrame());
    }

    private IEnumerator ApplyOverridesNextFrame()
    {
        yield return null;
        ApplyOverrides(false);
    }

    public void EquipHelmet()
    {
        hasHelmet = true;
        ApplyOverrides(hasHelmet);
    }

    private void ApplyOverrides(bool helmet)
    {
        if (animator == null || originalController == null) return;

        overrideController = new AnimatorOverrideController(originalController);

        var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>(overrideController.overridesCount);
        overrideController.GetOverrides(overrides);

        AnimationClip swimClip   = helmet ? swimmingHelmetClip : swimmingClip;
        AnimationClip currentIdle   = helmet && idleHelmetClip   != null ? idleHelmetClip   : idleClip;
        AnimationClip currentAttack = helmet && attackHelmetClip  != null ? attackHelmetClip  : attackClip;

        for (int i = 0; i < overrides.Count; i++)
        {
            if (overrides[i].Key == null) continue;
            string name = overrides[i].Key.name;

            if (name == "Idle" && currentIdle != null)
            {
                overrides[i] = new KeyValuePair<AnimationClip, AnimationClip>(overrides[i].Key, currentIdle);
                Debug.Log("[L3Appearance] Overriding Idle -> " + currentIdle.name);
            }
            else if (name == "Run" && swimClip != null)
            {
                overrides[i] = new KeyValuePair<AnimationClip, AnimationClip>(overrides[i].Key, swimClip);
                Debug.Log("[L3Appearance] Overriding Run -> " + swimClip.name);
            }
            else if (name == "Attack" && currentAttack != null)
            {
                overrides[i] = new KeyValuePair<AnimationClip, AnimationClip>(overrides[i].Key, currentAttack);
                Debug.Log("[L3Appearance] Overriding Attack -> " + currentAttack.name);
            }
        }
        if (swimClip == null) Debug.LogWarning("[L3Appearance] swimClip is NULL — assign Swimming Clip in Inspector!");

        overrideController.ApplyOverrides(overrides);
        animator.runtimeAnimatorController = overrideController;
    }

    // Restore original animator when leaving Level 3
    public void RestoreOriginal()
    {
        hasHelmet = false;
        if (animator != null && originalController != null)
            animator.runtimeAnimatorController = originalController;
    }
}
