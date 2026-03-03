using UnityEngine;

public class PatrollingEnemy : MonoBehaviour
{
    private Vector2 _movementDirection;
    private Rigidbody2D _rigidBody;

    private bool IsMovingRight = false;

    private Transform _leftTarget;
    private Transform _rightTarget;

    public float _moveSpeed;

    public void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _leftTarget = transform.Find("LeftTarget");
        _rightTarget = transform.Find("RightTarget");
    }
    public void Update()
    {
        

        if(transform.position.x > _leftTarget.position.x)
        {
            IsMovingRight = false;
        }
        if(transform.position.x < _rightTarget.position.x)
        {
            IsMovingRight = true;
        }

        if(IsMovingRight == true)
        {
            _movementDirection = (_rightTarget.position - transform.position).normalized;
            _rigidBody.linearVelocityX = _movementDirection.x * _moveSpeed;
        }
        else
        {
            _movementDirection = (_leftTarget.position - transform.position).normalized;
            _rigidBody.linearVelocityX = _movementDirection.x * _moveSpeed;
        }
        
    }
    
}
