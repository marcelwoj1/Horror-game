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

    public void Start()
    {
        _movement = GetComponent<Movement>();
        _playerManager = GetComponent<PlayerManager>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rigidBody = GetComponent<Rigidbody2D>();
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
        Debug.Log("Run Hide");
        _playerManager.IsHiding = true;

        // Change Sprite Layer
        _spriteRenderer.sortingLayerName = "Wall";

        // Stop Movement
        _rigidBody.linearVelocityX = 0;
        _rigidBody.linearVelocityY = 0;
        _playerManager.AllowMovement = false;

        // Ignore Enemy Collision
        Physics2D.IgnoreLayerCollision(
                LayerMask.NameToLayer("Player"),
                LayerMask.NameToLayer("Enemy"),
                true
        );

        // Start Coroutine
        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);

        hideCoroutine = StartCoroutine(WaitUntilGrounded());
    }

    public void UnHide()
    {
        _playerManager.IsHiding = false;
        _playerManager.AllowMovement = true;
        _spriteRenderer.sortingLayerName = "Player";
        _rigidBody.gravityScale = 2;
    }

    IEnumerator WaitUntilGrounded()
    {
        yield return new WaitUntil(() => _movement.IsGrounded() || !_playerManager.IsHiding);

        if (!_playerManager.IsHiding)
            yield break;

        _rigidBody.gravityScale = 0;
        transform.position = new Vector3(transform.position.x, transform.position.y + 1.4f, transform.position.z);
    }

    
}
