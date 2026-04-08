using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    public string itemType; // Wood, Iron, Stone, Scroll, Magic Flame

    private bool _playerNearby = false;

    void Update()
    {
        if (!_playerNearby) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("[CollectibleItem] Picked up: " + itemType);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_playerNearby) return;

        // Accept by tag OR by having a PlayerController component (covers any player name)
        if (!other.CompareTag("Player") && other.GetComponent<PlayerController>() == null) return;

        _playerNearby = true;
        Debug.Log("[CollectibleItem] Player near " + itemType + " - press E to pick up");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player") && other.GetComponent<PlayerController>() == null) return;

        _playerNearby = false;
    }
}
