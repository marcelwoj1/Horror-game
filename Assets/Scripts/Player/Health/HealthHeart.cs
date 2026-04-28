using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Represents a single heart UI element used to display player health.
/// </summary>
/// <remarks>
/// This component:
/// - Updates the visual state of a heart (full or empty)
/// - Uses sprite swapping to reflect player health
/// - Is controlled externally by the HealthHeartManager
/// </remarks>
public class HealthHeart : MonoBehaviour
{
    /// <summary>Sprite used when the heart is full.</summary>
    public Sprite FullHeart;

    /// <summary>Sprite used when the heart is empty.</summary>
    public Sprite EmptyHeart;

    /// <summary>Reference to the UI Image component.</summary>
    private Image HeartImage;

    /// <summary>
    /// Initialises the Image component reference.
    /// </summary>
    private void Awake()
    {
        HeartImage = GetComponent<Image>();
    }

    /// <summary>
    /// Updates the visual state of the heart.
    /// </summary>
    /// <param name="state">Desired heart state (full or empty).</param>
    public void SetHeartState(HeartState state)
    {
        switch (state)
        {
            case HeartState.Full:
                HeartImage.sprite = FullHeart;
                break;

            case HeartState.Empty:
                HeartImage.sprite = EmptyHeart;
                break;
        }
    }

    /// <summary>
    /// Defines possible visual states of a heart.
    /// </summary>
    public enum HeartState
    {
        /// <summary>Heart is full (represents available health).</summary>
        Full = 1,

        /// <summary>Heart is empty (represents lost health).</summary>
        Empty = 0
    }
}