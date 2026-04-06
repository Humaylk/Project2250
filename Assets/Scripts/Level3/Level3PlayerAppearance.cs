using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Swaps the player's animations in Level 3 using AnimatorOverrideController.
// Idle -> PlayerIdle.anim
// Moving -> Player Swimming.anim (or Player Swimming Helmet.anim after pickup)
// Attacking -> PlayerSlash.anim
public class Level3PlayerAppearance : MonoBehaviour
{
    [Header("Level 3 Animation Clips")]
    public AnimationClip idleClip;          // PlayerIdle.anim
    public AnimationClip swimmingClip;      // Player Swimming.anim
    public AnimationClip swimmingHelmetClip;// Player Swimming Helmet.anim
    public AnimationClip attackClip;        // PlayerSlash.anim

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

        AnimationClip swimClip = helmet ? swimmingHelmetClip : swimmingClip;

        for (int i = 0; i < overrides.Count; i++)
        {
            if (overrides[i].Key == null) continue;
            string name = overrides[i].Key.name;

            if (name == "Idle" && idleClip != null)
            {
                overrides[i] = new KeyValuePair<AnimationClip, AnimationClip>(overrides[i].Key, idleClip);
                Debug.Log("[L3Appearance] Overriding Idle -> " + idleClip.name);
            }
            else if (name == "Run" && swimClip != null)
            {
                overrides[i] = new KeyValuePair<AnimationClip, AnimationClip>(overrides[i].Key, swimClip);
                Debug.Log("[L3Appearance] Overriding Run -> " + swimClip.name);
            }
            else if (name == "Attack" && attackClip != null)
            {
                overrides[i] = new KeyValuePair<AnimationClip, AnimationClip>(overrides[i].Key, attackClip);
                Debug.Log("[L3Appearance] Overriding Attack -> " + attackClip.name);
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
