using UnityEngine;
using UnityEngine.UI;

public class IntroObjects : MonoBehaviour
{
    [Header("Light")]
    private GameObject ObjectLight;

    [Header("Panel")]
    public GameObject panel;

    [Header("Text")]
    public Text HintText;
    public string Hint;

    [Header("State")]
    public bool isInteracted = false;
    
    void Start()
    {
        ObjectLight = transform.Find("Light")?.gameObject;
        ObjectLight.SetActive(false);
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        //Checks if player enters the trigger and if not already interacted
        if (collision.CompareTag("Player") && !isInteracted)
        {
            //Turns light on
            ObjectLight.SetActive(true);
            //Shows Hint UI
            panel.SetActive(true);
            HintText.text = Hint;

            //Pauses game
            Time.timeScale = 0f;
            //Sets interacted to true
            isInteracted = true;
        }
    }
    public void Update()
    {
        //Removes hint UI when left mouse is clicked and resumes time
        if (Input.GetMouseButtonDown(0) && isInteracted)
        {
            Time.timeScale = 1f;
            panel.SetActive(false);
            ObjectLight.SetActive(false);
        }
    }
}
