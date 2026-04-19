using UnityEngine;

public class Crate : MonoBehaviour
{
    private Hiding _hiding;
    private PlayerManager _playerManager;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _playerManager = GameObject.Find("Player").GetComponent<PlayerManager>();
        _hiding = _playerManager.GetComponent<Hiding>();
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log("Crate");
        if(collision.gameObject.CompareTag("Player"))
        {
            if(Input.GetKeyDown(KeyCode.E))
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
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
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
}
