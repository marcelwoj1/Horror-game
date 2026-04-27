using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IntroductionService : MonoBehaviour
{
    [Header("Tutorial Panel")]
    public GameObject panel;
    public Text HintText;
    public PlayerManager playerManager;

    [Header("Tutorials")]
    public bool StartTutorialDone;
    public bool MovementTutorialDone;
    public bool DrawerTutorialDone;
    public bool RatTutorialDone;
    public bool InventoryTutorialDone;
    public bool ItemTutorialDone;
    public bool KeyTutorialDone;

    [Header("Tutorial Texts")]
    public string StartHintText = "Welcome to the Domatophobia! This is a tutorial level to show you the basics of the game. Click the left mouse button to continue.";
    public string MovementHintText = "You can use the A and D keys to move left and right, and the space bar to jump. You can also use C to crouch if your feeling extra sneeky(maybe when trying to take something off someone).";
    public string DrawerHintText = "Items dropped from drawers can be picked up by clicking E while stood over them. Once picked up they will be stored in your inventory which you can check by clicking the inventory button or I.";
    public string InventoryHintText = "Items picked up are stored here- with what they are and what they do detailed- and can be equipped by dragging the icon of the item into the Equipped item slot. You can then close the inventory by clicking the inventory button again or clicking I.";
    public string ItemHintText = "Once you have an item equipped you can use it by clicking the left mouse button. Different items have different uses. You can also drop items by clicking the Q key. Items dont stack so watch your inventory space.";
    public string KeyHintText = "Some doors are locked and require a key to open. You will find keys in different places such as drawers or maybe attached to enemies. Use this key to open the door in front of you and begin the game... good luck.";

    void Start()
    {
        playerManager = GameObject.Find("Player").GetComponent<PlayerManager>();
        StartCoroutine(StartTutorialDelayed());
    }

    IEnumerator StartTutorialDelayed()
    {
        yield return null; // wait 1 frame
        StartTutorial();
    }

    public void StartTutorial()
    {
        //Plays First Tutorial
        StartTutorialDone = true;
        HintText.text = StartHintText;
        panel.SetActive(true);
        playerManager.AllowMovement = false;
    }
    public void Update()
    {
        //Removes panel when left click is pressed
        if (Input.GetMouseButtonDown(0) && panel.activeSelf)
        {
            panel.SetActive(false);
            Time.timeScale = 1f;
            playerManager.AllowMovement = true;
            
            //Plays Second Tutorial if not already played
            if(MovementTutorialDone == false)
            {
                MovementTutorialDone = true;
                ShowPanel(MovementHintText);
            }
        }
    }
    //Plays Drawer Tutorial if not already played
    public void DrawerTutorial()
    {
        if(DrawerTutorialDone == true)
            return;
        DrawerTutorialDone = true;
        ShowPanel(DrawerHintText);
    }
    //Plays Inventory Tutorial if not already played
    public void InventoryTutorial()
    {
        if(InventoryTutorialDone == true)
            return;
        InventoryTutorialDone = true;
        ShowPanel(InventoryHintText);
    }
    //Plays Item Tutorial if not already played
    public void ItemTutorial()
    {
        if(ItemTutorialDone == true)
            return;
        ItemTutorialDone = true;
        ShowPanel(ItemHintText);
    }
    //Plays Key Tutorial if not already played
    public void KeyTutorial()
    {
        if(KeyTutorialDone == true)
            return;
        KeyTutorialDone = true;
        ShowPanel(KeyHintText);
    }
    //Shows panel with hint text and pauses the game
    public void ShowPanel(string hintText)
    {
        panel.SetActive(true);
        HintText.text = hintText;
        Time.timeScale = 0f;
    }
}
