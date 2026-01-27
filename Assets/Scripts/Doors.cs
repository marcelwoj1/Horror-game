using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Doors : MonoBehaviour
{
    public bool AtDoor;
    public int DoorValue;
    public GameObject DoorPrompt;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E)) 
        {
            if(AtDoor == true){SceneManager.LoadScene(DoorValue);}
        }
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.tag == "Player")
        {
            AtDoor = true;
            DoorPrompt.SetActive(true);
        }
        
    }
    private void OnTriggerExit2D(Collider2D other) 
    {
        if(other.tag == "Player")
        {
            AtDoor = false;
            DoorPrompt.SetActive(false);
        }
    }
}
