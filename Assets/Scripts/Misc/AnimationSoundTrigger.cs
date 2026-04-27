using UnityEngine;
using System.Collections.Generic;

public class AnimationSoundTrigger : MonoBehaviour
{
    [System.Serializable]
    public class FrameSoundEntry
    {
        public string AnimationName;
        public int[] FrameIndices;
        public string SoundGroupName;
        [System.NonSerialized] public HashSet<int> FrameSet;
    }

    [SerializeField] private List<FrameSoundEntry> _entries = new List<FrameSoundEntry>();
    private SpriteAnimator _animator;

    private void Awake()
    {
        _animator = GetComponent<SpriteAnimator>();
        if (_animator == null) return;

        foreach (var entry in _entries)
            entry.FrameSet = new HashSet<int>(entry.FrameIndices);

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
            if (entry.AnimationName == animationName && entry.FrameSet.Contains(frameIndex))
            {
                SoundService.Instance.Play(entry.SoundGroupName, (Vector2)transform.position, gameObject);
            }
        }
    }
}
