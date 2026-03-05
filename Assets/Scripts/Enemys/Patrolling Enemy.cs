using UnityEngine;

public class PatrollingEnemy : MonoBehaviour
{
    // | VARIABLES |

    private Vector2 _movementDirection;
    private Rigidbody2D _rigidBody;

    [Header("Left and right Anchors")]
    public Transform LeftTarget;
    public Transform RightTarget;

    [Header("Speed")]
    public float _moveSpeed;

    [Header("Player Health")]
    public PlayerHealth _playerHealth;

    public void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _movementDirection = (RightTarget.position - transform.position).normalized;
        _rigidBody.linearVelocityX = _movementDirection.x * _moveSpeed;
    }
    public void Update()
    {
        if(transform.position.x < LeftTarget.position.x)
        {
            _movementDirection = (RightTarget.position - transform.position).normalized;
            _rigidBody.linearVelocityX = _movementDirection.x * _moveSpeed;
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if(transform.position.x > RightTarget.position.x)
        {
            _movementDirection = (LeftTarget.position - transform.position).normalized;
            _rigidBody.linearVelocityX = _movementDirection.x * _moveSpeed;
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            _playerHealth.TakeDamage(1);
        }
    }
}
