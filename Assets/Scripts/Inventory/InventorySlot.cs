using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Represents an inventory slot that can receive and display items.
/// </summary>
/// <remarks>
/// This system:
/// - Handles drag-and-drop item placement
/// - Supports swapping items between slots
/// - Validates slot size compatibility
/// - Updates UI text to reflect current item contents
/// - Determines whether items are equipped based on slot type
/// </remarks>
public class InventorySlot : MonoBehaviour, IDropHandler
{
    /// <summary>Maximum item size this slot can hold.</summary>
    public int slotSize;

    /// <summary>Indicates whether this slot is an equipment slot.</summary>
    public bool EquipSlot;

    /// <summary>UI text displaying the item name.</summary>
    public Text ItemName;

    /// <summary>UI text displaying the item description.</summary>
    public Text ItemInfo;

    /// <summary>
    /// Updates the slot UI each frame based on its contents.
    /// </summary>
    void Update()
    {
        if(transform.childCount == 0)
        {
            ItemName.text = "Empty";
            ItemInfo.text = "";
        }
        else
        {
            DisplayItemInfo();
        }
    }

    /// <summary>
    /// Handles item drop events during drag-and-drop.
    /// </summary>
    /// <param name="eventData">Pointer event data.</param>
    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag;
        Inventory draggedInventory = droppedObject.GetComponent<Inventory>();

        if(transform.childCount == 0)
        {
            // Empty slot — just place the item
            if(draggedInventory.SlotSize <= slotSize)
            {
                PlaceItem(draggedInventory);
            }
        }
        else
        {
            // Occupied slot — attempt a swap
            Inventory existingInventory = GetComponentInChildren<Inventory>();
            if(existingInventory == null || existingInventory == draggedInventory) return;

            InventorySlot sourceSlot = draggedInventory.parentAfterDrag.GetComponent<InventorySlot>();
            if(sourceSlot == null) return;

            // Check both items fit in their new slots
            if(draggedInventory.SlotSize <= slotSize && existingInventory.SlotSize <= sourceSlot.slotSize)
            {
                // Move existing item to the source slot
                existingInventory.transform.SetParent(sourceSlot.transform);

                if(sourceSlot.EquipSlot)
                {
                    existingInventory.IsEquiped = true;
                }
                else
                {
                    existingInventory.IsEquiped = false;
                }

                existingInventory.CheckIfEquiped();
                sourceSlot.DisplayItemInfo();

                // Place dragged item into this slot
                PlaceItem(draggedInventory);
            }
        }
    }

    /// <summary>
    /// Places an item into this slot and updates its equipped state.
    /// </summary>
    /// <param name="inventory">Item being placed.</param>
    private void PlaceItem(Inventory inventory)
    {
        inventory.parentAfterDrag = transform;

        if(EquipSlot)
        {
            inventory.IsEquiped = true;
        }
        else
        {
            inventory.IsEquiped = false;
        }

        inventory.CheckIfEquiped();
    }

    /// <summary>
    /// Updates the UI text to match the current item in the slot.
    /// </summary>
    public void DisplayItemInfo()
    {
        Inventory inventory = GetComponentInChildren<Inventory>();

        ItemName.text = inventory.ItemName;
        ItemInfo.text = inventory.ItemDescription;
    }
}