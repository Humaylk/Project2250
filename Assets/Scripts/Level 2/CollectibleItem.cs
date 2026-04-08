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
            // Find the player — prefer HeroKnight component search (tag may not be set)
            GameObject player = null;
            HeroKnight hk = FindFirstObjectByType<HeroKnight>();
            if (hk != null)
                player = hk.gameObject;
            if (player == null)
                player = GameObject.FindWithTag("Player");

            if (player != null)
            {
                Inventory inv = player.GetComponent<Inventory>();
                if (inv != null)
                {
                    inv.AddItem(itemType);
                    Debug.Log("[CollectibleItem] Picked up and registered: " + itemType);
                }
                else
                    Debug.LogWarning("[CollectibleItem] No Inventory found on player!");
            }

            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_playerNearby) return;

        // Accept by tag OR by having a known player component
        if (!other.CompareTag("Player")
            && other.GetComponent<PlayerController>()         == null
            && other.GetComponent<HeroKnight>()               == null
            && other.GetComponentInParent<HeroKnight>()       == null) return;

        _playerNearby = true;
        Debug.Log("[CollectibleItem] Player near " + itemType + " - press E to pick up");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")
            && other.GetComponent<PlayerController>()   == null
            && other.GetComponent<HeroKnight>()         == null
            && other.GetComponentInParent<HeroKnight>() == null) return;

        _playerNearby = false;
    }
}
