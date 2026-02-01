using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

public class Movement : MonoBehaviour
{
    
    // Variables
    private Rigidbody2D _rigidBody;
    private Vector2 _movementDirection;


    // Config
    private float _moveSpeed = 5;


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


    void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    
    void Update()
    {

        // Update input 
        _movementDirection = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );


        // Update state machine
        if ( Mathf.Abs(_rigidBody.linearVelocityX) > 0)
        {
            MoveState = MoveStates.Moving;
        
        }
        else
        {
           MoveState = MoveStates.Idle; 
        }

        if (MoveState == MoveStates.Moving)
        {
            Debug.Log("YURP");
        }

      

  
    }


    void FixedUpdate()
    {
        _rigidBody.linearVelocityX = _movementDirection.x * _moveSpeed;
    }

}
