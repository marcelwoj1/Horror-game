using UnityEngine;
using UnityEngine.EventSystems;

public class EquippedItem : MonoBehaviour
{
    public string ItemEquipped;
    private Movement _movement;
    private Hiding _hiding;
    private SpriteAnimator _animator;
    public GameObject TorchLight;
    [HideInInspector] public bool TorchIsLit;
    private InvenotryManager _inventoryManager;
    private PlayerHealth _playerHealth;

    void Start()
    {
        _movement = GameObject.Find("Player").GetComponent<Movement>();
        _animator = GameObject.Find("Player").GetComponent<SpriteAnimator>();
        _hiding = GameObject.Find("Player").GetComponent<Hiding>();
        _inventoryManager = GameObject.Find("InventoryManager").GetComponent<InvenotryManager>();
        _playerHealth = GameObject.Find("Player").GetComponent<PlayerHealth>();
    }

    public void SetItem(string item)
    {
        ItemEquipped = item;
        switch (ItemEquipped)
        {
            case "Axe":
                _movement.AxeEquipped = true;
                break;
            case "Flashlight":
                _movement.FlashlightEquipped = true;
                break;
            default:
                _movement.AxeEquipped = false;
                _movement.FlashlightEquipped = false;
                break;
        }
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0) && ItemEquipped != "" && _movement.IsInventoryOpen == false)
        {
            if (EventSystem.current.IsPointerOverGameObject())
            return;


            switch (ItemEquipped)
    {
        case "Axe":
            _animator.Play("Attack");
            break;

        case "Flashlight":
            if(TorchIsLit == false){TorchIsLit = true;} else {TorchIsLit = false;}
            TorchLight.SetActive(TorchIsLit);
            break;

        case "BugSpray":
            _hiding.BugSprayUsed();
            _inventoryManager.RemoveItem(ItemEquipped);
            break;

        case "JuicyMorsels":
            _playerHealth.Heal(2);
            _inventoryManager.RemoveItem(ItemEquipped);
            break;

        case "OrangeJuice":
            _playerHealth.Heal(1);
            _inventoryManager.RemoveItem(ItemEquipped);
            break;

        default:
            Debug.Log("No item equipped");
            break;
    }
        }
        if(_movement.FlashlightEquipped == false)
        {
            TorchIsLit = false;
            TorchLight.SetActive(TorchIsLit);
        }
    }
}
