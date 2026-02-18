using UnityEngine;
using System.Collections.Generic;

// ---------------------------------------------------------------------------
// AnimationSoundTrigger
// Attach to any GameObject that has a SpriteAnimator.
// Configure which animation frames should trigger which SoundService group.
// ---------------------------------------------------------------------------
public class AnimationSoundTrigger : MonoBehaviour
{
    [System.Serializable]
    public class FrameSoundEntry
    {
        [Tooltip("Animation name to listen for (e.g. \"Walk\")")]
        public string AnimationName;

        [Tooltip("Frame indices that trigger the sound (e.g. 0, 1)")]
        public int[] FrameIndices;

        [Tooltip("SoundService group to play (e.g. \"FootstepPlayer\")")]
        public string SoundGroupName;

        [Tooltip("Play as a local (positional) sound")]
        public bool IsLocal = true;

        [Tooltip("Attach the audio source to this object so it follows")]
        public bool AttachToObject = false;

        // Runtime lookup set built from FrameIndices for O(1) checks
        [System.NonSerialized] public HashSet<int> FrameSet;
    }

    [Header("Frame → Sound Mappings")]
    [SerializeField] private List<FrameSoundEntry> _entries = new List<FrameSoundEntry>();

    private SpriteAnimator _animator;

    private void Awake()
    {
        _animator = GetComponent<SpriteAnimator>();

        if (_animator == null)
        {
            Debug.LogWarning($"[AnimationSoundTrigger] No SpriteAnimator found on {gameObject.name}.");
            return;
        }

        // Build HashSets for fast frame-index lookup
        foreach (var entry in _entries)
        {
            entry.FrameSet = new HashSet<int>(entry.FrameIndices);
        }

        _animator.OnFrameChanged += HandleFrameChanged;
    }

    private void OnDestroy()
    {
        if (_animator != null)
            _animator.OnFrameChanged -= HandleFrameChanged;
    }

    private void HandleFrameChanged(string animationName, int frameIndex)
    {
        if (SoundService.Instance == null) return;

        foreach (var entry in _entries)
        {
            if (entry.AnimationName != animationName) continue;
            if (!entry.FrameSet.Contains(frameIndex)) continue;

            if (entry.IsLocal)
            {
                GameObject attachTarget = entry.AttachToObject ? gameObject : null;
                SoundService.Instance.Play(entry.SoundGroupName, (Vector2)transform.position, attachTarget);
            }
            else
            {
                SoundService.Instance.Play(entry.SoundGroupName);
            }
        }
    }
}
