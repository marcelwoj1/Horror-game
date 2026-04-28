using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Triggers sound effects at specific animation frames.
/// </summary>
/// <remarks>
/// This system:
/// - Listens for frame change events from a SpriteAnimator
/// - Matches animation names and frame indices
/// - Plays corresponding sound effects using SoundService
///
/// Designed to be data-driven via the inspector,
/// allowing designers to assign sounds without modifying code.
/// </remarks>
public class AnimationSoundTrigger : MonoBehaviour
{
    /// <summary>
    /// Defines a mapping between animation frames and sound effects.
    /// </summary>
    [System.Serializable]
    public class FrameSoundEntry
    {
        /// <summary>Name of the animation clip.</summary>
        public string AnimationName;

        /// <summary>Frame indices that should trigger a sound.</summary>
        public int[] FrameIndices;

        /// <summary>Name of the sound group to play.</summary>
        public string SoundGroupName;

        /// <summary>Cached set of frame indices for efficient lookup.</summary>
        [System.NonSerialized] public HashSet<int> FrameSet;
    }

    /// <summary>List of frame-to-sound mappings.</summary>
    [SerializeField] private List<FrameSoundEntry> _entries = new List<FrameSoundEntry>();

    /// <summary>Reference to the animator providing frame events.</summary>
    private SpriteAnimator _animator;

    /// <summary>
    /// Initialises animator reference and prepares lookup data.
    /// </summary>
    private void Awake()
    {
        _animator = GetComponent<SpriteAnimator>();
        if (_animator == null) return;

        // Convert frame arrays into hash sets for fast lookup
        foreach (var entry in _entries)
            entry.FrameSet = new HashSet<int>(entry.FrameIndices);

        _animator.OnFrameChanged += HandleFrameChanged;
    }

    /// <summary>
    /// Cleans up event subscription when object is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        if (_animator != null)
            _animator.OnFrameChanged -= HandleFrameChanged;
    }

    /// <summary>
    /// Handles animation frame changes and triggers sounds when appropriate.
    /// </summary>
    /// <param name="animationName">Name of the current animation.</param>
    /// <param name="frameIndex">Index of the current frame.</param>
    private void HandleFrameChanged(string animationName, int frameIndex)
    {
        if (SoundService.Instance == null) return;

        foreach (var entry in _entries)
        {
            if (entry.AnimationName == animationName && entry.FrameSet.Contains(frameIndex))
            {
                SoundService.Instance.Play(entry.SoundGroupName, (Vector2)transform.position, gameObject);
            }
        }
    }
}