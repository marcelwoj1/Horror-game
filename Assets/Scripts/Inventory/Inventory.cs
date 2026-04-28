using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Represents a draggable inventory item within the UI system.
/// </summary>
/// <remarks>
/// This system:
/// - Stores item data (name, description, size)
/// - Handles drag-and-drop interactions
/// - Updates equipped state when placed in equip slots
/// - Integrates with the EquippedItem system
/// </remarks>
public class Inventory : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Item Slot")]

    /// <summary>Size of the item used for slot compatibility.</summary>
    public int SlotSize;

    /// <summary>Indicates whether the item is currently equipped.</summary>
    public bool IsEquiped;

    [Header("Item Info")]

    /// <summary>Name of the item.</summary>
    public string ItemName;

    /// <summary>Description of the item.</summary>
    public string ItemDescription;

    /// <summary>Reference to the item data.</summary>
    public Item item;
    
    [Header("Item Image")]

    /// <summary>UI image used to display the item icon.</summary>
    public Image image;
    
    /// <summary>Reference to the equipped item system.</summary>
    private EquippedItem equippedItem;
    
    /// <summary>Stores the original parent transform during dragging.</summary>
    [HideInInspector] public Transform parentAfterDrag;
    
    /// <summary>
    /// Initialises references to required components.
    /// </summary>
    void Start()
    {
        equippedItem = GameObject.Find("Player").GetComponent<EquippedItem>();
    }

    /// <summary>
    /// Initialises the inventory item with provided data.
    /// </summary>
    /// <param name="newItem">Item data to assign.</param>
    public void Initialize(Item newItem)
    {
        item = newItem;
        SlotSize = newItem.SlotSize;
        image.sprite = newItem.itemImage;
        ItemName = newItem.itemName;
        ItemDescription = newItem.ItemDescription;
    }

    /// <summary>
    /// Removes the item if the name matches.
    /// </summary>
    /// <param name="itemName">Name of the item to remove.</param>
    public void RemoveItem(string itemName)
    {
        if(ItemName == itemName)
        {
            Destroy(gameObject);
            equippedItem.SetItem("");
        }
    }

    /// <summary>
    /// Called when dragging begins.
    /// </summary>
    /// <param name="eventData">Pointer event data.</param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        image.raycastTarget = false;
    }

    /// <summary>
    /// Updates the position of the item while dragging.
    /// </summary>
    /// <param name="eventData">Pointer event data.</param>
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    /// <summary>
    /// Called when dragging ends and restores the parent.
    /// </summary>
    /// <param name="eventData">Pointer event data.</param>
    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(parentAfterDrag);
        image.raycastTarget = true;
    }

    /// <summary>
    /// Updates the equipped state based on slot placement.
    /// </summary>
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