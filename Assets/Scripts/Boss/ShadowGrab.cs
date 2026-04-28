using System.Collections;
using UnityEngine;

/// <summary>
/// Controls a grab attack where the boss lifts the player into the air
/// and temporarily disables their movement.
/// </summary>
/// <remarks>
/// Attack flow:
/// 1. A shadow hand is spawned above the player
/// 2. Player movement and gravity are disabled
/// 3. Player is lifted vertically to a fixed height
/// 4. Player must click to escape the grab
/// 5. Control and gravity are restored after release
///
/// This creates an interactive attack that requires player input to escape.
/// </remarks>
public class ShadowGrab : MonoBehaviour
{
    [Header("Variables")]

    /// <summary>Height the player is lifted to.</summary>
    public float liftHeight = 6f;

    /// <summary>Speed at which the player is lifted.</summary>
    public float liftSpeed = 5f;

    /// <summary>Indicates whether the system is waiting for player input.</summary>
    public bool waitingForClick = false;

    [Header("Hand Prefab")]

    /// <summary>Prefab used to visually represent the grabbing hand.</summary>
    public GameObject shadowHand;

    /// <summary>Instance of the spawned shadow hand.</summary>
    private GameObject _shadowHand;

    [Header("References")]

    /// <summary>Reference to the player transform.</summary>
    private Transform player;

    /// <summary>Rigidbody used to control player physics.</summary>
    private Rigidbody2D rb;

    /// <summary>Provides access to player movement control.</summary>
    private PlayerManager _playerManager;

    /// <summary>
    /// Initialises references to the player and required components.
    /// </summary>
    void Start()
    {
        rb = GameObject.Find("Player").GetComponent<Rigidbody2D>();
        _playerManager = GameObject.Find("Player").GetComponent<PlayerManager>();
        player = _playerManager.transform;
    }

    /// <summary>
    /// Starts the grab attack sequence.
    /// </summary>
    /// <remarks>
    /// Spawns a visual indicator (shadow hand) and begins lifting the player.
    /// </remarks>
    public void StartLiftAttack()
    {
        waitingForClick = true;

        // Spawn shadow hand above player
        _shadowHand = Instantiate(
            shadowHand,
            new Vector3(player.position.x, 29f, player.position.z),
            Quaternion.identity
        );

        StartCoroutine(LiftRoutine());
    }

    /// <summary>
    /// Handles lifting the player and waiting for input to release them.
    /// </summary>
    /// <returns>Coroutine controlling the full grab sequence.</returns>
    /// <remarks>
    /// During this routine:
    /// - Player movement is disabled
    /// - Gravity is removed
    /// - Player is lifted to a fixed position
    /// - The system waits for player input to escape
    /// </remarks>
    IEnumerator LiftRoutine()
    {
        // Disable player movement and gravity
        _playerManager.AllowMovement = false;
        rb.gravityScale = 0f;

        // Define target height
        Vector3 targetPos = new Vector3(
            player.position.x,
            30f,
            player.position.z
        );

        // Lift player upward
        while (Vector3.Distance(player.position, targetPos) > 0.05f)
        {
            player.position = Vector3.MoveTowards(
                player.position,
                targetPos,
                liftSpeed * Time.deltaTime
            );

            yield return null;
        }

        // Wait for player input to escape
        waitingForClick = true;

        while (waitingForClick)
        {
            if (Input.GetMouseButtonDown(0))
            {
                waitingForClick = false;
            }

            yield return null;
        }

        // Restore player control and gravity
        rb.gravityScale = 2f;
        _playerManager.AllowMovement = true;

        // Remove visual effect
        Destroy(_shadowHand);
    }
}