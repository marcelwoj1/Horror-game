using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

/// <summary>
/// UnityEvent that passes the animation name as a string.
/// </summary>
[Serializable]
public class AnimationStringEvent : UnityEvent<string> { }

/// <summary>
/// Defines animation-specific UnityEvents for start and end.
/// </summary>
[Serializable]
public class AnimationSpecificEvents
{
    /// <summary>Name of the animation.</summary>
    public string AnimationName;

    /// <summary>Event triggered when the animation starts.</summary>
    public UnityEvent onAnimationStart;

    /// <summary>Event triggered when the animation ends.</summary>
    public UnityEvent onAnimationEnd;
}

/// <summary>
/// Handles sprite-based animation playback using frame sequences.
/// </summary>
/// <remarks>
/// This system:
/// - Plays animations defined via CustomSpriteAnimation assets
/// - Supports looping and ping-pong playback
/// - Allows event-driven animation hooks (start, end, frame change)
/// - Supports interruptible and uninterruptible animations
/// - Provides both C# events and UnityEvents for flexibility
///
/// Designed to be modular, efficient, and inspector-friendly.
/// </remarks>
public class SpriteAnimator : MonoBehaviour
{
    /// <summary>List of available animations.</summary>
    [SerializeField] private List<CustomSpriteAnimation> _animations;

    /// <summary>Name of the animation to play on start.</summary>
    [SerializeField] private string _startingAnimation;

    /// <summary>Renderer used to display sprite frames.</summary>
    private SpriteRenderer _spriteRenderer;

    /// <summary>Currently active animation.</summary>
    private CustomSpriteAnimation _currentAnimation;

    /// <summary>Current frame index.</summary>
    private int _currentFrame;

    /// <summary>Timer used to control frame updates.</summary>
    private float _timer;

    /// <summary>Indicates whether an animation is currently playing.</summary>
    private bool _isPlaying;

    /// <summary>Playback direction (1 = forward, -1 = backward).</summary>
    private int _direction = 1;

    /// <summary>Optional override for loop behaviour.</summary>
    private bool? _loopOverride;

    /// <summary>Dictionary for fast animation lookup by name.</summary>
    private Dictionary<string, CustomSpriteAnimation> _animationDict = new Dictionary<string, CustomSpriteAnimation>();

    /// <summary>Indicates if an animation is currently playing.</summary>
    public bool IsPlaying => _isPlaying;

    /// <summary>Name of the current animation.</summary>
    public string CurrentAnimationName => _currentAnimation?.AnimationName;

    /// <summary>Current frame index.</summary>
    public int CurrentFrame => _currentFrame;

    /// <summary>Event triggered when the animation frame changes.</summary>
    public event Action<string, int> OnFrameChanged;

    /// <summary>Event triggered when an animation starts.</summary>
    public event Action<string> OnAnimationStarted;

    /// <summary>Event triggered when an animation ends.</summary>
    public event Action<string> OnAnimationEnded;

    /// <summary>UnityEvent triggered on animation start.</summary>
    public AnimationStringEvent onAnimationStart;

    /// <summary>UnityEvent triggered on animation end.</summary>
    public AnimationStringEvent onAnimationEnd;

    /// <summary>List of animation-specific UnityEvents.</summary>
    [SerializeField] private List<AnimationSpecificEvents> _specificEvents = new List<AnimationSpecificEvents>();

    /// <summary>
    /// Invokes specific start events for a given animation.
    /// </summary>
    private void TryInvokeSpecificStartEvent(string animName)
    {
        if (string.IsNullOrEmpty(animName)) return;
        foreach (var specEvent in _specificEvents)
            if (specEvent.AnimationName == animName)
                specEvent.onAnimationStart?.Invoke();
    }

    /// <summary>
    /// Invokes specific end events for a given animation.
    /// </summary>
    private void TryInvokeSpecificEndEvent(string animName)
    {
        if (string.IsNullOrEmpty(animName)) return;
        foreach (var specEvent in _specificEvents)
            if (specEvent.AnimationName == animName)
                specEvent.onAnimationEnd?.Invoke();
    }

    /// <summary>
    /// Initialises the animator and builds the animation lookup dictionary.
    /// </summary>
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();

        foreach (var anim in _animations)
            if (anim != null && !string.IsNullOrEmpty(anim.AnimationName) && !_animationDict.ContainsKey(anim.AnimationName))
                _animationDict.Add(anim.AnimationName, anim);
    }

    /// <summary>
    /// Plays the starting animation if defined.
    /// </summary>
    private void Start()
    {
        if (!string.IsNullOrEmpty(_startingAnimation))
            Play(_startingAnimation);
    }

    /// <summary>
    /// Updates animation playback and frame progression.
    /// </summary>
    private void Update()
    {
        if (!_isPlaying || _currentAnimation == null || _currentAnimation.Frames.Length == 0) return;

        _timer += Time.deltaTime;
        float frameDuration = 1f / _currentAnimation.FrameRate;

        if (_timer >= frameDuration)
        {
            _timer -= frameDuration;
            _currentFrame += _direction;

            if (_currentAnimation.PingPong)
            {
                if (_currentFrame >= _currentAnimation.Frames.Length)
                {
                    _currentFrame = Mathf.Max(0, _currentAnimation.Frames.Length - 2);
                    _direction = -1;
                }
                else if (_currentFrame < 0)
                {
                    if (_loopOverride ?? _currentAnimation.Loop)
                    {
                        _currentFrame = 1;
                        _direction = 1;
                    }
                    else
                    {
                        _currentFrame = 0;
                        EndAnimation();
                    }
                }
            }
            else
            {
                if (_currentFrame >= _currentAnimation.Frames.Length)
                {
                    if (_loopOverride ?? _currentAnimation.Loop)
                        _currentFrame = 0;
                    else
                    {
                        _currentFrame = _currentAnimation.Frames.Length - 1;
                        EndAnimation();
                    }
                }
            }

            UpdateSprite();
        }
    }

    /// <summary>
    /// Ends the current animation and triggers events.
    /// </summary>
    private void EndAnimation()
    {
        _isPlaying = false;
        string animName = _currentAnimation?.AnimationName;

        OnAnimationEnded?.Invoke(animName);
        onAnimationEnd?.Invoke(animName);
        TryInvokeSpecificEndEvent(animName);
    }

    /// <summary>
    /// Updates the displayed sprite based on the current frame.
    /// </summary>
    private void UpdateSprite()
    {
        if (_currentAnimation.Frames.Length > _currentFrame)
        {
            _spriteRenderer.sprite = _currentAnimation.Frames[_currentFrame];
            OnFrameChanged?.Invoke(_currentAnimation.AnimationName, _currentFrame);
        }
    }

    /// <summary>Plays an animation using default settings.</summary>
    public void Play(string animationName) => Play(animationName, null, false);

    /// <summary>Plays an animation with loop override.</summary>
    public void Play(string animationName, bool? overrideLoop) => Play(animationName, overrideLoop, false);

    /// <summary>
    /// Plays an animation with full control over behaviour.
    /// </summary>
    public void Play(string animationName, bool? overrideLoop, bool forcePlay)
    {
        if (_currentAnimation != null && _currentAnimation.AnimationName == animationName && _isPlaying && overrideLoop == _loopOverride) return;
        if (!forcePlay && _isPlaying && _currentAnimation != null && _currentAnimation.Uninterruptible) return;

        if (_animationDict.TryGetValue(animationName, out CustomSpriteAnimation anim))
        {
            if (_isPlaying && _currentAnimation != null)
            {
                string prevName = _currentAnimation.AnimationName;
                OnAnimationEnded?.Invoke(prevName);
                onAnimationEnd?.Invoke(prevName);
                TryInvokeSpecificEndEvent(prevName);
            }

            _currentAnimation = anim;
            _currentFrame = 0;
            _timer = 0f;
            _direction = 1;
            _loopOverride = overrideLoop;
            _isPlaying = true;

            UpdateSprite();

            OnAnimationStarted?.Invoke(animationName);
            onAnimationStart?.Invoke(animationName);
            TryInvokeSpecificStartEvent(animationName);
        }
    }

    /// <summary>
    /// Stops the current animation.
    /// </summary>
    public void Stop()
    {
        if (_isPlaying) EndAnimation();
    }

    /// <summary>
    /// Flips the sprite horizontally.
    /// </summary>
    /// <param name="flipX">True to flip, false to reset.</param>
    public void SetFlip(bool flipX)
    {
        if (_spriteRenderer != null) _spriteRenderer.flipX = flipX;
    }
}