using UnityEngine;
using UnityEngine.UI;

public class IntroductionService : MonoBehaviour
{
    [Header("Tutorial Panel")]
    public GameObject panel;
    public Text HintText;

    [Header("Tutorials")]
    public bool StartTutorialDone;
    public bool MovementTutorialDone;
    public bool DrawerTutorialDone;
    public bool RatTutorialDone;
    public bool InventoryTutorialDone;
    public bool ItemTutorialDone;
    public bool KeyTutorialDone;

    void Start()
    {
        StartTutorialDone = true;
        panel.SetActive(true);
        HintText.text = "Welcome to the Domatophobia! This is a tutorial level to show you the basics of the game. Click the left mouse button to continue.";
    }
    public void Update()
    {
        if (Input.GetMouseButtonDown(0) && panel.activeSelf)
        {
            panel.SetActive(false);
            Time.timeScale = 1f;
            if(MovementTutorialDone == false)
            {
                MovementTutorialDone = true;
                panel.SetActive(true);
                HintText.text = "You can use the A and D keys to move left and right, and the space bar to jump. You can also use C to crouch if your feeling extra sneeky(maybe when trying to take something off someone).";
                Time.timeScale = 0f;
            }
        }
    }

    public void DrawerTutorial()
    {
        if(DrawerTutorialDone == true)
            return;
        DrawerTutorialDone = true;
        panel.SetActive(true);
        HintText.text = "Items dropped from drawers can be picked up by clicking E while stood over them. Once picked up they will be stored in your inventory which you can check by clicking the inventory button or I.";
        Time.timeScale = 0f;
    }
    public void InventoryTutorial()
    {
        if(InventoryTutorialDone == true)
            return;
        InventoryTutorialDone = true;
        panel.SetActive(true);
        HintText.text = "Items picked up are stored here- with what they are and what they do detailed- and can be equipped by dragging the icon of the item into the Equipped item slot. You can then close the inventory by clicking the inventory button again or clicking I.";
        Time.timeScale = 0f;
    }
    public void ItemTutorial()
    {
        if(ItemTutorialDone == true)
            return;
        ItemTutorialDone = true;
        panel.SetActive(true);
        HintText.text = "Once you have an item equipped you can use it by clicking the left mouse button. Different items have different uses. You can also drop items by clicking the Q key. Items dont stack do watch your inventory space.";
        Time.timeScale = 0f;
    }
    public void KeyTutorial()
    {
        if(KeyTutorialDone == true)
            return;
        KeyTutorialDone = true;
        panel.SetActive(true);
        HintText.text = "Some doors are locked and require a key to open. You will find keys in different places such as drawers or maybe attached to enemies. Use this key to open the door in front of you and begin the game... good luck.";
        Time.timeScale = 0f;
    }
}
