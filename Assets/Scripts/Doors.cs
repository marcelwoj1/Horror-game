using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Doors : MonoBehaviour
{
    public bool AtDoor;
    public bool AtBackDoor;
    public bool temp;
    public GameObject DoorPrompt;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E)) 
        {
            temp = true;
            if(AtDoor == true){SceneManager.LoadScene(1);}
            if(AtBackDoor == true){SceneManager.LoadScene(0);}
        }
    }
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.tag == "Door")
        {
            AtDoor = true;
            DoorPrompt.SetActive(true);
        }
        if(other.tag == "BackDoor")
        {
            AtBackDoor = true;
            DoorPrompt.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D other) {
        if(other.tag == "Door")
        {
            AtDoor = false;
            DoorPrompt.SetActive(false);
        }
        if(other.tag == "BackDoor")
        {
            AtBackDoor = false;
            DoorPrompt.SetActive(false);
        }
    }
}
