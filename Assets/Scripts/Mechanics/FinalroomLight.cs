using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Controls lighting behaviour when the player enters the final room.
/// </summary>
/// <remarks>
/// This script:
/// - Detects when the player enters a trigger zone
/// - Adjusts the intensity of a 2D light
/// - Enhances atmosphere for the final area of the game
/// </remarks>
public class FinalroomLight : MonoBehaviour
{
    [Header("Components")]

    /// <summary>Reference to the final room light.</summary>
    public Light2D finalLight;

    /// <summary>
    /// Increases light intensity when the player enters the trigger.
    /// </summary>
    /// <param name="collision">Collider of the entering object.</param>
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            finalLight.intensity = 0.4f;
        }
    }
}