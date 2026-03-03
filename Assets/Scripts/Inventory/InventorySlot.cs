using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public int slotSize;
    public bool BottomSlot;
    public void OnDrop(PointerEventData eventData)
    {
        if(transform.childCount == 0)
        {
            GameObject droppedObject = eventData.pointerDrag;
            Inventory inventory = droppedObject.GetComponent<Inventory>();
            if(inventory.SlotSize == slotSize)
            {
                inventory.parentAfterDrag = transform;
                if(BottomSlot)
                {
                    inventory.transform.eulerAngles = new Vector3(0,0,270.701996f);
                }
                else
                {
                    inventory.transform.eulerAngles = new Vector3(0,0,0);
                }
            }
        }
    }
}
