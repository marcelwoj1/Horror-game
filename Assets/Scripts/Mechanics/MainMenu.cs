using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void OptionsMenu()
    {
        Debug.Log("Options Menu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
