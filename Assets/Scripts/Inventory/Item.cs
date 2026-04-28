using UnityEngine;

/// <summary>
/// Defines an item used within the inventory system.
/// </summary>
/// <remarks>
/// This ScriptableObject stores data for inventory items, including:
/// - Item name
/// - Slot size requirements
/// - Visual representation (sprite)
/// - Description text
///
/// Using ScriptableObjects allows items to be reused and edited
/// independently from runtime logic.
/// </remarks>
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    /// <summary>Name of the item.</summary>
    public string itemName;

    /// <summary>Size of the item for inventory slot compatibility.</summary>
    public int SlotSize;

    /// <summary>Sprite used to represent the item in the UI.</summary>
    public Sprite itemImage;

    /// <summary>Description of the item.</summary>
    public string ItemDescription;
}