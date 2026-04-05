using UnityEngine;

public class QuestManager : MonoBehaviour
{
    [Header("Player")]
    public GameObject player;
    [Header("Quest Objects")]
    public GameObject       invisibleWall;
    public DragonInteraction dragon;
    public Gate             gate;
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
            Debug.LogError("[QuestManager] No Inventory on Player!");
            return;
        }
        _inventory.OnAllItemsCollected += OnAllItemsCollected;
        Debug.Log("[QuestManager] Ready.");
    }
    private void OnDestroy()
    {
        if (_inventory != null)
            _inventory.OnAllItemsCollected -= OnAllItemsCollected;
    }
    private void OnAllItemsCollected()
    {
        Debug.Log("[QuestManager] All items collected!");
        if (invisibleWall != null) invisibleWall.SetActive(false);
        if (dragon != null)        dragon.hasAllItems = true;
        if (gate != null)          gate.OpenGate();
    }
}