using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public int slotSize;
    public bool EquipSlot;
    public Text ItemName;
    public Text ItemInfo;
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
    public void DisplayItemInfo()
    {
        Inventory inventory = GetComponentInChildren<Inventory>();
        ItemName.text = inventory.ItemName;
        ItemInfo.text = inventory.ItemDescription;
    }
}
