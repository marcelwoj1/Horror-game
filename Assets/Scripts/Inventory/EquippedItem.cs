using UnityEngine;

public class EquippedItem : MonoBehaviour
{
    public string ItemEquipped;

    public void SetItem(string item)
    {
        ItemEquipped = item;
        Debug.Log(ItemEquipped + " Equiped");
    }
}
