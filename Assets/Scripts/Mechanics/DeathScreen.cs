using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles the death screen UI and related game flow actions.
/// </summary>
/// <remarks>
/// This system:
/// - Displays the death screen when the player dies
/// - Allows restarting the current scene
/// - Allows returning to the main menu
/// - Allows quitting the game
/// - Clears enemies before showing the death screen
/// </remarks>
public class DeathScreen : MonoBehaviour
{
    [Header("DeathScreen")]

    /// <summary>Reference to the death screen UI panel.</summary>
    public GameObject DeathPanel;

    /// <summary>
    /// Restarts the game by reloading the current scene.
    /// </summary>
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Returns to the main menu scene.
    /// </summary>
    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    /// <summary>
    /// Quits the application.
    /// </summary>
    /// <remarks>
    /// Note: This will only work in a built application, not in the Unity editor.
    /// </remarks>
    public void QuitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// Clears enemies from the scene and displays the death screen.
    /// </summary>
    /// <remarks>
    /// Finds all objects tagged as "Enemy", destroys them,
    /// pauses the game, and activates the death UI panel.
    /// </remarks>
    public void ReloadScene()
    {
        // Finds all objects with the tag "Enemy"
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Enemy");

        // Destroys all objects with the tag "Enemy"
        foreach (GameObject obj in objects)
        {
            Destroy(obj);
        }

        Time.timeScale = 0f;
        DeathPanel.SetActive(true);
    }
}