using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Provides a generic interaction system for game objects.
/// </summary>
/// <remarks>
/// This system:
/// - Detects when the player is within interaction range
/// - Displays an interaction prompt
/// - Handles input for triggering interactions
/// - Invokes UnityEvents for flexible, designer-driven behaviour
///
/// Supports single-use interactions and visual debugging via Gizmos.
/// </remarks>
public class Interactable : MonoBehaviour
{
    [Header("Settings")]

    /// <summary>Maximum distance at which the player can interact.</summary>
    [SerializeField] private float interactionDistance = 2.5f;

    /// <summary>Key used to trigger interaction.</summary>
    [SerializeField] private KeyCode interactionKey = KeyCode.E;

    /// <summary>Determines whether the interaction can only occur once.</summary>
    [SerializeField] private bool singleUse = true;

    [Header("Events")]

    /// <summary>Event invoked when the player interacts with this object.</summary>
    public UnityEvent OnInteract;

    /// <summary>Reference to the player object.</summary>
    private GameObject _player;

    /// <summary>UI element shown when interaction is possible.</summary>
    private GameObject _interactionImage;

    /// <summary>Indicates whether the player is currently in range.</summary>
    private bool _isPlayerInRange;

    /// <summary>Tracks whether the interaction has already been used.</summary>
    private bool _interactionUsed;

    /// <summary>
    /// Initialises references and interaction UI.
    /// </summary>
    private void Awake()
    {
        _player = GameObject.Find("Player");

        // Finds the Interact image on the gameobject
        Transform imageTransform = transform.Find("Image");
        if (imageTransform != null)
        {
            _interactionImage = imageTransform.gameObject;
            _interactionImage.SetActive(false);
        }
    }

    /// <summary>
    /// Updates interaction state and handles player input.
    /// </summary>
    private void Update()
    {
        if (_player == null) return;

        // Check distance to player
        float distance = Vector3.Distance(transform.position, _player.transform.position);
        bool inRange = distance <= interactionDistance;

        // Update UI prompt visibility
        if (inRange != _isPlayerInRange)
        {
            _isPlayerInRange = inRange;
            if (_interactionImage != null && _interactionUsed == false)
            {
                _interactionImage.SetActive(_isPlayerInRange);
            }
        }

        // Handle interaction input
        if (_isPlayerInRange && Input.GetKeyDown(interactionKey))
        {
            Interact();
        }
    }

    /// <summary>
    /// Executes the interaction logic and triggers associated events.
    /// </summary>
    public void Interact()
    {
        // If it's single use and already triggered, don't do anything
        if (singleUse && _interactionUsed) return;

        OnInteract?.Invoke();

        if (singleUse)
        {
            _interactionUsed = true;
            if (_interactionImage != null)
            {
                _interactionImage.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Draws a visual representation of the interaction range in the editor.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        // Visualize interaction range in the editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}