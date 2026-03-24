using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    private InvenotryManager inventoryManager;
    public Item item;
    private ItemInformation itemInformation;

    public void Start()
    {
        inventoryManager = GameObject.Find("InventoryManager").GetComponent<InvenotryManager>();
        itemInformation = GameObject.Find("ItemInformation").GetComponent<ItemInformation>();
    }

    public void Pickup()
    {
        inventoryManager.AddItem(item);
        Destroy(gameObject);
        itemInformation.GetItemInfo(item.name);
    }
}
