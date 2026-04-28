using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles picking up items from the world and adding them to the inventory.
/// </summary>
/// <remarks>
/// This system:
/// - Adds items to the player's inventory
/// - Removes the item object from the scene
/// - Triggers quest progression for specific items
/// - Activates tutorial hints when required
///
/// Acts as a bridge between world items and gameplay systems.
/// </remarks>
public class ItemPickup : MonoBehaviour
{
    /// <summary>Reference to the inventory manager.</summary>
    private InvenotryManager inventoryManager;

    /// <summary>Item data associated with this pickup.</summary>
    public Item item;

    /// <summary>Reference to the quest system.</summary>
    private QuestService _questService;

    /// <summary>Reference to the tutorial system.</summary>
    private IntroductionService _introductionService;

    /// <summary>
    /// Initialises required system references.
    /// </summary>
    public void Start()
    {
        inventoryManager = GameObject.Find("InventoryManager").GetComponent<InvenotryManager>();
        _questService = GameObject.Find("QuestService").GetComponent<QuestService>();
    }

    /// <summary>
    /// Handles the pickup logic when the player collects the item.
    /// </summary>
    /// <remarks>
    /// Adds the item to the inventory, destroys the world object,
    /// and triggers any relevant quest or tutorial events.
    /// </remarks>
    public void Pickup()
    {
        inventoryManager.AddItem(item);

        Destroy(gameObject);

        // Special behaviour for key item
        if(item.itemName == "Key")
        {
            _questService.SatisfyQuest("Key");

            // Trigger tutorial if in demo scene
            if(SceneManager.GetActiveScene().name == "Demo")
            {
                _introductionService = GameObject.Find("IntroductionService").GetComponent<IntroductionService>();
                _introductionService.KeyTutorial();
            }
        }
    }
}