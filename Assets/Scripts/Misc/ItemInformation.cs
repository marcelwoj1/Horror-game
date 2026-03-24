using UnityEngine;
using TMPro;

public class ItemInformation : MonoBehaviour
{
    public string itemName;
    public TextMeshProUGUI itemInfoText;
    public GameObject itemInfoPanel;
    private string BugSprayInfo = "Use the bug spray to hide yourself from all spiders for a Limited time!";
    private string FlashlightInfo = "Use the flashlight to see in the dark!";
    private string JuicyMorselInfo = "Use the juicy morsel to heal yourself!";
    private string KeyInfo = "Use the key to open the door!";
    private string AxeInfo = "Use the axe to swing at enemys or wooden planks!";

    public void GetItemInfo(string itemName)
    {
        switch (itemName)
        {
            case "BugSpray":
                itemInfoText.text = BugSprayInfo;
                break;
            case "Flashlight":
                itemInfoText.text = FlashlightInfo;
                break;
            case "JuicyMorsel":
                itemInfoText.text = JuicyMorselInfo;
                break;
            case "Key":
                itemInfoText.text = KeyInfo;
                break;
            case "Axe":
                itemInfoText.text = AxeInfo;
                break;
            default:
                itemInfoText.text = "";
                break;
        }
        itemInfoPanel.SetActive(true);
    }

    public void HideItemInfo()
    {
        itemInfoPanel.SetActive(false);
    }
}
