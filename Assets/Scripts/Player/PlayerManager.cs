using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages player state, input handling, and interactions with core gameplay systems.
/// </summary>
/// <remarks>
/// This script acts as a central controller for:
/// - Player state (movement, crouching, hiding, inventory)
/// - Input handling (inventory toggle, crouch)
/// - Interaction with other systems (quests, UI, combat)
/// - Temporary effects (e.g., bug spray)
///
/// It ensures consistent behaviour across multiple gameplay systems.
/// </remarks>
public class PlayerManager : MonoBehaviour
{
    [Header("Scripts")]

    /// <summary>Handles player movement logic.</summary>
    public Movement _movement;

    /// <summary>Handles player attack behaviour.</summary>
    public PlayerAttack _playerAttack;

    /// <summary>Handles quest progression.</summary>
    public QuestService _questService;

    [Header("Variables")]

    /// <summary>Indicates whether the inventory UI is open.</summary>
    public bool IsInventoryOpen = false;

    /// <summary>Determines if the player can move.</summary>
    public bool AllowMovement = true;

    /// <summary>Indicates if the game is in tutorial mode.</summary>
    public bool isTutorial = false;

    /// <summary>Indicates whether the player is hiding.</summary>
    public bool IsHiding = false;

    /// <summary>Indicates whether bug spray effect is active.</summary>
    public bool IsBugSprayActive = false;

    /// <summary>Indicates whether the player is crouching.</summary>
    public bool IsCrouching = false;

    [Header("Components")]

    /// <summary>Inventory UI GameObject.</summary>
    public GameObject Inventory;

    /// <summary>Player Rigidbody for physics control.</summary>
    public Rigidbody2D _rigidBody;

    /// <summary>Handles animation playback.</summary>
    public SpriteAnimator _animator;

    /// <summary>Handles tutorial sequences.</summary>
    public IntroductionService introductionService;

    /// <summary>UI element for orange juice timer.</summary>
    public GameObject OrangeJuiceTimer;

    /// <summary>UI element for bug spray timer.</summary>
    public GameObject BugSprayTimer;

    /// <summary>Visual effect for bug spray.</summary>
    public GameObject BugSprayEffect;

    /// <summary>UI element for controls panel.</summary>
    public GameObject ControlsPanel;

    /// <summary>
    /// Initialises references and determines game mode.
    /// </summary>
    void Start()
    {
        _movement = GetComponent<Movement>();
        _rigidBody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<SpriteAnimator>();
        _playerAttack = GetComponent<PlayerAttack>();
        _questService = GameObject.Find("QuestService").GetComponent<QuestService>();

        // Check if current scene is tutorial
        if (SceneManager.GetActiveScene().name == "Demo")
        {
            isTutorial = true;
        }

        AllowMovement = true;
    }

    /// <summary>
    /// Handles player input each frame.
    /// </summary>
    /// <remarks>
    /// - Press 'I' to toggle inventory
    /// - Press 'C' to toggle crouching
    /// - Press 'Escape' to toggle controls panel
    /// </remarks>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            InventoryButton();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            Crouch();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (ControlsPanel.activeSelf)
            {
                ControlsPanel.SetActive(false);
                Time.timeScale = 1;
            }
            else
            {
                ControlsPanel.SetActive(true);
                Time.timeScale = 0;
            }
        }
    }

    /// <summary>
    /// Toggles crouch state and updates movement permissions.
    /// </summary>
    public void Crouch()
    {
        if (!IsCrouching)
        {
            IsCrouching = true;
            AllowMovement = false;
            _animator.Play("Crouching");
        }
        else
        {
            IsCrouching = false;
            AllowMovement = true;
        }
    }

    /// <summary>
    /// Toggles the inventory UI and updates player state.
    /// </summary>
    /// <remarks>
    /// Opening the inventory:
    /// - Stops player movement
    /// - Resets velocity
    /// - Plays idle animation
    ///
    /// Closing the inventory:
    /// - Restores movement (if not hiding)
    /// - Triggers tutorial events if active
    /// </remarks>
    public void InventoryButton()
    {
        if (!IsInventoryOpen)
        {
            IsInventoryOpen = true;
            Inventory.SetActive(true);

            // Stop movement
            _rigidBody.linearVelocity = Vector2.zero;
            _animator.Play("Idle");
            AllowMovement = false;

            if (isTutorial)
            {
                introductionService = GameObject.Find("IntroductionService").GetComponent<IntroductionService>();
                introductionService.InventoryTutorial();
            }
        }
        else
        {
            IsInventoryOpen = false;
            Inventory.SetActive(false);

            if (!IsHiding)
            {
                AllowMovement = true;
            }

            if (isTutorial)
            {
                introductionService = GameObject.Find("IntroductionService").GetComponent<IntroductionService>();
                introductionService.ItemTutorial();
            }
        }
    }

    /// <summary>
    /// Handles player picking up an axe.
    /// </summary>
    /// <remarks>
    /// Triggers animation and updates quest progression.
    /// </remarks>
    public void PickAxeUp()
    {
        _animator.Play("AquireAxe");
        _questService.SatisfyQuest("Axe");
    }

    /// <summary>
    /// Activates bug spray effect.
    /// </summary>
    /// <remarks>
    /// Enables visual effects and UI indicators.
    /// </remarks>
    public void BugSprayUsed()
    {
        IsBugSprayActive = true;
        BugSprayTimer.SetActive(true);
        BugSprayEffect.SetActive(true);
    }

    /// <summary>
    /// Ends bug spray effect.
    /// </summary>
    /// <remarks>
    /// Disables visual effects and UI indicators.
    /// </remarks>
    public void BugSprayEnded()
    {
        IsBugSprayActive = false;
        BugSprayTimer.SetActive(false);
        BugSprayEffect.SetActive(false);
    }
}