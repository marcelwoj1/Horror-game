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
    private float _JUMP_BUFFER = 0.1f;



    // | VARIABLES |

    // Components
    private Rigidbody2D _rigidBody;
    private Transform _feetLocation;


    // Config
    public float _moveSpeed = 5;
    public float _jumpPower = 30;
    [SerializeField] LayerMask _groundLayer;


    // Input
    private Vector2 _movementDirection;
    private bool _jumpQueued = false;
    private float _lastJumpInput;
    
    

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
        _rigidBody = GetComponent<Rigidbody2D>();
        _feetLocation = transform.Find("FeetLocation").transform;
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
        if (Input.GetButtonDown("Jump"))
        {
            _jumpQueued = true;
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



   
    }


    void FixedUpdate()
    {

        //Debug.Log(_jumpQueued);

        // Move
        _rigidBody.linearVelocityX = _movementDirection.x * _moveSpeed;

        if (_jumpQueued && IsGrounded())
        {
            Debug.Log("jumping");
            _jumpQueued = false;
            _rigidBody.linearVelocityY = 0;
            _rigidBody.AddForce(Vector2.up * _jumpPower, ForceMode2D.Impulse);
        }

    }

}
