using UnityEngine;
using System.Collections;

public class Hiding : MonoBehaviour
{
    Movement _movement;
    SpriteRenderer _spriteRenderer;
    Rigidbody2D _rigidBody;
    public GameObject BugSprayTimer;
    private Coroutine hideCoroutine;
    [HideInInspector] public bool IsHiding;

    public void Hide()
    {
        if(IsHiding == true)
        {
            return;
        }
        if(_movement.IsGrounded() == false)
        {
            return;
        }
        IsHiding = true;
        _spriteRenderer.sortingLayerName = "Wall";
        _rigidBody.linearVelocityX = 0;
        _rigidBody.linearVelocityY = 0;
        _movement.AllowMovement = false;
        //_spriteRenderer.color = new Color(0.1f, 0.1f, 0.1f, 0.1f);
        Physics2D.IgnoreLayerCollision(
                LayerMask.NameToLayer("Player"),
                LayerMask.NameToLayer("Enemy"),
                true
        );
        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);

        hideCoroutine = StartCoroutine(WaitUntilGrounded());
    }

    public void UnHide()
    {
        IsHiding = false;
        _movement.AllowMovement = true;
        _spriteRenderer.sortingLayerName = "Player";
        //_spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
  
        _rigidBody.gravityScale = 2;
    }

    IEnumerator WaitUntilGrounded()
    {
        yield return new WaitUntil(() => _movement.IsGrounded() || !IsHiding);

        if (!IsHiding)
            yield break;

        _rigidBody.gravityScale = 0;
        transform.position = new Vector3(transform.position.x, transform.position.y + 1.4f, transform.position.z);
    }

    public void Start()
    {
        _movement = GetComponent<Movement>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rigidBody = GetComponent<Rigidbody2D>();
    }
    public void Update()
    {
        if (IsHiding && Input.GetKeyDown(KeyCode.E))
        {
            UnHide();
        }
    }

    public void BugSprayUsed()
    {
        IsHiding = true;
        BugSprayTimer.SetActive(true);

    }

    public void BugSprayEnded()
    {
        IsHiding = false;
        BugSprayTimer.SetActive(false);
    }
}
