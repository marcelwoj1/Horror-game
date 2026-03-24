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

    [Header("Distance Based Damage")]
    public float detectionRange = 1.5f;
    public float damageCooldown = 1.0f;
    public float knockbackForce = 15f;
    private float _nextDamageTime;
    private Transform _playerTransform;
    private PlayerHealth _playerHealth;

    public void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _movementDirection = (RightTarget.position - transform.position).normalized;
        _rigidBody.linearVelocityX = _movementDirection.x * _moveSpeed;

        if (_playerHealth != null)
        {
            _playerTransform = _playerHealth.transform;
        }
        else
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                _playerTransform = player.transform;
                _playerHealth = player.GetComponent<PlayerHealth>();
            }
        }
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

        CheckDistanceDamage();
    }

    private void CheckDistanceDamage()
    {
        if (_playerTransform == null || _playerHealth == null) return;
        if (Time.time < _nextDamageTime) return;

        float distance = Vector2.Distance(transform.position, _playerTransform.position);
        if (distance <= detectionRange)
        {
            // Direction for knockback (based on enemy facing direction)
            float side = transform.localScale.x;
            Vector2 knockbackDir = new Vector2(side, 1f).normalized;

            _playerHealth.TakeDamage(1, knockbackDir * knockbackForce);
            _nextDamageTime = Time.time + damageCooldown;
        }
    }
}
