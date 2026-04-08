using UnityEngine;

public class QuestManager : MonoBehaviour
{
    [Header("Player")]
    public GameObject player;
    [Header("Quest Objects")]
    public DragonInteraction dragon;
    public Gate              gate;

    private Inventory _inventory;

    private void Start()
    {
        if (player == null)
            player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogError("[QuestManager] Player not found!");
            return;
        }
        _inventory = player.GetComponent<Inventory>();
        if (_inventory == null)
        {
            // Auto-add Inventory if it's missing — no manual setup needed
            _inventory = player.AddComponent<Inventory>();
            Debug.Log("[QuestManager] Inventory component added to player automatically.");
        }
        _inventory.OnAllItemsCollected += OnAllItemsCollected;

        // Hide the gate at the start — it appears only when all items are collected
        if (gate != null) gate.HideGate();

        Debug.Log("[QuestManager] Ready.");
    }

    private void OnDestroy()
    {
        if (_inventory != null)
            _inventory.OnAllItemsCollected -= OnAllItemsCollected;
    }

    private void OnAllItemsCollected()
    {
        Debug.Log("[QuestManager] All items collected — waiting for dragon dialogue.");
        if (dragon != null)
        {
            dragon.hasAllItems = true;
            dragon.gateToReveal = gate; // dragon will open the gate after its dialogue
        }
    }
}
