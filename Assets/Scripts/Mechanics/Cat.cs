using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles end-of-game behaviour triggered by the cat.
/// </summary>
/// <remarks>
/// This script is responsible for transitioning the game
/// to the outro scene when the end condition is met.
/// </remarks>
public class Cat : MonoBehaviour
{
    /// <summary>
    /// Ends the game by loading the outro scene.
    /// </summary>
    public void EndGame()
    {
        SceneManager.LoadScene("Outro");
    }
}