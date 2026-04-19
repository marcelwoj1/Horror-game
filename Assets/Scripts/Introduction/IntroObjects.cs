using UnityEngine;
using UnityEngine.UI;

public class IntroObjects : MonoBehaviour
{
    private GameObject ObjectLight;
    public GameObject panel;
    public Text HintText;
    public string Hint;
    public bool isInteracted = false;
    
    void Start()
    {
        ObjectLight = transform.Find("Light")?.gameObject;
        ObjectLight.SetActive(false);
        panel.SetActive(false);
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isInteracted)
        {
            Debug.Log("Interacted");
            ObjectLight.SetActive(true);
            panel.SetActive(true);
            HintText.text = Hint;
            Time.timeScale = 0f;
            isInteracted = true;
        }
    }
    public void Update()
    {
        if (Input.GetMouseButtonDown(0) && isInteracted)
        {
            Time.timeScale = 1f;
            panel.SetActive(false);
            ObjectLight.SetActive(false);
        }
    }
}
