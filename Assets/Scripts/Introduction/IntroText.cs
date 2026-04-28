using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controls progression through intro/outro text sequences and handles scene transitions.
/// </summary>
/// <remarks>
/// This system:
/// - Activates the next text object in a sequence
/// - Destroys the current text after progression
/// - Loads the next scene when the final text is reached
///
/// Used for both intro and outro cutscenes.
/// </remarks>
public class IntroText : MonoBehaviour
{
    [Header("Next Text")]

    /// <summary>Reference to the next text object in the sequence.</summary>
    public GameObject nextText;

    [Header("Finish Text")]

    /// <summary>Indicates if this is the final text in the sequence.</summary>
    public bool finishText;

    /// <summary>
    /// Advances to the next text or triggers a scene change if this is the final text.
    /// </summary>
    public void NextText()
    {
        // If this is the final text, transition to the appropriate scene
        if (finishText)
        {
            string currentScene = SceneManager.GetActiveScene().name;

            if (currentScene == "Intro")
            {
                SceneManager.LoadScene("Game");
            }
            else if (currentScene == "Outro")
            {
                SceneManager.LoadScene("Menu");
            }
        }

        // Activate next text in sequence
        if (nextText != null)
        {
            nextText.SetActive(true);
        }

        // Remove current text
        Destroy(gameObject);
    }
}