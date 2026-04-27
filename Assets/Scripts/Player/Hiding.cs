using UnityEngine;
using System.Collections;

public class Hiding : MonoBehaviour
{
    [Header("Variables")]
    private Coroutine hideCoroutine;

    [Header("Components")]
    private Movement _movement;
    private PlayerManager _playerManager;
    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rigidBody;
    private CapsuleCollider2D _capsuleCollider;

    public void Start()
    {
        _movement = GetComponent<Movement>();
        _playerManager = GetComponent<PlayerManager>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rigidBody = GetComponent<Rigidbody2D>();
        _capsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    public void Hide()
    {
        if(_playerManager.IsHiding == true)
        {
            return;
        }
        if(_movement.IsGrounded() == false)
        {
            return;
        }
        _playerManager.IsHiding = true;

        // Change Sprite Layer
        _spriteRenderer.sortingLayerName = "Wall";

        // Stop Movement
        _rigidBody.linearVelocityX = 0;
        _rigidBody.linearVelocityY = 0;
        _playerManager.AllowMovement = false;

        // Disable Collision
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Janitor"), true);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy Hitbox"), true);

        // Start Coroutine
        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);

        hideCoroutine = StartCoroutine(WaitUntilGrounded());
    }

    public void UnHide()
    {
        _playerManager.IsHiding = false;

        // Allow Movement
        _playerManager.AllowMovement = true;
        
        // Change Sprite Layer
        _spriteRenderer.sortingLayerName = "Player";

        // Enable Gravity
        _rigidBody.gravityScale = 2;

        // Enable Collision
        _capsuleCollider.enabled = true;
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Janitor"), false);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy Hitbox"), false);
    }

    IEnumerator WaitUntilGrounded()
    {
        // Wait until the player is grounded or stops hiding
        yield return new WaitUntil(() => _movement.IsGrounded() || !_playerManager.IsHiding);

        if (!_playerManager.IsHiding)
            yield break;

        // Change Y position so Player appears inside the crate
        _rigidBody.gravityScale = 0;
        transform.position = new Vector3(transform.position.x, transform.position.y + 1.4f, transform.position.z);
    }

    
}
