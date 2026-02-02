using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

[System.Serializable]
public class PositionOption
{
    public string name;
    public Vector3 position;
}

public class Doors : MonoBehaviour
{
    public bool AtDoor;
    public GameObject DoorPrompt;
    public GameObject Player;
    
    [Header("Door Locations")]
    [SerializeField]
    private List<PositionOption> positions = new List<PositionOption>()
    {
        new PositionOption { name = "1st Room", position = new Vector3(23, 0, 0) },
        new PositionOption { name = "2nd Room", position = new Vector3(0, 1, 0) },
        new PositionOption { name = "3rd Room", position = new Vector3(5, 1, 0) }
    };

    [SerializeField]
    private int selectedIndex = 0;

    void Start()
    {
        selectedIndex = Mathf.Clamp(selectedIndex, 0, positions.Count - 1);
    } 
    
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E)) 
        {
            if(AtDoor == true)
            {
                Player.transform.position = positions[selectedIndex].position;
            }
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
