using UnityEngine;
using TMPro;

public class Teleport : MonoBehaviour
{
    [Header("Teleport Settings")]
    [Tooltip("Where the player will teleport to")]
    [SerializeField] private Transform destination;

    [Header("Interaction Settings")]
    [Tooltip("Key to press to teleport")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    
    [Tooltip("Distance at which the text appears and player can teleport")]
    [SerializeField] private float interactionDistance = 2f;

    [Header("Camera Bounds (Optional)")]
    [Tooltip("New camera bounds after teleport. Leave empty to keep current bounds.")]
    [SerializeField] private Transform newCameraBounds;

    [Header("Debug")]
    [SerializeField] private bool showGizmos = true;

    private TextMeshPro promptText;
    private Transform playerTransform;
    private bool playerInRange = false;

    private void Awake()
    {
        // Find the TextMeshPro component in children
        promptText = GetComponentInChildren<TextMeshPro>();
        
        if (promptText != null)
        {
            promptText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        // Check distance to player if we have a reference
        if (playerTransform != null)
        {
            float distance = Vector2.Distance(transform.position, playerTransform.position);
            playerInRange = distance <= interactionDistance;

            // Show/hide text based on distance
            if (promptText != null)
            {
                promptText.gameObject.SetActive(playerInRange);
            }

            // Teleport when E is pressed and player is in range
            if (playerInRange && Input.GetKeyDown(interactKey))
            {
                TeleportPlayer(playerTransform);
            }
        }
        else
        {
            // Hide text if no player nearby
            if (promptText != null && promptText.gameObject.activeSelf)
            {
                promptText.gameObject.SetActive(false);
            }
            playerInRange = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the colliding object is on the "Player" layer
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            playerTransform = other.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Clear player reference when they leave the trigger
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            playerTransform = null;
            playerInRange = false;
            
            if (promptText != null)
            {
                promptText.gameObject.SetActive(false);
            }
        }
    }

    private void TeleportPlayer(Transform player)
    {
        if (destination == null)
        {
            Debug.LogWarning("Teleport destination is not set!", this);
            return;
        }

        // Get the root player GameObject (in case the collider is on a child object)
        Transform rootPlayer = player.root;

        // Teleport the player to the destination
        rootPlayer.position = destination.position;

        // Update camera bounds if a new bounds transform is provided
        if (newCameraBounds != null)
        {
            CameraTrack.SetBounds(newCameraBounds);
        }
    }

    private void OnDrawGizmos()
    {
        if (!showGizmos) return;

        // Draw interaction distance range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);

        // Draw a line from this teleporter to the destination
        if (destination != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, destination.position);
            
            // Draw destination marker
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(destination.position, 0.5f);
            Gizmos.DrawLine(destination.position + Vector3.up * 0.5f, destination.position + Vector3.up * 1.5f);
        }

        // Draw the trigger area (if collider exists)
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f); // Orange semi-transparent
            
            if (col is BoxCollider2D boxCol)
            {
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawCube(boxCol.offset, boxCol.size);
            }
            else if (col is CircleCollider2D circleCol)
            {
                Gizmos.DrawWireSphere(transform.position + (Vector3)circleCol.offset, circleCol.radius);
            }
        }
    }
}
