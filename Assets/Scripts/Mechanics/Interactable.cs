using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float interactionDistance = 2.5f;
    [SerializeField] private KeyCode interactionKey = KeyCode.E;

    [Header("Events")]
    public UnityEvent OnInteract;

    private GameObject _player;
    private GameObject _interactionImage;
    private bool _isPlayerInRange;

    private void Awake()
    {
        // Find the player game object by name as requested
        _player = GameObject.Find("Player");
        
        if (_player == null)
        {
            Debug.LogWarning($"Interactable on {gameObject.name} could not find 'Player' object in the scene.");
        }

        // Cache the child named "Image"
        Transform imageTransform = transform.Find("Image");
        if (imageTransform != null)
        {
            _interactionImage = imageTransform.gameObject;
            _interactionImage.SetActive(false);
        }
        else
        {
            Debug.LogWarning($"Interactable on {gameObject.name} could not find a child named 'Image'.");
        }
    }

    private void Update()
    {
        if (_player == null) return;

        // Check distance to player
        float distance = Vector3.Distance(transform.position, _player.transform.position);
        bool inRange = distance <= interactionDistance;

        // Update UI prompt visibility
        if (inRange != _isPlayerInRange)
        {
            _isPlayerInRange = inRange;
            if (_interactionImage != null)
            {
                _interactionImage.SetActive(_isPlayerInRange);
            }
        }

        // Handle interaction input
        if (_isPlayerInRange && Input.GetKeyDown(interactionKey))
        {
            Interact();
        }
    }

    public void Interact()
    {
        OnInteract?.Invoke();
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize interaction range in the editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}
