using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles player teleportation, interaction prompts, and door unlocking logic.
/// </summary>
/// <remarks>
/// This system:
/// - Allows the player to teleport when within range and interacting
/// - Displays UI prompts based on proximity
/// - Supports door unlocking via key interaction
/// - Integrates with quest progression
/// - Optionally updates camera bounds after teleport
/// - Supports tutorial-specific behaviour
///
/// Includes debugging visualisation using Gizmos.
/// </remarks>
public class Teleport : MonoBehaviour
{
    [Header("Teleport Settings")]

    /// <summary>Destination transform where the player will be teleported.</summary>
    [Tooltip("Where the player will teleport to")]
    [SerializeField] private Transform destination;

    [Header("Interaction Settings")]

    /// <summary>Key used to trigger teleportation.</summary>
    [Tooltip("Key to press to teleport")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    
    /// <summary>Maximum distance required for interaction.</summary>
    [Tooltip("Distance at which the text appears and player can teleport")]
    [SerializeField] private float interactionDistance = 2f;

    [Header("Camera Bounds (Optional)")]

    /// <summary>Optional new camera bounds applied after teleport.</summary>
    [Tooltip("New camera bounds after teleport. Leave empty to keep current bounds.")]
    [SerializeField] private Transform newCameraBounds;

    [Header("Debug")]

    /// <summary>Enables gizmo visualisation in the editor.</summary>
    [SerializeField] private bool showGizmos = true;

    [Header("Prompt Text")]

    /// <summary>UI element shown when player can interact.</summary>
    public GameObject promptText;

    /// <summary>Reference to the player's transform when in range.</summary>
    private Transform playerTransform;

    /// <summary>Indicates whether the player is within interaction range.</summary>
    private bool playerInRange = false;

    /// <summary>Indicates whether the door is unlocked.</summary>
    public bool DoorUnlocked = false;

    [Header("Components")]

    /// <summary>Reference to player GameObject.</summary>
    private GameObject _player;

    /// <summary>Reference to lock GameObject.</summary>
    private GameObject _lock;

    /// <summary>Reference to quest system.</summary>
    private QuestService _questService;

    [Header("Variables")]

    /// <summary>Indicates if the current scene is the tutorial.</summary>
    private bool isTutorial = false;

    /// <summary>Indicates if this teleport is part of the outro sequence.</summary>
    public bool isOutro = false;

    /// <summary>
    /// Initialises references and determines scene context.
    /// </summary>
    private void Awake()
    {
        _player = GameObject.Find("Player");
        _lock = GameObject.Find("Lock");
        _questService = GameObject.Find("QuestService").GetComponent<QuestService>();

        if(SceneManager.GetActiveScene().name == "Demo")
        {
            isTutorial = true;
        }

        if (promptText != null)
        {
            promptText.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Updates interaction state, UI visibility, and teleport input.
    /// </summary>
    private void Update()
    {
        if (playerTransform != null)
        {
            float distance = Vector2.Distance(transform.position, playerTransform.position);
            playerInRange = distance <= interactionDistance;

            if (promptText != null)
            {
                promptText.gameObject.SetActive(playerInRange);
            }

            if (playerInRange && Input.GetKeyDown(interactKey) && DoorUnlocked)
            {
                TeleportPlayer(playerTransform);
            }
        }
        else
        {
            if (promptText != null && promptText.gameObject.activeSelf)
            {
                promptText.gameObject.SetActive(false);
            }
            playerInRange = false;
        }

        if(DoorUnlocked == false)
        {
            promptText.SetActive(false);
        }
        else
        {
            promptText.SetActive(true);
        }
    }

    /// <summary>
    /// Unlocks the door, allowing teleportation.
    /// </summary>
    public void UnlockDoor()
    {
        DoorUnlocked = true;
    }

    /// <summary>
    /// Handles unlocking the door when the player uses a key.
    /// </summary>
    public void NoKeyInDoor()
    {
        if (_player == null) return;

        float distance = Vector3.Distance(transform.position, _player.transform.position);
        bool inRange = distance <= interactionDistance;

        if (inRange)
        {
            Destroy(_lock);
            UnlockDoor();
            _questService.SatisfyQuest("DoorUnlock");
        }
    }

    /// <summary>
    /// Detects when the player enters the interaction trigger.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            playerTransform = other.transform;
        }
    }

    /// <summary>
    /// Detects when the player exits the interaction trigger.
    /// </summary>
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            playerTransform = null;
            playerInRange = false;

            if (promptText != null)
            {
                promptText.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Teleports the player to the destination and updates camera bounds.
    /// </summary>
    private void TeleportPlayer(Transform player)
    {
        if(isTutorial == true)
        {
            SceneManager.LoadScene("Intro");
        }

        if (destination == null)
        {
            Debug.LogWarning("Teleport destination is not set!", this);
            return;
        }

        Transform rootPlayer = player.root;
        rootPlayer.position = destination.position;

        if (newCameraBounds != null)
        {
            CameraTrack.SetBounds(newCameraBounds);
        }
    }

    /// <summary>
    /// Draws debug visualisation for interaction range and destination.
    /// </summary>
    private void OnDrawGizmos()
    {
        if (!showGizmos) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);

        if (destination != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, destination.position);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(destination.position, 0.5f);
            Gizmos.DrawLine(destination.position + Vector3.up * 0.5f, destination.position + Vector3.up * 1.5f);
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f);

            if (col is BoxCollider2D boxCol)
            {
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawCube(boxCol.offset, boxCol.size);
            }
            else if (col is CircleCollider2D circleCol)
            {
                Gizmos.DrawWireSphere(transform.position + (Vector3)circleCol.offset, circleCol.radius);
            }
        }
    }
}