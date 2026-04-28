using UnityEngine;

/// <summary>
/// Controls door behaviour, including unlocking and plank destruction.
/// </summary>
/// <remarks>
/// This system:
/// - Unlocks the door when all planks are removed
/// - Triggers quest progression upon unlocking
/// - Allows planks to be destroyed via player interaction (e.g., axe)
/// </remarks>
public class Doors : MonoBehaviour
{
    [Header("Components")]

    /// <summary>Reference to teleport system controlling door access.</summary>
    public Teleport teleport;

    [Header("Variables")]

    /// <summary>Reference to quest system.</summary>
    private QuestService _questService;
    
    /// <summary>
    /// Initialises required component references.
    /// </summary>
    void Start()
    {
        _questService = GameObject.Find("QuestService").GetComponent<QuestService>();
    }

    /// <summary>
    /// Checks if the door is unlocked and updates quest state.
    /// </summary>
    /// <remarks>
    /// When all planks are removed (no child objects remain),
    /// the door is unlocked and the corresponding quest is completed.
    /// </remarks>
    void Update()
    {
        if(transform.childCount == 0)
        {
            teleport.UnlockDoor();
            _questService.SatisfyQuest("Door");
        }
    }

    /// <summary>
    /// Breaks a plank attached to the door.
    /// </summary>
    /// <remarks>
    /// Removes the first child object, representing a plank,
    /// if one exists.
    /// </remarks>
    public void breakPlank()
    {
        // Only breaks the plank if there is a plank on the door
        if(transform.childCount == 0)
            return;

        Destroy(transform.GetChild(0).gameObject);
    }
}