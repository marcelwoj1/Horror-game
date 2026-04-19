using UnityEngine;

public class Rat : MonoBehaviour
{

    [Header("Settings")]
    public float speed = 6f;
    public float offsetMagnitude = 2f;
    public float offsetSpeed = 2f;
    public float flipDebounceTime = 0.5f;
    
    [Header("References")]
    private Transform player;
    private PlayerManager _playerManager;
    private float startY;
    private SpriteAnimator _animator;
    private float _noiseTime;
    private float _direction = 1f;
    private float _flipTimer;
    private float _awakeTime;

    void Awake()
    {
        _awakeTime = Time.time;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startY = transform.position.y;
        _animator = GetComponent<SpriteAnimator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        _playerManager = player.GetComponent<PlayerManager>();
    }

    void Update()
    {
        if (Time.time < _awakeTime + 1f) 
        {
            _animator.Play("Idle");
            return;
        }

        if(_playerManager.IsHiding)
        {
            _animator.Play("Idle");
            return;
        }

        _noiseTime += Time.deltaTime * offsetSpeed;
        float xOffset = (Mathf.PerlinNoise(_noiseTime, 0f) - 0.5f) * 2f * offsetMagnitude;
        float targetX = player.position.x + xOffset;

        float targetDirection = Mathf.Sign(targetX - transform.position.x);
        
        if (targetDirection != _direction && _flipTimer <= 0)
        {
            _direction = targetDirection;
            _flipTimer = flipDebounceTime;
        }

        if (_flipTimer > 0)
            _flipTimer -= Time.deltaTime;

        Vector3 move = new Vector3(_direction * speed * Time.deltaTime, 0, 0);
        _animator.Play("Walk");
        transform.position += move;

        transform.localScale = new Vector3(_direction, 1, 1);
    }
    
}
