using UnityEngine;

public class Doors : MonoBehaviour
{
    public Teleport teleport;
    private QuestService _questService;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _questService = GameObject.Find("QuestService").GetComponent<QuestService>();
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.childCount == 0)
        {
            teleport.UnlockDoor();
            _questService.SatisfyQuest("Door");
        }
    }

    public void breakPlank()
    {
        Destroy(transform.GetChild(0).gameObject);
    }
}
