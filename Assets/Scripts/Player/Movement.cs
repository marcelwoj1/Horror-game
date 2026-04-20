using System.Runtime.CompilerServices;
using NUnit.Framework;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class Movement : MonoBehaviour
{
    [Header("Variables")]
    private float _JUMP_THRESHOLD = 0f;
    private float _FALL_THRESHOLD = 0f;
    private float _MOVE_THRESHOLD = 0f;
    private float _GROUND_CHECK_RADIUS = 0.15f;
    private float _JUMP_BUFFER = 0.5f;
    public bool AxeEquipped = false;
    public bool FlashlightEquipped = false;
    public bool isKnockedBack = false;

    [Header("Components")]
    public GameObject Inventory;
    private Rigidbody2D _rigidBody;
    private Transform _feetLocation;
    private SpriteAnimator _animator;
    public SpriteRenderer _spriteRenderer;
    private PlayerManager _playerManager;

    [Header("Config")]
    public float _moveSpeed = 6;
    public float _jumpPower = 30;
    [SerializeField] LayerMask _groundLayer;

    [Header("Input")]
    private Vector2 _movementDirection;
    private float _lastJumpInput = -100f;
    
    public enum MoveStates
    {
        Idle,
        Moving,
    }
    public enum AirStates
    {
        Jumping,
        Falling,
        Grounded
    }
    public MoveStates MoveState;
    public AirStates AirState;

    public bool IsGrounded()
    {
        return Physics2D.OverlapCircle( new Vector2(_feetLocation.position.x, _feetLocation.position.y) , _GROUND_CHECK_RADIUS, _groundLayer);
    }


    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rigidBody = GetComponent<Rigidbody2D>();
        _feetLocation = transform.Find("FeetLocation").transform;
        _animator = GetComponent<SpriteAnimator>();
        _playerManager = GetComponent<PlayerManager>();
    }

    
    void Update()
    {
        // Movement
        _movementDirection = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );

        // Jump
        if (Input.GetButton("Jump"))
        {
            _lastJumpInput = Time.time;
        }

        // State Machine when grounded
        if ( Mathf.Abs(_rigidBody.linearVelocityX) > _MOVE_THRESHOLD)
        {
            MoveState = MoveStates.Moving;
        }
        else
        {
           MoveState = MoveStates.Idle; 
        }

        // State Machine when in air
        if ( _rigidBody.linearVelocityY > _JUMP_THRESHOLD)
        {
            AirState = AirStates.Jumping;
        }
        else if ( _rigidBody.linearVelocityY < _FALL_THRESHOLD)
        {
            AirState = AirStates.Falling;
        }
        else if (IsGrounded())
        {
            AirState = AirStates.Grounded;
        }


        // ANIMATION & FLIPPING
        if (_animator != null)
        {
            // Handle Flipping
            if (_movementDirection.x > 0) 
            {
                _animator.SetFlip(true);
            }
            else if (_movementDirection.x < 0) 
            {
                _animator.SetFlip(false);
            }

            // Handle States
            if (AirState != AirStates.Grounded)
            {
                if (AirState == AirStates.Jumping) _animator.Play("Jump");
                else if (AirState == AirStates.Falling) _animator.Play("Fall");
            }
            else
            {
                if (MoveState == MoveStates.Moving)
                {
                    // Walk Animations
                    if(FlashlightEquipped == true)
                    {
                        _animator.Play("TorchWalk");
                    }
                    else
                    {
                        _animator.Play("Walk");
                    }
                }
                else
                {
                    if(_playerManager.IsCrouching == true)
                    {
                        _animator.Play("Crouch");
                    }
                    // Idle Animations
                    else if(AxeEquipped == true)
                    {
                        _animator.Play("AxeIdle");
                    }
                    else if(FlashlightEquipped == true)
                    {
                        _animator.Play("TorchIdle");
                    }
                    else
                    {
                        _animator.Play("Idle");
                    }
                }
            }
        }

        if(_playerManager.AllowMovement == false)
        {
            _rigidBody.linearVelocity = Vector2.zero;
        }
    }


    void FixedUpdate()
    {

        if(_playerManager.AllowMovement == true)
        {
            if (!isKnockedBack)
            {
                // Move
                _rigidBody.linearVelocityX = _movementDirection.x * _moveSpeed;

                // Jump
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
}
