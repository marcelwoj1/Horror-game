using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public Movement _movement;
    public GameObject AttackPoint;
    
    private SpriteRenderer _spriteRenderer;

    public bool IsAttcking;

    public void Attack()
    {
        IsAttcking = true;
    }

    public void StopAttack()
    {
        IsAttcking = false;
    }

    void Update()
    {
        if (AttackPoint != null)
        {
            float xPos = Mathf.Abs(AttackPoint.transform.localPosition.x);

            if (_movement._spriteRenderer.flipX) // Right
            {
                AttackPoint.transform.localPosition = new Vector3(xPos, AttackPoint.transform.localPosition.y, AttackPoint.transform.localPosition.z);
            }
            else // Left
            {
                AttackPoint.transform.localPosition = new Vector3(-xPos, AttackPoint.transform.localPosition.y, AttackPoint.transform.localPosition.z);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && IsAttcking)
        {
            Debug.Log("Enemy hit");
            IsAttcking = false;
        }
    }
    
}
