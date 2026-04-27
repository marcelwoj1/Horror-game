using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroText : MonoBehaviour
{
    [Header("Next Text")]
    public GameObject nextText;

    [Header("Finish Text")]
    public bool finishText;

    public void NextText()
    {
        //Checks if this is the last text
        if(finishText == true)
        {
            //Loads the Game if its intro cutscene
            if(SceneManager.GetActiveScene().name == "Intro")
            {
                SceneManager.LoadScene("Game");
            }
            //Loads the Menu if its outro cutscene
            else if(SceneManager.GetActiveScene().name == "Outro")
            {
                SceneManager.LoadScene("Menu");
            }
        }
        //Sets next text to active
        if(nextText != null)
        {
            nextText.SetActive(true);
        }

        //Destroys current text
        Destroy(gameObject);
    }
}
