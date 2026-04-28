using UnityEngine;

/// <summary>
/// Handles player interaction with a crate used for hiding mechanics.
/// </summary>
/// <remarks>
/// This system:
/// - Detects when the player is near the crate
/// - Allows the player to hide or unhide using input
/// - Integrates with the Hiding and PlayerManager systems
///
/// Interaction is triggered when the player presses the E key
/// while inside the crate's trigger area.
/// </remarks>
public class Crate : MonoBehaviour
{
    [Header("Components")]

    /// <summary>Reference to the hiding system.</summary>
    private Hiding _hiding;

    /// <summary>Reference to the player manager.</summary>
    private PlayerManager _playerManager;
    
    [Header("Variables")]

    /// <summary>Indicates whether the player is within interaction range.</summary>
    public bool _isPlayerNear = false;

    /// <summary>
    /// Initialises required component references.
    /// </summary>
    void Start()
    {
        _playerManager = GameObject.Find("Player").GetComponent<PlayerManager>();
        _hiding = _playerManager.GetComponent<Hiding>();
    }

    /// <summary>
    /// Handles player input for hiding and unhiding.
    /// </summary>
    void Update()
    {
        // Checks if player is near the crate and presses E to hide/unhide
        if(Input.GetKeyDown(KeyCode.E) && _isPlayerNear == true)
        {
            if(_playerManager.IsHiding == false)
            {
                _hiding.Hide();
            }
            else
            {
                _hiding.UnHide();
            }
        }
    }

    /// <summary>
    /// Detects when the player enters the crate trigger area.
    /// </summary>
    /// <param name="collision">Collider of the entering object.</param>
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            _isPlayerNear = true;
        }
    }

    /// <summary>
    /// Detects when the player exits the crate trigger area.
    /// </summary>
    /// <param name="collision">Collider of the exiting object.</param>
    void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            _isPlayerNear = false;
        }
    }
}