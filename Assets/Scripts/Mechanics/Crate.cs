using UnityEngine;

public class Crate : MonoBehaviour
{
    [Header("Components")]
    private Hiding _hiding;
    private PlayerManager _playerManager;
    
    [Header("Variables")]
    public bool _isPlayerNear = false;

    void Start()
    {
        _playerManager = GameObject.Find("Player").GetComponent<PlayerManager>();
        _hiding = _playerManager.GetComponent<Hiding>();
    }

    //Checks if player is near and hides/unhides
    void Update()
    {
        //Checks if player is near the crate and presses E to hide/unhide
        if(Input.GetKeyDown(KeyCode.E) && _isPlayerNear == true)
        {
            if(_playerManager.IsHiding == false)
            {
                _hiding.Hide();
            }
            else
            {
                _hiding.UnHide();
            }
        }
    }

    //Checks if player is near the crate and sets isPlayerNear to true when player enters
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            _isPlayerNear = true;
        }
    }

    //Checks if player is near the crate and sets isPlayerNear to false when player leaves
    void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            _isPlayerNear = false;
        }
    }
}
