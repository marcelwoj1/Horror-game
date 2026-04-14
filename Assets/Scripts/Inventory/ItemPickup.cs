using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    private InvenotryManager inventoryManager;
    public Item item;
    private QuestService _questService;

    public void Start()
    {
        inventoryManager = GameObject.Find("InventoryManager").GetComponent<InvenotryManager>();
        _questService = GameObject.Find("QuestService").GetComponent<QuestService>();
    }

    public void Pickup()
    {
        inventoryManager.AddItem(item);
        Destroy(gameObject);
        if(item.itemName == "Key")
        {
            _questService.SatisfyQuest("Key");
        }
        
    }
}
