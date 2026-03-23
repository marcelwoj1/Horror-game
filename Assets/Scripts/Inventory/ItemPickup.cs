using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    private InvenotryManager inventoryManager;
    public Item item;

    public void Start()
    {
        inventoryManager = GameObject.Find("InventoryManager").GetComponent<InvenotryManager>();
    }

    public void Pickup()
    {
        inventoryManager.AddItem(item);
        Destroy(gameObject);
    }
}
