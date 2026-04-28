using UnityEngine;

/// <summary>
/// Handles the initialisation of the boss encounter.
/// </summary>
/// <remarks>
/// This script is responsible for:
/// - Triggering the boss intro animation
/// - Starting boss music
/// - Updating quest progression
/// 
/// It is typically called when the player enters a specific trigger
/// or interacts with an object that begins the boss fight.
/// </remarks>
public class BossStart : MonoBehaviour
{
    [Header("Components")]

    /// <summary>Handles boss animation playback.</summary>
    private SpriteAnimator animator;

    /// <summary>Provides access to quest progression systems.</summary>
    private QuestService questService;

    /// <summary>
    /// Initialises required component references.
    /// </summary>
    void Start()
    {
        animator = GetComponent<SpriteAnimator>();
        questService = FindAnyObjectByType<QuestService>();
    }

    /// <summary>
    /// Starts the boss encounter.
    /// </summary>
    /// <remarks>
    /// When triggered:
    /// - Plays the boss intro animation
    /// - Starts boss-specific background music
    /// - Marks the related quest as complete or progressed
    /// </remarks>
    public void StartBoss()
    {
        // Play boss intro animation
        animator.Play("Start");

        // Start boss music
        SoundService.Instance?.Play("BossMusic");

        // Update quest progression
        questService.SatisfyQuest("FirstCat");
    }
}