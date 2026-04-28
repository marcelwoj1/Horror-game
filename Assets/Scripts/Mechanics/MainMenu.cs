using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles main menu interactions and scene transitions.
/// </summary>
/// <remarks>
/// This system:
/// - Loads the intro scene to start the main game
/// - Loads a demo/tutorial scene for testing
/// - Allows the player to quit the application
/// </remarks>
public class MainMenu : MonoBehaviour
{
    /// <summary>
    /// Starts the main game by loading the intro scene.
    /// </summary>
    public void PlayGame()
    {
        SceneManager.LoadScene("Intro");
    }

    /// <summary>
    /// Starts the tutorial or demo level.
    /// </summary>
    public void PlayTestGame()
    {
        SceneManager.LoadScene("Demo");
    }

    /// <summary>
    /// Quits the application.
    /// </summary>
    /// <remarks>
    /// This will only function in a built application, not within the Unity editor.
    /// </remarks>
    public void QuitGame()
    {
        Application.Quit();
    }
}