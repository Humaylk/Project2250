using UnityEngine;

public class CollectibleOrb : MonoBehaviour
{
    [Header("Identity")]
    public string orbID = "Orb_A";

    public System.Action<CollectibleOrb> OnOrbCollected;

    private bool _collected = false;
    private bool _playerNearby = false;

    public bool IsCollected() => _collected;

    void Update()
    {
        if (_collected || !_playerNearby) return;

        if (Input.GetKeyDown(KeyCode.E))
            Collect();
    }

    private bool IsPlayer(Collider2D col)
    {
        return col.CompareTag("Player") || col.GetComponent<PlayerController>() != null;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (_collected || !IsPlayer(other)) return;

        _playerNearby = true;
        Debug.Log("[CollectibleOrb] Player near " + orbID + " - press E to collect");
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!IsPlayer(other)) return;

        _playerNearby = false;
    }

    private void Collect()
    {
        _collected = true;
        _playerNearby = false;
        Debug.Log("[CollectibleOrb] " + orbID + " collected!");
        OnOrbCollected?.Invoke(this);
        gameObject.SetActive(false);
    }

    public void ResetOrb()
    {
        _collected = false;
        _playerNearby = false;
        gameObject.SetActive(true);
        Debug.Log("[CollectibleOrb] " + orbID + " reset.");
    }
}
