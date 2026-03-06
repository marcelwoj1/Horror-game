using UnityEngine;

public class Hiding : MonoBehaviour
{
    Movement _movement;
    SpriteRenderer _spriteRenderer;

    [HideInInspector] public bool IsHiding;

    public void Hide()
    {
        IsHiding = true;
        _movement.AllowMovement = false;
        _spriteRenderer.color = new Color(0.1f, 0.1f, 0.1f, 0.1f);
        Physics2D.IgnoreLayerCollision(
                LayerMask.NameToLayer("Player"),
                LayerMask.NameToLayer("Enemy"),
                true
        );
    }

    public void UnHide()
    {
        IsHiding = false;
        _movement.AllowMovement = true;
        _spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
        Physics2D.IgnoreLayerCollision(
                LayerMask.NameToLayer("Player"),
                LayerMask.NameToLayer("Enemy"),
                false
        );
    }

    public void Start()
    {
        _movement = GetComponent<Movement>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void Interact()
    {
        if(IsHiding == false)
        {
            Hide();
        }
        else
        {
            UnHide();
        }
    }
}
