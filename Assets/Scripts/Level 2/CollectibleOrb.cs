using UnityEngine;

// A glowing orb the player walks over to collect.
// Notifies the OrbPuzzle when picked up.
public class CollectibleOrb : MonoBehaviour
{
    [Header("Identity")]
    public string orbID = "Orb_A";

    public System.Action<CollectibleOrb> OnOrbCollected;

    private bool _collected = false;
    public bool IsCollected() => _collected;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (_collected) return;
        if (!other.CompareTag("Player")) return;

        _collected = true;
        Debug.Log("[CollectibleOrb] " + orbID + " collected!");
        GameManager.Instance?.uiManager?.ShowHint(orbID + " collected!");
        OnOrbCollected?.Invoke(this);
        gameObject.SetActive(false);
    }

    // Called by OrbPuzzle.ResetPuzzle() to restore the orb.
    public void ResetOrb()
    {
        _collected = false;
        gameObject.SetActive(true);
        Debug.Log("[CollectibleOrb] " + orbID + " reset.");
    }
}
