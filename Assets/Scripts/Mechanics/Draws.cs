using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles drawer interactions and item spawning logic.
/// </summary>
/// <remarks>
/// This system:
/// - Spawns predefined items or enemies from a drawer
/// - Uses a dictionary for flexible prefab lookup
/// - Ensures drawers can only be opened once
/// - Integrates with tutorial systems when in demo mode
/// - Plays audio feedback when opened
/// </remarks>
public class Draws : MonoBehaviour
{
    [Header("What will Spawn from Drawer")]

    /// <summary>Name of the object to spawn from the drawer.</summary>
    public string SpawnerName;

    [Header("Prefabs")]

    /// <summary>Prefab for spawning a rat enemy.</summary>
    public GameObject RatPrefab;

    /// <summary>Prefab for spawning bug spray.</summary>
    public GameObject BugSprayPrefab;

    /// <summary>Prefab for spawning orange juice.</summary>
    public GameObject OrangeJuicePrefab;

    /// <summary>Prefab for spawning a flashlight.</summary>
    public GameObject FlashlightPrefab;

    /// <summary>Prefab for spawning juicy morsels.</summary>
    public GameObject JuicyMorselPrefab;

    /// <summary>Prefab for spawning a key.</summary>
    public GameObject KeyPrefab;

    [Header("Variables")]

    /// <summary>Indicates whether the drawer has already been opened.</summary>
    public bool DrawerOpened = false;

    /// <summary>Indicates whether the current scene is the tutorial.</summary>
    public bool isTutorial;
    
    [Header("Components")]

    /// <summary>Reference to tutorial system.</summary>
    private IntroductionService introductionService;

    /// <summary>Dictionary mapping item names to prefabs.</summary>
    private Dictionary<string, GameObject> prefabDict;

    /// <summary>
    /// Initialises prefab dictionary and determines tutorial state.
    /// </summary>
    void Start()
    {
        // Creates a dictionary to store the prefabs
        prefabDict = new Dictionary<string, GameObject>
        {
            { "Rat", RatPrefab },
            { "BugSpray", BugSprayPrefab },
            { "OrangeJuice", OrangeJuicePrefab },
            { "Flashlight", FlashlightPrefab },
            { "JuicyMorsels", JuicyMorselPrefab },
            { "Key", KeyPrefab }
        };

        // Checks if the current scene is the tutorial
        if(SceneManager.GetActiveScene().name == "Demo")
        {
            isTutorial = true;
        }
    }

    /// <summary>
    /// Spawns an item or enemy from the drawer.
    /// </summary>
    /// <remarks>
    /// - Only executes once per drawer
    /// - Uses the SpawnerName to find the correct prefab
    /// - Plays sound feedback
    /// - Triggers tutorial hints if applicable
    /// </remarks>
    public void Spawn()
    {
        // Only spawns if drawer is not already opened
        if (DrawerOpened == false)
        {
            // Tries to spawn the item in the drawer
            if (prefabDict.TryGetValue(SpawnerName, out GameObject prefab))
            {
                // Spawns the item in the drawer
                Instantiate(prefab, transform.position + new Vector3(0, 0, 0), Quaternion.identity);
                DrawerOpened = true;

                // Plays the "DrawsOpen" sound at this drawer's position
                if (SoundService.Instance != null)
                {
                    SoundService.Instance.Play("DrawsOpen", (Vector2)transform.position);
                }
            }
            else
                Debug.LogWarning("No prefab found for: " + SpawnerName);
        }

        // Plays the drawer tutorial if the current scene is the tutorial
        if(isTutorial == true)
        {
            introductionService = GameObject.Find("IntroductionService").GetComponent<IntroductionService>();
            introductionService.DrawerTutorial();
        }
    }
}