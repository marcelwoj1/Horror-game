using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Draws : MonoBehaviour
{
    [Header("What will Spawn from Drawer")]
    public string SpawnerName;

    [Header("Prefabs")]
    public GameObject RatPrefab;
    public GameObject BugSprayPrefab;
    public GameObject OrangeJuicePrefab;
    public GameObject FlashlightPrefab;
    public GameObject JuicyMorselPrefab;
    public GameObject KeyPrefab;

    [Header("Variables")]
    public bool DrawerOpened = false;
    public bool isTutorial;
    
    [Header("Components")]
    private IntroductionService introductionService;
    private Dictionary<string, GameObject> prefabDict;

    void Start()
    {
        //Creates a dictionary to store the prefabs
        prefabDict = new Dictionary<string, GameObject>
        {
            { "Rat", RatPrefab },
            { "BugSpray", BugSprayPrefab },
            { "OrangeJuice", OrangeJuicePrefab },
            { "Flashlight", FlashlightPrefab },
            { "JuicyMorsels", JuicyMorselPrefab },
            { "Key", KeyPrefab }
        };
        //Checks if the current scene is the tutorial
        if(SceneManager.GetActiveScene().name == "Demo")
        {
            isTutorial = true;
        }
    }

    //Spawns item in drawer
    public void Spawn()
    {
        //Only spawns if drawer is not already opened
        if (DrawerOpened == false)
        {
            //Tries to spawn the item in the drawer
            if (prefabDict.TryGetValue(SpawnerName, out GameObject prefab))
            {
                //Spawns the item in the drawer
                Instantiate(prefab, transform.position + new Vector3(0, 0, 0), Quaternion.identity);
                DrawerOpened = true;

                //Plays the "DrawsOpen" sound at this drawer's position
                if (SoundService.Instance != null)
                {
                    SoundService.Instance.Play("DrawsOpen", (Vector2)transform.position);
                }
            }
            else
                Debug.LogWarning("No prefab found for: " + SpawnerName);
        }
        //Plays the drawer tutorial if the current scene is the tutorial
        if(isTutorial == true)
        {
            introductionService = GameObject.Find("IntroductionService").GetComponent<IntroductionService>();
            introductionService.DrawerTutorial();
        }
    }


}
