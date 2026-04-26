using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Intro");
    }
    public void PlayTestGame()
    {
        SceneManager.LoadScene("Demo");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
