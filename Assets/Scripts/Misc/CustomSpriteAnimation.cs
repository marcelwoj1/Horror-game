using UnityEngine;

/// <summary>
/// Defines a custom sprite animation as a reusable data asset.
/// </summary>
/// <remarks>
/// This ScriptableObject allows animations to be created and configured
/// directly in the Unity editor. It supports:
/// - Frame-based animation using sprite arrays
/// - Adjustable playback speed
/// - Looping and ping-pong behaviour
/// - Optional uninterruptible playback control
///
/// Designed to be used with animation systems such as SpriteAnimator.
/// </remarks>
[CreateAssetMenu(fileName = "New Custom Sprite Animation", menuName = "Custom/Sprite Animation")]
public class CustomSpriteAnimation : ScriptableObject
{
    /// <summary>Name of the animation.</summary>
    public string AnimationName;

    /// <summary>Array of sprites representing animation frames.</summary>
    public Sprite[] Frames;

    /// <summary>Playback speed in frames per second.</summary>
    public float FrameRate = 10f;

    /// <summary>Determines whether the animation loops continuously.</summary>
    public bool Loop = true;

    /// <summary>
    /// If true, the animation reverses direction when reaching the end.
    /// </summary>
    public bool PingPong = false;

    /// <summary>
    /// If true, the animation must finish before another can interrupt it.
    /// </summary>
    public bool Uninterruptible = false;
}