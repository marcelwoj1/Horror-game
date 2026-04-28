using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

/// <summary>
/// Handles the logic for equipped items, including usage, toggling,
/// and throwing items from the player inventory.
/// </summary>
/// <remarks>
/// This system:
/// - Tracks the currently equipped item
/// - Handles player input for item usage
/// - Spawns and throws item prefabs
/// - Integrates with player systems such as health, movement, and inventory
/// </remarks>
public class EquippedItem : MonoBehaviour
{
    [Header("Variables")]

    /// <summary>The name of the currently equipped item.</summary>
    public string ItemEquipped;

    /// <summary>Indicates whether the torch is currently lit.</summary>
    [HideInInspector] public bool TorchIsLit;

    [Header("Torch")]

    /// <summary>Reference to the torch light GameObject.</summary>
    public GameObject TorchLight;
    
    [Header("Player Components")]

    /// <summary>Reference to the player manager.</summary>
    private PlayerManager _playerManager;

    /// <summary>Reference to the movement script.</summary>
    private Movement _movement;

    /// <summary>Reference to the inventory manager.</summary>
    private InvenotryManager _inventoryManager;

    /// <summary>Reference to the player health system.</summary>
    private PlayerHealth _playerHealth;

    /// <summary>Reference to teleport system.</summary>
    public Teleport _teleport;
    
    /// <summary>Reference to the animator.</summary>
    private SpriteAnimator _animator;

    [Header("Enemy Parent")]

    /// <summary>Parent transform for spawned objects (e.g., food).</summary>
    public Transform FoodParent;
    
    [Header("Components")]

    /// <summary>Sprite renderer used to determine facing direction.</summary>
    private SpriteRenderer _spriteRenderer;

    /// <summary>Dictionary mapping item names to prefabs.</summary>
    private Dictionary<string, GameObject> prefabDict;

    [Header("Item Prefabs")]

    public GameObject BugSprayPrefab;
    public GameObject OrangeJuicePrefab;
    public GameObject FlashlightPrefab;
    public GameObject JuicyMorselPrefab;
    public GameObject KeyPrefab;
    public GameObject AxePrefab;
    public GameObject SpiderFoodPrefab;

    [Header("Throwing Force")]

    /// <summary>Forward force applied when throwing items.</summary>
    public float forwardForce = 10f;

    /// <summary>Upward force applied when throwing items.</summary>
    public float upForce = 5f;

    /// <summary>
    /// Initialises references and prefab dictionary.
    /// </summary>
    void Start()
    {
        _movement = GameObject.Find("Player").GetComponent<Movement>();
        _animator = GameObject.Find("Player").GetComponent<SpriteAnimator>();
        _inventoryManager = GameObject.Find("InventoryManager").GetComponent<InvenotryManager>();
        _playerHealth = GameObject.Find("Player").GetComponent<PlayerHealth>();
        _playerManager = GameObject.Find("Player").GetComponent<PlayerManager>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        prefabDict = new Dictionary<string, GameObject>
        {
            { "BugSpray", BugSprayPrefab },
            { "OrangeJuice", OrangeJuicePrefab },
            { "Flashlight", FlashlightPrefab },
            { "JuicyMorsels", JuicyMorselPrefab },
            { "Key", KeyPrefab },
            { "Axe", AxePrefab },
            { "SpiderFood", SpiderFoodPrefab }
        };
    }

    /// <summary>
    /// Sets the currently equipped item and updates player state.
    /// </summary>
    /// <param name="item">Name of the item to equip.</param>
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

    /// <summary>
    /// Handles player input for using and throwing items.
    /// </summary>
    void Update()
    {
        // Throw item input
        if(Input.GetKeyDown(KeyCode.Q) && ItemEquipped != "" && _playerManager.IsInventoryOpen == false)
        {
            Debug.Log("Throwing " + ItemEquipped);
            ThrowItem(ItemEquipped);
            _inventoryManager.RemoveItem(ItemEquipped);
        }

        // Use item input
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
                    ThrowItem("SpiderFood");
                    _inventoryManager.RemoveItem(ItemEquipped);
                    break;

                case "OrangeJuice":
                    _playerHealth.Heal(3);
                    _inventoryManager.RemoveItem(ItemEquipped);
                    break;

                case "Key":
                    _teleport.NoKeyInDoor();
                    break;

                default:
                    Debug.Log("No item equipped");
                    break;
            }
        }

        // Ensure torch is off if not equipped
        if(_movement.FlashlightEquipped == false)
        {
            TorchIsLit = false;
            TorchLight.SetActive(TorchIsLit);
        }
    }

    /// <summary>
    /// Spawns and throws an item prefab with force.
    /// </summary>
    /// <param name="item">Item name used to find prefab.</param>
    void ThrowItem(string item)
    {
        if (prefabDict.TryGetValue(item, out GameObject prefab))
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
        else
        {
            Debug.LogWarning("No prefab found for: " + item);
        }
    }
}