using UnityEngine;
using UnityEngine.EventSystems;

public class EquippedItem : MonoBehaviour
{
    [Header("Variables")]
    public string ItemEquipped;
    [HideInInspector] public bool TorchIsLit;

    [Header("Components")]
    public GameObject TorchLight;
    private InvenotryManager _inventoryManager;
    private PlayerHealth _playerHealth;
    public Teleport _teleport;
    private PlayerManager _playerManager;
    private Movement _movement;
    private SpriteAnimator _animator;
    public Transform FoodParent;
    private SpriteRenderer _spriteRenderer;

    [Header("Throwing Variables")]
    public GameObject prefab;
    public float forwardForce = 10f;
    public float upForce = 5f;


    void Start()
    {
        _movement = GameObject.Find("Player").GetComponent<Movement>();
        _animator = GameObject.Find("Player").GetComponent<SpriteAnimator>();
        _inventoryManager = GameObject.Find("InventoryManager").GetComponent<InvenotryManager>();
        _playerHealth = GameObject.Find("Player").GetComponent<PlayerHealth>();
        _playerManager = GameObject.Find("Player").GetComponent<PlayerManager>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
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
        if(Input.GetKeyDown(KeyCode.Mouse0) && ItemEquipped != "" && _playerManager.IsInventoryOpen == false)
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
            _playerManager.BugSprayUsed();
            _inventoryManager.RemoveItem(ItemEquipped);
            break;

        case "JuicyMorsels":
            ThrowItem();
            _inventoryManager.RemoveItem(ItemEquipped);
            break;

        case "OrangeJuice":
            _playerHealth.Heal(3);
            _inventoryManager.RemoveItem(ItemEquipped);
            break;

        case "Key":
            _teleport.NoKeyInDoor();
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
    void ThrowItem()
    {
        GameObject obj = Instantiate(prefab, transform.position, Quaternion.identity, FoodParent);

        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
        if(_spriteRenderer.flipX == true)
        {
            rb.AddForce(new Vector2(forwardForce, upForce), ForceMode2D.Impulse);
        }
        else
        {
            rb.AddForce(new Vector2(-forwardForce, upForce), ForceMode2D.Impulse);
        }
    }
}
