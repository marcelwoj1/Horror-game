using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Inventory : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image image;
    public int SlotSize;
    public bool isBottomSlot;
    public bool IsEquiped;
    public string ItemName;
    private EquippedItem equippedItem;
    
    [HideInInspector] public Transform parentAfterDrag;
    [HideInInspector] public Item item;

    void Start()
    {
        equippedItem = GameObject.Find("Player").GetComponent<EquippedItem>();
    }

    public void Initialize(Item newItem)
    {
        item = newItem;
        SlotSize = newItem.SlotSize;
        image.sprite = newItem.itemImage;
        ItemName = newItem.itemName;
        CheckIfBottomSlot();
    }
    public void RemoveItem(string itemName)
    {
        if(ItemName == itemName)
        {
            Destroy(gameObject);
            equippedItem.SetItem("");
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
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
    public void CheckIfEquiped()
    {
        if(IsEquiped)
        {
            equippedItem.SetItem(ItemName);
        }
        else
        {
            equippedItem.SetItem("");
        }
    }
}
