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
            ItemName.text = "";
            ItemInfo.text = "";
        }
    }
    public void OnDrop(PointerEventData eventData)
    {
        if(transform.childCount == 0)
        {
            GameObject droppedObject = eventData.pointerDrag;
            Inventory inventory = droppedObject.GetComponent<Inventory>();
            if(inventory.SlotSize <= slotSize)
            {
                ItemName.text = inventory.ItemName;
                ItemInfo.text = inventory.ItemDescription;
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
    public void DisplayItemInfo()
    {
        Inventory inventory = GetComponentInChildren<Inventory>();
        ItemName.text = inventory.ItemName;
        ItemInfo.text = inventory.ItemDescription;
    }
}
