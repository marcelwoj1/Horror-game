using UnityEngine;

public class PlayerDetection : MonoBehaviour
{
    public Transform player;
    public float viewDistance = 6f;
    public float viewAngle = 60f;
    public LayerMask obstacleMask;

    public bool playerDetected = false;
    public Vector2 directionToPlayer;
    public float angle;
    public float distance;
    public Vector2 facingDirection;

    void Update()
    {
        DetectPlayer();
        
    }

    void DetectPlayer()
{
    facingDirection = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
    directionToPlayer = player.position - transform.position;

    distance = directionToPlayer.magnitude;

    if (distance < viewDistance)
    {
        angle = Vector2.Angle(facingDirection, directionToPlayer);

        if (angle < viewAngle / 2)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer.normalized, viewDistance, ~obstacleMask);
            Debug.DrawRay(transform.position, directionToPlayer.normalized * viewDistance, Color.red);
            Debug.Log("Ray hit: " + hit.collider.name);

            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                playerDetected = true;
                Debug.Log("Player detected!");
                return;
            }
        }
    }

    playerDetected = false;
}
    void OnDrawGizmos()
    {
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireSphere(transform.position, viewDistance);

    Vector3 left = Quaternion.Euler(0, 0, viewAngle / 2) * facingDirection;
    Vector3 right = Quaternion.Euler(0, 0, -viewAngle / 2) * facingDirection;

    Gizmos.color = Color.red;
    Gizmos.DrawLine(transform.position, transform.position + left * viewDistance);
    Gizmos.DrawLine(transform.position, transform.position + right * viewDistance);
    }
}

