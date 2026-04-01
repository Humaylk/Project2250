using UnityEngine;
using TMPro;

// Attach to any GameObject that needs a world-space popup hint.
// Shows the hint text only when the player is within proximityRange.
// Used on FishAssassin ("Press G to kill the killer fish")
// The RockBarrier/bomb uses the existing InteractionSystem instead.
public class ProximityPopup : MonoBehaviour
{
    [Header("Settings")]
    [TextArea]
    public string popupText = "PRESS G TO KILL THE KILLER FISH";
    public float proximityRange = 2.5f;

    [Header("References")]
    public TMP_Text popupLabel;   // drag the TMP_Text child here in Inspector

    private Transform player;

    void Start()
    {
        // Auto-find the player
        PlayerController pc = FindFirstObjectByType<PlayerController>();
        if (pc != null) player = pc.transform;

        // Set the text content and hide on start
        if (popupLabel != null)
        {
            popupLabel.text = popupText;
            popupLabel.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (player == null || popupLabel == null) return;

        float dist = Vector2.Distance(transform.position, player.position);
        bool inRange = dist <= proximityRange;

        // Only toggle when state actually changes to avoid every-frame SetActive calls
        if (inRange != popupLabel.gameObject.activeSelf)
            popupLabel.gameObject.SetActive(inRange);
    }

    // Draw the detection radius in Scene view for easy positioning
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, proximityRange);
    }
}
