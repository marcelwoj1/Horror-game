using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float interactionDistance = 2.5f;
    [SerializeField] private KeyCode interactionKey = KeyCode.E;
    [SerializeField] private bool singleUse = true;

    [Header("Events")]
    public UnityEvent OnInteract;

    private GameObject _player;
    private GameObject _interactionImage;
    private bool _isPlayerInRange;
    private bool _interactionUsed;

    private void Awake()
    {
        _player = GameObject.Find("Player");

        //Finds the Interact image on the gameobject
        Transform imageTransform = transform.Find("Image");
        if (imageTransform != null)
        {
            _interactionImage = imageTransform.gameObject;
            _interactionImage.SetActive(false);
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
            if (_interactionImage != null && _interactionUsed == false)
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
        // If it's single use and already triggered, don't do anything
        if (singleUse && _interactionUsed) return;

        OnInteract?.Invoke();

        if (singleUse)
        {
            _interactionUsed = true;
            if (_interactionImage != null)
            {
                _interactionImage.SetActive(false);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize interaction range in the editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}
