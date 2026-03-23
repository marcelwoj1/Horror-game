using UnityEngine;
using System.Collections.Generic;

public class Draws : MonoBehaviour
{
    public string SpawnerName;
    public GameObject RatPrefab;
    public GameObject BugSprayPrefab;
    public GameObject OrangeJuicePrefab;
    public GameObject FlashlightPrefab;
    public GameObject JuicyMorselPrefab;

    public bool DrawerOpened = false;

    private Dictionary<string, GameObject> prefabDict;

    void Start()
    {
        prefabDict = new Dictionary<string, GameObject>
        {
            { "Rat", RatPrefab },
            { "BugSpray", BugSprayPrefab },
            { "OrangeJuice", OrangeJuicePrefab },
            { "Flashlight", FlashlightPrefab },
            { "JuicyMorsel", JuicyMorselPrefab }
        };
    }


    public void Spawn()
    {
        if (DrawerOpened == false)
        {
            if (prefabDict.TryGetValue(SpawnerName, out GameObject prefab))
            {
                Instantiate(prefab, transform.position, Quaternion.identity);
                DrawerOpened = true;
            }
            else
                Debug.LogWarning("No prefab found for: " + SpawnerName);
        }
    }


}
