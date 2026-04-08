using UnityEngine;

// Attach to the HeroKnight in Level 3.
// Replaces the Animator Controller with HeroKnightSwimming_0 so the player
// uses the Swimming Animation and looks like the HeroKnightSwimming sprite.
public class HeroKnightSwimmingSprite : MonoBehaviour
{
    [Tooltip("Drag in: Assets/HeroKnightSwimming_0.controller")]
    public RuntimeAnimatorController swimmingController;

    private Animator _animator;

    void Awake()
    {
        _animator = GetComponent<Animator>();

        if (_animator == null)
        {
            Debug.LogError("[HeroKnightSwimmingSprite] No Animator found on HeroKnight!");
            return;
        }

        if (swimmingController == null)
        {
            Debug.LogError("[HeroKnightSwimmingSprite] Swimming controller not assigned!");
            return;
        }

        // Replace the HeroKnight run/idle controller with the swimming one
        _animator.runtimeAnimatorController = swimmingController;
        _animator.enabled = true;

        Debug.Log("[HeroKnightSwimmingSprite] Animator controller replaced with swimming controller.");
    }
}
