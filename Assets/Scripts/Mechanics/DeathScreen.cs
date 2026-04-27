using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScreen : MonoBehaviour
{
    [Header("DeathScreen")]
    public GameObject DeathPanel;

    //Restarts the game by reloading the current scene
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    //Returns to main menu
    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
    //Quits the game
    public void QuitGame()
    {
        Application.Quit();
    }
    //Removes all enemies from the scene and shows the death screen
    public void ReloadScene()
    {
        //Finds all objects with the tag "Enemy"
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Enemy");

        //Destroys all objects with the tag "Enemy"
        foreach (GameObject obj in objects)
        {
            Destroy(obj);
        }
        Time.timeScale = 0f;
        DeathPanel.SetActive(true);
    }
}
