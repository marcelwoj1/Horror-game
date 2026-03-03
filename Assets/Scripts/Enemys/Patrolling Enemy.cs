using UnityEngine;

public class PatrollingEnemy : MonoBehaviour
{
    private Vector2 _movementDirection;
    private Rigidbody2D _rigidBody;

    public bool IsMovingRight = false;

    public Transform LeftTarget;
    public Transform RightTarget;

    public float _leftTargetX;
    public float _rightTargetX;
    public float _currentPositionX;

    public float _moveSpeed;

    public void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _movementDirection = (RightTarget.position - transform.position).normalized;
        _rigidBody.linearVelocityX = _movementDirection.x * _moveSpeed;
    }
    public void Update()
    {
        _currentPositionX = transform.position.x;
        _leftTargetX = LeftTarget.position.x;
        _rightTargetX = RightTarget.position.x;
        if(transform.position.x < LeftTarget.position.x)
        {
            _movementDirection = (RightTarget.position - transform.position).normalized;
            _rigidBody.linearVelocityX = _movementDirection.x * _moveSpeed;
        }
        else if(transform.position.x > RightTarget.position.x)
        {
            _movementDirection = (LeftTarget.position - transform.position).normalized;
            _rigidBody.linearVelocityX = _movementDirection.x * _moveSpeed;
        }
        
    }
    
}
