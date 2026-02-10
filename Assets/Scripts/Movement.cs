using System.Runtime.CompilerServices;
using NUnit.Framework;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;



public class Movement : MonoBehaviour
{
    

    // | CONSTANTS |
    private float _JUMP_THRESHOLD = 0f;
    private float _FALL_THRESHOLD = 0f;
    private float _MOVE_THRESHOLD = 0f;
    private float _GROUND_CHECK_RADIUS = 0.15f;
    private float _JUMP_BUFFER = 0.5f;
    private bool IsLit;
    private bool IsInventoryOpen;



    // | VARIABLES |

    // Components
    public GameObject TorchLight;
    public GameObject Inventory;
    private Rigidbody2D _rigidBody;
    private Transform _feetLocation;
    private SpriteAnimator _animator;
    public SpriteRenderer _spriteRenderer;


    // Config
    public float _moveSpeed = 5;
    public float _jumpPower = 30;
    [SerializeField] LayerMask _groundLayer;


    // Input
    private Vector2 _movementDirection;
    private float _lastJumpInput = -100f;
    
    

    // State machine
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




    // Methods
    bool IsGrounded()
    {
        return Physics2D.OverlapCircle( new Vector2(_feetLocation.position.x, _feetLocation.position.y) , _GROUND_CHECK_RADIUS, _groundLayer);
    }


    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rigidBody = GetComponent<Rigidbody2D>();
        _feetLocation = transform.Find("FeetLocation").transform;
        _animator = GetComponent<SpriteAnimator>();
        
    }

    
    void Update()
    {


        // GET INPUT


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




        // SET STATE MACHINE


        // Ground
        if ( Mathf.Abs(_rigidBody.linearVelocityX) > _MOVE_THRESHOLD)
        {
            MoveState = MoveStates.Moving;
        
        }
        else
        {
           MoveState = MoveStates.Idle; 
        }


        // Air
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
            if (_movementDirection.x > 0) _animator.SetFlip(true);
            else if (_movementDirection.x < 0) _animator.SetFlip(false);

            // Handle States
            if (AirState != AirStates.Grounded)
            {
                if (AirState == AirStates.Jumping) _animator.Play("Jump");
                else if (AirState == AirStates.Falling) _animator.Play("Fall");
            }
            else
            {
                if (MoveState == MoveStates.Moving) _animator.Play("Walk");
                else _animator.Play("Idle");
            }
        }

        // Torch
        if (Input.GetKeyDown(KeyCode.F))
        {
            if(IsLit == false){IsLit = true;} else {IsLit = false;}
            TorchLight.SetActive(IsLit);
        }

        //Inventory
        if (Input.GetKeyDown(KeyCode.I))
        {
            if(IsInventoryOpen == false){IsInventoryOpen = true;} else {IsInventoryOpen = false;}
            Inventory.SetActive(IsInventoryOpen);
        }
    }


    void FixedUpdate()
    {


        // Move
        _rigidBody.linearVelocityX = _movementDirection.x * _moveSpeed;

        if (Time.time - _lastJumpInput <= _JUMP_BUFFER && IsGrounded())
        {
            _lastJumpInput = -100f;
            _rigidBody.linearVelocityY = 0;
            _rigidBody.AddForce(Vector2.up * _jumpPower, ForceMode2D.Impulse);
        }

    }

}
