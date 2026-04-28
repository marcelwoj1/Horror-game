using UnityEngine;

/// <summary>
/// Controls the behaviour and positioning of the player's torch.
/// </summary>
/// <remarks>
/// This script manages:
/// - Enabling/disabling the torch based on player state
/// - Updating the torch's position relative to movement
/// - Adjusting rotation and orientation based on player direction
///
/// The torch provides visual feedback and is tied to player actions
/// such as movement, grounding, and hiding.
/// </remarks>
public class Torch : MonoBehaviour
{
    [Header("Components")]

    /// <summary>Reference to the torch light GameObject.</summary>
    public GameObject TorchLight;

    /// <summary>Reference to player movement system.</summary>
    public Movement _movement;

    /// <summary>Provides access to player state (e.g., hiding).</summary>
    private PlayerManager _playerManager;

    /// <summary>Tracks equipped item state.</summary>
    private EquippedItem _equippedItem;
    
    [Header("Variables")]

    /// <summary>Horizontal offset of the torch.</summary>
    private float xpos;

    /// <summary>Vertical offset of the torch.</summary>
    private float ypos;

    /// <summary>
    /// Initialises required component references.
    /// </summary>
    void Start()
    {
        _playerManager = GameObject.Find("Player").GetComponent<PlayerManager>();
        _equippedItem = GameObject.Find("Player").GetComponent<EquippedItem>();
    }

    /// <summary>
    /// Updates torch state, position, and rotation each frame.
    /// </summary>
    /// <remarks>
    /// The torch:
    /// - Turns off when the player is airborne or hiding
    /// - Adjusts position based on movement state
    /// - Flips orientation depending on player facing direction
    /// </remarks>
    void Update()
    {
        // Disable torch if player is not grounded or is hiding
        if (_movement.AirState != Movement.AirStates.Grounded || _playerManager.IsHiding)
        {
            TorchLight.SetActive(false);
            _equippedItem.TorchIsLit = false;
        }

        // Determine base offsets from current local position
        ypos = Mathf.Abs(TorchLight.transform.localPosition.y);
        xpos = Mathf.Abs(TorchLight.transform.localPosition.x);

        // Adjust offsets based on movement state
        if (_movement.MoveState == Movement.MoveStates.Moving)
        {
            xpos = 1.1f;
            ypos = 0.2f;
        }
        else
        {
            xpos = 0.91f;
            ypos = -0.26f;
        }

        if (TorchLight != null)
        {
            // Adjust position and rotation based on facing direction
            if (_movement._spriteRenderer.flipX) // Facing right
            {
                TorchLight.transform.localPosition =
                    new Vector3(xpos, ypos, TorchLight.transform.localPosition.z);

                TorchLight.transform.localRotation =
                    Quaternion.Euler(0, 0, -68);
            }
            else // Facing left
            {
                TorchLight.transform.localPosition =
                    new Vector3(-xpos, ypos, TorchLight.transform.localPosition.z);

                TorchLight.transform.localRotation =
                    Quaternion.Euler(0, 0, 68);
            }
        }
    }
}