using UnityEngine;
using UnityEngine.SceneManagement;

public class ItemPickup : MonoBehaviour
{
    private InvenotryManager inventoryManager;
    public Item item;
    private QuestService _questService;
    private IntroductionService _introductionService;

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
            if(SceneManager.GetActiveScene().name == "Demo")
            {
                _introductionService = GameObject.Find("IntroductionService").GetComponent<IntroductionService>();
                _introductionService.KeyTutorial();
            }
        }
        
    }
}
