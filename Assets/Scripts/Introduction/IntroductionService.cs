using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Manages tutorial progression and displays instructional UI panels to the player.
/// </summary>
/// <remarks>
/// This system:
/// - Controls when tutorial messages are shown
/// - Pauses and resumes gameplay during tutorials
/// - Ensures each tutorial is only shown once
/// - Guides the player through core mechanics (movement, inventory, items, etc.)
///
/// Tutorials are triggered either automatically or via external events.
/// </remarks>
public class IntroductionService : MonoBehaviour
{
    [Header("Tutorial Panel")]

    /// <summary>UI panel used to display tutorial messages.</summary>
    public GameObject panel;

    /// <summary>Text component used for displaying tutorial hints.</summary>
    public Text HintText;

    /// <summary>Reference to player manager for movement control.</summary>
    public PlayerManager playerManager;

    [Header("Tutorials")]

    /// <summary>Indicates if the start tutorial has been completed.</summary>
    public bool StartTutorialDone;

    /// <summary>Indicates if the movement tutorial has been completed.</summary>
    public bool MovementTutorialDone;

    /// <summary>Indicates if the drawer tutorial has been completed.</summary>
    public bool DrawerTutorialDone;

    /// <summary>Indicates if the rat tutorial has been completed.</summary>
    public bool RatTutorialDone;

    /// <summary>Indicates if the inventory tutorial has been completed.</summary>
    public bool InventoryTutorialDone;

    /// <summary>Indicates if the item tutorial has been completed.</summary>
    public bool ItemTutorialDone;

    /// <summary>Indicates if the key tutorial has been completed.</summary>
    public bool KeyTutorialDone;

    [Header("Tutorial Texts")]

    /// <summary>Initial tutorial message.</summary>
    public string StartHintText = "Welcome to the Domatophobia! This is a tutorial level to show you the basics of the game. Click the left mouse button to continue. If at any times you forget the controls just press escape to open or close the controls menu.";

    /// <summary>Movement tutorial message.</summary>
    public string MovementHintText = "You can use the A and D keys to move left and right, and the space bar to jump. You can also use C to crouch if your feeling extra sneeky(maybe when trying to take something off someone).";

    /// <summary>Drawer interaction tutorial message.</summary>
    public string DrawerHintText = "Items dropped from drawers can be picked up by clicking E while stood over them. Once picked up they will be stored in your inventory which you can check by clicking the inventory button or I.";

    /// <summary>Inventory tutorial message.</summary>
    public string InventoryHintText = "Items picked up are stored here- with what they are and what they do detailed- and can be equipped by dragging the icon of the item into the Equipped item slot. You can then close the inventory by clicking the inventory button again or clicking I.";

    /// <summary>Item usage tutorial message.</summary>
    public string ItemHintText = "Once you have an item equipped you can use it by clicking the left mouse button. Different items have different uses. You can also drop items by clicking the Q key. Items dont stack so watch your inventory space.";

    /// <summary>Key usage tutorial message.</summary>
    public string KeyHintText = "Some doors are locked and require a key to open. You will find keys in different places such as drawers or maybe even held by enemies. Use this key to open the door in front of you and begin the game... good luck.";

    /// <summary>
    /// Initialises references and starts the tutorial sequence.
    /// </summary>
    void Start()
    {
        playerManager = GameObject.Find("Player").GetComponent<PlayerManager>();
        StartCoroutine(StartTutorialDelayed());
    }

    /// <summary>
    /// Delays the start tutorial by one frame to ensure UI is initialised.
    /// </summary>
    IEnumerator StartTutorialDelayed()
    {
        yield return null;
        StartTutorial();
    }

    /// <summary>
    /// Displays the initial tutorial message.
    /// </summary>
    public void StartTutorial()
    {
        StartTutorialDone = true;
        HintText.text = StartHintText;
        panel.SetActive(true);
        playerManager.AllowMovement = false;
    }

    /// <summary>
    /// Handles player input to progress or close tutorial panels.
    /// </summary>
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && panel.activeSelf)
        {
            panel.SetActive(false);
            Time.timeScale = 1f;
            playerManager.AllowMovement = true;

            // Trigger movement tutorial if not yet shown
            if (!MovementTutorialDone)
            {
                MovementTutorialDone = true;
                ShowPanel(MovementHintText);
            }
        }
    }

    /// <summary>
    /// Displays the drawer interaction tutorial if not already shown.
    /// </summary>
    public void DrawerTutorial()
    {
        if (DrawerTutorialDone) return;

        DrawerTutorialDone = true;
        ShowPanel(DrawerHintText);
    }

    /// <summary>
    /// Displays the inventory tutorial if not already shown.
    /// </summary>
    public void InventoryTutorial()
    {
        if (InventoryTutorialDone) return;

        InventoryTutorialDone = true;
        ShowPanel(InventoryHintText);
    }

    /// <summary>
    /// Displays the item usage tutorial if not already shown.
    /// </summary>
    public void ItemTutorial()
    {
        if (ItemTutorialDone) return;

        ItemTutorialDone = true;
        ShowPanel(ItemHintText);
    }

    /// <summary>
    /// Displays the key usage tutorial if not already shown.
    /// </summary>
    public void KeyTutorial()
    {
        if (KeyTutorialDone) return;

        KeyTutorialDone = true;
        ShowPanel(KeyHintText);
    }

    /// <summary>
    /// Shows the tutorial panel with specified text and pauses the game.
    /// </summary>
    /// <param name="hintText">Text to display in the tutorial panel.</param>
    public void ShowPanel(string hintText)
    {
        panel.SetActive(true);
        HintText.text = hintText;
        Time.timeScale = 0f;
    }
}