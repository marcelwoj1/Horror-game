using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    private InvenotryManager inventoryManager;
    public Item item;
    private ItemInformation itemInformation;
    private QuestService _questService;

    public void Start()
    {
        inventoryManager = GameObject.Find("InventoryManager").GetComponent<InvenotryManager>();
        itemInformation = GameObject.Find("ItemInformation").GetComponent<ItemInformation>();
        _questService = GameObject.Find("QuestService").GetComponent<QuestService>();
    }

    public void Pickup()
    {
        inventoryManager.AddItem(item);
        Destroy(gameObject);
        itemInformation.GetItemInfo(item.name);
        if(item.name == "Key")
        {
            _questService.SatisfyQuest("Key");
        }
        
    }
}
