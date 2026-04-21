using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroText : MonoBehaviour
{
    public GameObject nextText;
    public bool finishText;

    public void NextText()
    {
        if(finishText == true)
        {
            SceneManager.LoadScene("Game");
        }
        nextText.SetActive(true);
        Destroy(gameObject);
    }
}
