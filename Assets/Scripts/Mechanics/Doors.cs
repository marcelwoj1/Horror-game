using UnityEngine;

public class Doors : MonoBehaviour
{
    [Header("Components")]
    public Teleport teleport;

    [Header("Variables")]
    private QuestService _questService;
    
    void Start()
    {
        _questService = GameObject.Find("QuestService").GetComponent<QuestService>();
    }

    //Checks if door is unlocked and completes the door quest
    void Update()
    {
        if(transform.childCount == 0)
        {
            teleport.UnlockDoor();
            _questService.SatisfyQuest("Door");
        }
    }

    //Breaks the plank on the door when hit with axe
    public void breakPlank()
    {
        //Only breaks the plank if there is a plank on the door
        if(transform.childCount == 0)
            return;
        Destroy(transform.GetChild(0).gameObject);
    }
}
