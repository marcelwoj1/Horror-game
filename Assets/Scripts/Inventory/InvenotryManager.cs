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
                if(item.SlotSize <= slot.slotSize && slot.EquipSlot == false)
                {
                    SpawnNewItem(item, slot);
                    slot.DisplayItemInfo();
                    if (item.itemName == "Axe")
                    {
                        FindAnyObjectByType<QuestService>()?.SatisfyQuest("Axe");
                    }
                    return;
                }
            }
        }
    }

    public void RemoveItem(string itemName)
    {
        for(int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            Inventory ItemInSlot = slot.GetComponentInChildren<Inventory>();
            if(ItemInSlot != null)
            {
                if(ItemInSlot.item.itemName == itemName)
                {
                    ItemInSlot.RemoveItem(itemName);
                    return;
                }
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
