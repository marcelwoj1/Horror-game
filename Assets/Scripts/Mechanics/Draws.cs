using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Draws : MonoBehaviour
{
    public string SpawnerName;
    public GameObject RatPrefab;
    public GameObject BugSprayPrefab;
    public GameObject OrangeJuicePrefab;
    public GameObject FlashlightPrefab;
    public GameObject JuicyMorselPrefab;
    public GameObject KeyPrefab;
    public bool DrawerOpened = false;

    public bool isTutorial;
    private IntroductionService introductionService;

    private Dictionary<string, GameObject> prefabDict;

    void Start()
    {
        prefabDict = new Dictionary<string, GameObject>
        {
            { "Rat", RatPrefab },
            { "BugSpray", BugSprayPrefab },
            { "OrangeJuice", OrangeJuicePrefab },
            { "Flashlight", FlashlightPrefab },
            { "JuicyMorsels", JuicyMorselPrefab },
            { "Key", KeyPrefab }
        };
        if(SceneManager.GetActiveScene().name == "Demo")
        {
            isTutorial = true;
        }
    }


    public void Spawn()
    {
        if (DrawerOpened == false)
        {
            if (prefabDict.TryGetValue(SpawnerName, out GameObject prefab))
            {
                Instantiate(prefab, transform.position + new Vector3(0, 0, 0), Quaternion.identity);
                DrawerOpened = true;

                // Play the "DrawsOpen" sound at this drawer's position
                if (SoundService.Instance != null)
                {
                    SoundService.Instance.Play("DrawsOpen", (Vector2)transform.position);
                }
            }
            else
                Debug.LogWarning("No prefab found for: " + SpawnerName);
        }
        if(isTutorial == true)
        {
            introductionService = GameObject.Find("IntroductionService").GetComponent<IntroductionService>();
            introductionService.DrawerTutorial();
        }
    }


}
