using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public int slotSize;
    public bool EquipSlot;
    public void OnDrop(PointerEventData eventData)
    {
        if(transform.childCount == 0)
        {
            GameObject droppedObject = eventData.pointerDrag;
            Inventory inventory = droppedObject.GetComponent<Inventory>();
            if(inventory.SlotSize <= slotSize)
            {
                inventory.parentAfterDrag = transform;
                if(EquipSlot)
                {
                    inventory.IsEquiped = true;
                    inventory.CheckIfEquiped();
                }
                else
                {
                    inventory.IsEquiped = false;
                    inventory.CheckIfEquiped();
                }
            }
        }
    }
}
