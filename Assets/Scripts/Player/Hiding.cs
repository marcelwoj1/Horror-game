using UnityEngine;
using System.Collections;

/// <summary>
/// Controls the player's hiding mechanic, allowing them to avoid enemy detection.
/// </summary>
/// <remarks>
/// When hiding:
/// - Player movement is disabled
/// - Collisions with enemies are ignored
/// - Rendering layer is changed to simulate being behind objects
/// - Player position is adjusted to appear inside the hiding object
///
/// When exiting hiding:
/// - Movement and collisions are restored
/// - Rendering layer returns to normal
/// </remarks>
public class Hiding : MonoBehaviour
{
    [Header("Variables")]

    /// <summary>Coroutine used to manage delayed hiding behaviour.</summary>
    private Coroutine hideCoroutine;

    [Header("Components")]

    /// <summary>Reference to movement system.</summary>
    private Movement _movement;

    /// <summary>Reference to player state manager.</summary>
    private PlayerManager _playerManager;

    /// <summary>Controls sprite rendering layer.</summary>
    private SpriteRenderer _spriteRenderer;

    /// <summary>Rigidbody used for physics control.</summary>
    private Rigidbody2D _rigidBody;

    /// <summary>Collider used for player collision.</summary>
    private CapsuleCollider2D _capsuleCollider;

    /// <summary>
    /// Reference to the vignette material.
    /// </summary>
    public Material _vignetteMaterial;

    /// <summary>
    /// Reference to the vignette falloff.
    /// </summary>
    private float _fallOff;

    /// <summary>
    /// Initialises component references.
    /// </summary>
    public void Start()
    {
        _movement = GetComponent<Movement>();
        _playerManager = GetComponent<PlayerManager>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rigidBody = GetComponent<Rigidbody2D>();
        _capsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    /// <summary>
    /// Activates hiding behaviour.
    /// </summary>
    /// <remarks>
    /// Hiding can only occur when:
    /// - The player is not already hiding
    /// - The player is grounded
    ///
    /// This prevents unintended behaviour during jumps or repeated calls.
    /// </remarks>
    public void Hide()
    {
        if (_playerManager.IsHiding)
            return;

        if (!_movement.IsGrounded())
            return;

        _playerManager.IsHiding = true;

        _fallOff = 4f;
        _vignetteMaterial.SetFloat("_Falloff", _fallOff);
        _vignetteMaterial.color = Color.black;

        // Move player behind objects visually
        _spriteRenderer.sortingLayerName = "Wall";

        // Stop movement
        _rigidBody.linearVelocity = Vector2.zero;
        _playerManager.AllowMovement = false;

        // Disable collisions with enemies
        Physics2D.IgnoreLayerCollision(
            LayerMask.NameToLayer("Player"),
            LayerMask.NameToLayer("Janitor"),
            true
        );

        Physics2D.IgnoreLayerCollision(
            LayerMask.NameToLayer("Player"),
            LayerMask.NameToLayer("Enemy Hitbox"),
            true
        );

        // Restart coroutine safely
        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);

        hideCoroutine = StartCoroutine(WaitUntilGrounded());
    }

    /// <summary>
    /// Deactivates hiding behaviour and restores normal player state.
    /// </summary>
    public void UnHide()
    {
        _playerManager.IsHiding = false;

        _fallOff = 0f;
        _vignetteMaterial.SetFloat("_Falloff", _fallOff);

        // Restore movement
        _playerManager.AllowMovement = true;

        // Restore rendering layer
        _spriteRenderer.sortingLayerName = "Player";

        // Restore gravity
        _rigidBody.gravityScale = 2;

        // Re-enable collisions
        _capsuleCollider.enabled = true;

        Physics2D.IgnoreLayerCollision(
            LayerMask.NameToLayer("Player"),
            LayerMask.NameToLayer("Janitor"),
            false
        );

        Physics2D.IgnoreLayerCollision(
            LayerMask.NameToLayer("Player"),
            LayerMask.NameToLayer("Enemy Hitbox"),
            false
        );
    }

    /// <summary>
    /// Waits until the player is grounded before finalising the hiding position.
    /// </summary>
    /// <returns>Coroutine controlling delayed positioning.</returns>
    /// <remarks>
    /// Ensures the player is properly positioned inside the hiding object.
    /// Prevents incorrect placement if hiding is triggered mid-air.
    /// </remarks>
    IEnumerator WaitUntilGrounded()
    {
        yield return new WaitUntil(
            () => _movement.IsGrounded() || !_playerManager.IsHiding
        );

        if (!_playerManager.IsHiding)
            yield break;

        // Adjust position to simulate being inside the object
        _rigidBody.gravityScale = 0;

        transform.position = new Vector3(
            transform.position.x,
            transform.position.y + 1.4f,
            transform.position.z
        );
    }
}