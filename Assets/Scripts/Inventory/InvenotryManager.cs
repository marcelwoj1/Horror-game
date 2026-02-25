using UnityEngine;

public class InvenotryManager : MonoBehaviour
{
    public InventorySlot[] inventorySlots;
    public GameObject InventoryPrefab;

    public void AddItem(Item item)
    {
        for(int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            Inventory ItemInSlot = slot.GetComponentInChildren<Inventory>();
            if(ItemInSlot == null)
            {
                SpawnNewItem(item, slot);
                return;
            }
        }
    }

    void SpawnNewItem(Item item, InventorySlot slot)
    {
        GameObject newItem = Instantiate(InventoryPrefab, slot.transform);
        Inventory inventory = newItem.GetComponent<Inventory>();
        inventory.Initialize(item);
    }
}
