using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles interactive tutorial objects that display hints when the player approaches.
/// </summary>
/// <remarks>
/// This system:
/// - Detects player proximity using trigger collisions
/// - Displays contextual tutorial hints
/// - Highlights the object using a light indicator
/// - Pauses gameplay while the hint is shown
///
/// Each object can only be interacted with once.
/// </remarks>
public class IntroObjects : MonoBehaviour
{
    [Header("Light")]

    /// <summary>Visual indicator shown when the player is nearby.</summary>
    private GameObject ObjectLight;

    [Header("Panel")]

    /// <summary>UI panel used to display the hint.</summary>
    public GameObject panel;

    [Header("Text")]

    /// <summary>UI text component for displaying the hint.</summary>
    public Text HintText;

    /// <summary>Hint message shown to the player.</summary>
    public string Hint;

    [Header("State")]

    /// <summary>Indicates whether the object has already been interacted with.</summary>
    public bool isInteracted = false;
    
    /// <summary>
    /// Initialises the object and disables the light indicator.
    /// </summary>
    void Start()
    {
        ObjectLight = transform.Find("Light")?.gameObject;

        if (ObjectLight != null)
            ObjectLight.SetActive(false);
    }

    /// <summary>
    /// Detects when the player enters the trigger area.
    /// </summary>
    /// <param name="collision">Collider entering the trigger.</param>
    void OnTriggerEnter2D(Collider2D collision)
    {
        // Only trigger for player and if not already used
        if (collision.CompareTag("Player") && !isInteracted)
        {
            if (ObjectLight != null)
                ObjectLight.SetActive(true);

            panel.SetActive(true);
            HintText.text = Hint;

            // Pause game while reading hint
            Time.timeScale = 0f;

            isInteracted = true;
        }
    }

    /// <summary>
    /// Handles player input to close the hint and resume gameplay.
    /// </summary>
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && isInteracted)
        {
            Time.timeScale = 1f;

            panel.SetActive(false);

            if (ObjectLight != null)
                ObjectLight.SetActive(false);
        }
    }
}