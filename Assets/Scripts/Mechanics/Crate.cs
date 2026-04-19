using UnityEngine;

public class Crate : MonoBehaviour
{
    public Hiding _hiding;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log("Crate");
        if(collision.gameObject.CompareTag("Player"))
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                if(_hiding.IsHiding == false)
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
            if(_hiding.IsHiding == false)
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
