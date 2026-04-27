using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    //Plays the intro
    public void PlayGame()
    {
        SceneManager.LoadScene("Intro");
    }
    //Plays the Tutorial level
    public void PlayTestGame()
    {
        SceneManager.LoadScene("Demo");
    }
    //Quits the game
    public void QuitGame()
    {
        Application.Quit();
    }
}
