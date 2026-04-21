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
            if(SceneManager.GetActiveScene().name == "Intro")
            {
                SceneManager.LoadScene("Game");
            }
            else if(SceneManager.GetActiveScene().name == "Outro")
            {
                SceneManager.LoadScene("Menu");
            }
        }
        if(nextText != null)
        {
            nextText.SetActive(true);
        }
        Destroy(gameObject);
    }
}
