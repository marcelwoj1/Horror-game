using UnityEngine;

/// <summary>
/// Manages the player inventory, including adding and removing items
/// and spawning item UI elements in available slots.
/// </summary>
/// <remarks>
/// This system:
/// - Stores items in available inventory slots
/// - Ensures items fit within slot size constraints
/// - Prevents placing items into equip slots when adding
/// - Handles removal of equipped items
/// - Integrates with quest progression for specific items
/// </remarks>
public class InvenotryManager : MonoBehaviour
{
    /// <summary>Array of inventory slots available to the player.</summary>
    public InventorySlot[] inventorySlots;

    /// <summary>Prefab used to create new inventory UI items.</summary>
    public GameObject InventoryPrefab;

    /// <summary>
    /// Attempts to add an item to the inventory.
    /// </summary>
    /// <param name="item">Item to add.</param>
    /// <remarks>
    /// Finds the first empty slot that:
    /// - Has enough size capacity
    /// - Is not an equip slot
    /// </remarks>
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

                    // Trigger quest if Axe is picked up
                    if (item.itemName == "Axe")
                    {
                        FindAnyObjectByType<QuestService>()?.SatisfyQuest("Axe");
                    }

                    return;
                }
            }
        }
    }

    /// <summary>
    /// Removes an item from the inventory if it is currently equipped.
    /// </summary>
    /// <param name="itemName">Name of the item to remove.</param>
    public void RemoveItem(string itemName)
    {
        for(int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            Inventory ItemInSlot = slot.GetComponentInChildren<Inventory>();

            if(ItemInSlot != null)
            {
                if(slot.EquipSlot == true)
                {
                    if(ItemInSlot.item.itemName == itemName)
                    {
                        ItemInSlot.RemoveItem(itemName);
                        return;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Spawns a new inventory item UI element inside a given slot.
    /// </summary>
    /// <param name="item">Item data used to initialise the UI element.</param>
    /// <param name="slot">Target slot to place the item in.</param>
    void SpawnNewItem(Item item, InventorySlot slot)
    {
        GameObject newItem = Instantiate(InventoryPrefab, slot.transform);

        Inventory inventory = newItem.GetComponent<Inventory>();

        inventory.Initialize(item);
    }
}