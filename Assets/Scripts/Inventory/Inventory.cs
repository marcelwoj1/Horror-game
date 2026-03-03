using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Inventory : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image image;
    public int SlotSize;
    public bool isBottomSlot;
    
    [HideInInspector] public Transform parentAfterDrag;
    [HideInInspector] public Item item;

    public void Initialize(Item newItem)
    {
        item = newItem;
        SlotSize = newItem.SlotSize;
        image.sprite = newItem.itemImage;
        CheckIfBottomSlot();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Begin Drag");
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Drag");
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("End Drag");
        transform.SetParent(parentAfterDrag);
        image.raycastTarget = true;
    }
    public void CheckIfBottomSlot()
    {
        if(isBottomSlot)
        {
            transform.eulerAngles = new Vector3(0,0,270.701996f);
        }
        else
        {
            transform.eulerAngles = new Vector3(0,0,0);
        }
    }
}
