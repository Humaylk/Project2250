using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    public string itemType; // MagicItem, Scroll, Iron, Stone, Wood

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Inventory inventory = other.GetComponent<Inventory>();

            if (inventory != null)
            {
                inventory.AddItem(itemType);
                Destroy(gameObject);
            }
        }
    }
}