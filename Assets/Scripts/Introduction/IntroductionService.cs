using UnityEngine;
using UnityEngine.UI;

public class IntroductionService : MonoBehaviour
{
    [Header("Tutorial Panel")]
    public GameObject panel;
    public Text HintText;

    [Header("Tutorials")]
    public bool DrawerTutorialDone;
    public bool RatTutorialDone;
    public bool InventoryTutorialDone;
    public bool ItemTutorialDone;

    public void Start()
    {
        panel.SetActive(false);
    }
    public void Update()
    {
        if (Input.GetMouseButtonDown(0) && panel.activeSelf)
        {
            panel.SetActive(false);
            Time.timeScale = 1f;
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
    public void RatTutorial()
    {
        if(RatTutorialDone == true)
            return;
        RatTutorialDone = true;
        panel.SetActive(true);
        HintText.text = "Rats are attracted to light, keep it away from them";
        Time.timeScale = 0f;
    }
    public void InventoryTutorial()
    {
        if(InventoryTutorialDone == true)
            return;
        InventoryTutorialDone = true;
        panel.SetActive(true);
        HintText.text = "Items picked up are stored here and can be equipped by dragging the icon of the item into the Equipped item slot. You can then close the inventory by clicking the inventory button again or clicking I.";
        Time.timeScale = 0f;
    }
    public void ItemTutorial()
    {
        if(ItemTutorialDone == true)
            return;
        ItemTutorialDone = true;
        panel.SetActive(true);
        HintText.text = "Once you have an item equipped you can use it by clicking the left mouse button.";
        Time.timeScale = 0f;
    }
}
