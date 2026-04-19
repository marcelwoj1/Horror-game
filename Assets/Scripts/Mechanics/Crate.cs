using UnityEngine;

public class Crate : MonoBehaviour
{
    [Header("Components")]
    private Hiding _hiding;
    private PlayerManager _playerManager;
    
    [Header("Variables")]
    public bool _isPlayerNear = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _playerManager = GameObject.Find("Player").GetComponent<PlayerManager>();
        _hiding = _playerManager.GetComponent<Hiding>();
    }

    void Update()
    {
        if(_isPlayerNear == true && Input.GetKeyDown(KeyCode.E))
        {
            if(_playerManager.IsHiding == false)
            {
                Debug.Log("Hiding");
                _hiding.Hide();
            }
            else
            {
                Debug.Log("UnHiding");
                _hiding.UnHide();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            _isPlayerNear = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            _isPlayerNear = false;
        }
    }
}
