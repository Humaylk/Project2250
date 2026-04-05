using UnityEngine;

public class Inventory : MonoBehaviour
{
    public bool hasMagicItem = false;
    public bool hasScroll    = false;
    public bool hasIron      = false;
    public bool hasStone     = false;
    public bool hasWood      = false;


    public System.Action OnAllItemsCollected;

    public void AddItem(string itemType)
    {
        switch (itemType)
        {
            case "Magic Flame": hasMagicItem = true; Debug.Log("Picked up Magic Flame"); break;
            case "Scroll":    hasScroll    = true; Debug.Log("Picked up Scroll");     break;
            case "Iron":      hasIron      = true; Debug.Log("Picked up Iron");       break;
            case "Stone":     hasStone     = true; Debug.Log("Picked up Stone");      break;
            case "Wood":      hasWood      = true; Debug.Log("Picked up Wood");       break;
        }

        Debug.Log("Inventory -> Magic: " + hasMagicItem + ", Scroll: " + hasScroll +
                  ", Iron: " + hasIron + ", Stone: " + hasStone + ", Wood: " + hasWood);
        
        if (HasAllItems())
            OnAllItemsCollected?.Invoke();
    }

    public bool HasAllItems()
    {
        return hasMagicItem && hasScroll && hasIron && hasStone && hasWood;
    }
}