using System.Runtime.CompilerServices;
using NUnit.Framework;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles player movement, input processing, physics interactions,
/// and animation state management.
/// </summary>
/// <remarks>
/// This script implements:
/// - Physics-based horizontal movement and jumping
/// - Input buffering for responsive jumping
/// - Ground detection using collision checks
/// - State machines for movement and air behaviour
/// - Animation control based on player state and equipped items
///
/// It serves as the core system for player locomotion.
/// </remarks>
public class Movement : MonoBehaviour
{
    [Header("Variables")]

    /// <summary>Threshold for detecting upward movement.</summary>
    private float _JUMP_THRESHOLD = 0f;

    /// <summary>Threshold for detecting falling movement.</summary>
    private float _FALL_THRESHOLD = 0f;

    /// <summary>Threshold for detecting horizontal movement.</summary>
    private float _MOVE_THRESHOLD = 0f;

    /// <summary>Radius used for ground detection.</summary>
    private float _GROUND_CHECK_RADIUS = 0.15f;

    /// <summary>Time window allowing buffered jump input.</summary>
    private float _JUMP_BUFFER = 0.5f;

    /// <summary>Indicates if the axe is equipped.</summary>
    public bool AxeEquipped = false;

    /// <summary>Indicates if the flashlight is equipped.</summary>
    public bool FlashlightEquipped = false;

    /// <summary>Indicates if the player is currently knocked back.</summary>
    public bool isKnockedBack = false;

    [Header("Components")]

    /// <summary>Inventory UI reference.</summary>
    public GameObject Inventory;

    /// <summary>Rigidbody used for physics movement.</summary>
    private Rigidbody2D _rigidBody;

    /// <summary>Transform used for ground detection.</summary>
    private Transform _feetLocation;

    /// <summary>Handles animation playback.</summary>
    private SpriteAnimator _animator;

    /// <summary>Controls sprite orientation.</summary>
    public SpriteRenderer _spriteRenderer;

    /// <summary>Reference to player manager for state control.</summary>
    private PlayerManager _playerManager;

    
    public Player_IK _playerIK;
    public SpriteRenderer HeadSpriteRenderer;
    public SpriteRenderer UpperTorsoSpriteRenderer;
    public SpriteRenderer LowerTorsoSpriteRenderer;

    [Header("Config")]

    /// <summary>Horizontal movement speed.</summary>
    public float _moveSpeed = 6;

    /// <summary>Jump force applied to the player.</summary>
    public float _jumpPower = 30;

    /// <summary>Layer mask used for ground detection.</summary>
    [SerializeField] LayerMask _groundLayer;

    [Header("Input")]

    /// <summary>Current movement input direction.</summary>
    private Vector2 _movementDirection;

    /// <summary>Timestamp of last jump input.</summary>
    private float _lastJumpInput = -100f;

    /// <summary>
    /// Defines horizontal movement states.
    /// </summary>
    public enum MoveStates
    {
        /// <summary>Player is stationary.</summary>
        Idle,

        /// <summary>Player is moving horizontally.</summary>
        Moving,
    }

    /// <summary>
    /// Defines vertical movement states.
    /// </summary>
    public enum AirStates
    {
        /// <summary>Player is moving upward.</summary>
        Jumping,

        /// <summary>Player is falling downward.</summary>
        Falling,

        /// <summary>Player is grounded.</summary>
        Grounded
    }

    /// <summary>Current horizontal movement state.</summary>
    public MoveStates MoveState;

    /// <summary>Current vertical movement state.</summary>
    public AirStates AirState;

    /// <summary>
    /// Checks if the player is grounded using a circular overlap check.
    /// </summary>
    /// <returns>True if grounded, otherwise false.</returns>
    public bool IsGrounded()
    {
        return Physics2D.OverlapCircle(
            new Vector2(_feetLocation.position.x, _feetLocation.position.y),
            _GROUND_CHECK_RADIUS,
            _groundLayer
        );
    }

    /// <summary>
    /// Initialises component references.
    /// </summary>
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rigidBody = GetComponent<Rigidbody2D>();
        _feetLocation = transform.Find("FeetLocation").transform;
        _animator = GetComponent<SpriteAnimator>();
        _playerManager = GetComponent<PlayerManager>();

        if (_playerIK == null)
        {
            _playerIK = GetComponent<Player_IK>();
        }

        if (_playerIK != null && _playerIK.Player_IK_Rig != null)
        {
            Transform[] children = _playerIK.Player_IK_Rig.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in children)
            {
                if (child.name == "Head" && HeadSpriteRenderer == null) HeadSpriteRenderer = child.GetComponent<SpriteRenderer>();
                else if (child.name == "UpperTorso" && UpperTorsoSpriteRenderer == null) UpperTorsoSpriteRenderer = child.GetComponent<SpriteRenderer>();
                else if (child.name == "LowerTorso" && LowerTorsoSpriteRenderer == null) LowerTorsoSpriteRenderer = child.GetComponent<SpriteRenderer>();
            }
        }
    }

    /// <summary>
    /// Handles input processing, state updates, and animation logic.
    /// </summary>
    /// <remarks>
    /// Updates movement and air states based on velocity,
    /// and selects appropriate animations depending on state and equipment.
    /// </remarks>
    void Update()
    {
        // Input handling
        _movementDirection = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );

        if (Input.GetButton("Jump"))
        {
            _lastJumpInput = Time.time;
        }

        // Movement state
        MoveState = Mathf.Abs(_rigidBody.linearVelocityX) > _MOVE_THRESHOLD
            ? MoveStates.Moving
            : MoveStates.Idle;

        // Air state
        if (_rigidBody.linearVelocityY > _JUMP_THRESHOLD)
            AirState = AirStates.Jumping;
        else if (_rigidBody.linearVelocityY < _FALL_THRESHOLD)
            AirState = AirStates.Falling;
        else if (IsGrounded())
            AirState = AirStates.Grounded;

        // Animation and sprite orientation
        if (_movementDirection.x > 0)
        {
            if (_animator != null) _animator.SetFlip(true);
            SetIKSpriteFlip(true);
        }
        else if (_movementDirection.x < 0)
        {
            if (_animator != null) _animator.SetFlip(false);
            SetIKSpriteFlip(false);
        }

        if (_animator != null)
        {
            if (AirState != AirStates.Grounded)
            {
                if (AirState == AirStates.Jumping) _animator.Play("Jump");
                else if (AirState == AirStates.Falling) _animator.Play("Fall");
            }
            else
            {
                if (MoveState == MoveStates.Moving)
                {
                    _animator.Play(FlashlightEquipped ? "TorchWalk" : "Walk");
                }
                else
                {
                    if (_playerManager.IsCrouching)
                        _animator.Play("Crouch");
                    else if (AxeEquipped)
                        _animator.Play("AxeIdle");
                    else if (FlashlightEquipped)
                        _animator.Play("TorchIdle");
                    else
                        _animator.Play("Idle");
                }
            }
        }

        // Stop movement if disabled
        if (!_playerManager.AllowMovement)
        {
            _rigidBody.linearVelocity = Vector2.zero;
        }
    }

    /// <summary>
    /// Applies physics-based movement and jump logic.
    /// </summary>
    /// <remarks>
    /// Uses FixedUpdate for consistent physics updates.
    /// Includes jump buffering to improve responsiveness.
    /// </remarks>
    void FixedUpdate()
    {
        if (_playerManager.AllowMovement)
        {
            if (!isKnockedBack)
            {
                _rigidBody.linearVelocityX = _movementDirection.x * _moveSpeed;

                if (Time.time - _lastJumpInput <= _JUMP_BUFFER && IsGrounded())
                {
                    _lastJumpInput = -100f;
                    _rigidBody.linearVelocityY = 0;
                    _rigidBody.AddForce(Vector2.up * _jumpPower, ForceMode2D.Impulse);
                }
            }
        }
        else
        {
            _rigidBody.linearVelocity = Vector2.zero;
        }
    }

    private void SetIKSpriteFlip(bool flipX)
    {
        if (HeadSpriteRenderer != null) HeadSpriteRenderer.flipX = flipX;
        if (UpperTorsoSpriteRenderer != null) UpperTorsoSpriteRenderer.flipX = flipX;
        if (LowerTorsoSpriteRenderer != null) LowerTorsoSpriteRenderer.flipX = flipX;
    }
}