using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public InvenotryManager inventoryManager;
    public Item item;

    public void Pickup()
    {
        inventoryManager.AddItem(item);
        Destroy(gameObject);
    }
}
