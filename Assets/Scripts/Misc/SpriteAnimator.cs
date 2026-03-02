using System;
using UnityEngine;
using System.Collections.Generic;

public class SpriteAnimator : MonoBehaviour
{
    [Header("Animations")]
    [SerializeField] private List<CustomSpriteAnimation> _animations;
    [SerializeField] private string _startingAnimation;

    // Components
    private SpriteRenderer _spriteRenderer;

    // State
    private CustomSpriteAnimation _currentAnimation;
    private int _currentFrame;
    private float _timer;
    private bool _isPlaying = false;
    private int _direction = 1; // 1 for forward, -1 for backward (used for ping-pong)
    private bool? _loopOverride = null; // If set, overrides the animation's Loop setting

    private Dictionary<string, CustomSpriteAnimation> _animationDict = new Dictionary<string, CustomSpriteAnimation>();

    public bool IsPlaying => _isPlaying;
    public string CurrentAnimationName => _currentAnimation?.AnimationName;
    public int CurrentFrame => _currentFrame;

    /// <summary>Fired every frame change: (animationName, frameIndex)</summary>
    public event Action<string, int> OnFrameChanged;

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Initialize dictionary for fast lookup
        foreach (var anim in _animations)
        {
            if (anim != null && !string.IsNullOrEmpty(anim.AnimationName))
            {
                if (!_animationDict.ContainsKey(anim.AnimationName))
                {
                    _animationDict.Add(anim.AnimationName, anim);
                }
                else
                {
                    Debug.LogWarning($"Duplicate animation name '{anim.AnimationName}' on {gameObject.name}");
                }
            }
        }
    }

    void Start()
    {
        if (!string.IsNullOrEmpty(_startingAnimation))
        {
            Play(_startingAnimation);
        }
    }

    void Update()
    {
        if (!_isPlaying || _currentAnimation == null || _currentAnimation.Frames.Length == 0) return;

        _timer += Time.deltaTime;
        float frameDuration = 1f / _currentAnimation.FrameRate;

        if (_timer >= frameDuration)
        {
            _timer -= frameDuration;
            _currentFrame += _direction;

            // Handle ping-pong animations
            if (_currentAnimation.PingPong)
            {
                // Reached the end going forward
                if (_currentFrame >= _currentAnimation.Frames.Length)
                {
                    _currentFrame = _currentAnimation.Frames.Length - 2;
                    _direction = -1; // Reverse direction
                    
                    if (_currentFrame < 0) _currentFrame = 0;
                }
                // Reached the beginning going backward
                else if (_currentFrame < 0)
                {
                    bool shouldLoop = _loopOverride.HasValue ? _loopOverride.Value : _currentAnimation.Loop;
                    
                    if (shouldLoop)
                    {
                        _currentFrame = 1;
                        _direction = 1; // Go forward again
                    }
                    else
                    {
                        _currentFrame = 0;
                        _isPlaying = false;
                    }
                }
            }
            // Handle normal animations
            else
            {
                if (_currentFrame >= _currentAnimation.Frames.Length)
                {
                    bool shouldLoop = _loopOverride.HasValue ? _loopOverride.Value : _currentAnimation.Loop;
                    
                    if (shouldLoop)
                    {
                        _currentFrame = 0;
                    }
                    else
                    {
                        _currentFrame = _currentAnimation.Frames.Length - 1;
                        _isPlaying = false;
                    }
                }
            }

            UpdateSprite();
        }
    }

    private void UpdateSprite()
    {
        if (_currentAnimation.Frames.Length > _currentFrame)
        {
            _spriteRenderer.sprite = _currentAnimation.Frames[_currentFrame];
            OnFrameChanged?.Invoke(_currentAnimation.AnimationName, _currentFrame);
        }
    }

    /// <summary>
    /// Plays an animation by name.
    /// </summary>
    public void Play(string animationName)
    {
        Play(animationName, null, false);
    }

    /// <summary>
    /// Plays an animation by name with optional loop override.
    /// </summary>
    public void Play(string animationName, bool? overrideLoop)
    {
        Play(animationName, overrideLoop, false);
    }

    /// <summary>
    /// Plays an animation by name with optional loop override and force play option.
    /// </summary>
    public void Play(string animationName, bool? overrideLoop, bool forcePlay)
    {
        // Don't restart if already playing the same thing (unless we're changing the loop override)
        if (_currentAnimation != null && _currentAnimation.AnimationName == animationName && _isPlaying && overrideLoop == _loopOverride)
            return;

        // If the current animation is uninterruptible and still playing, bypass unless forcePlay is true
        if (!forcePlay && _isPlaying && _currentAnimation != null && _currentAnimation.Uninterruptible)
            return;

        if (_animationDict.TryGetValue(animationName, out CustomSpriteAnimation anim))
        {
            _currentAnimation = anim;
            _currentFrame = 0;
            _timer = 0f;
            _direction = 1; // Always start going forward
            _loopOverride = overrideLoop; // Set the loop override
            _isPlaying = true;
            
            UpdateSprite();
        }
        else
        {
            Debug.LogWarning($"Animation '{animationName}' not found on {gameObject.name}");
        }
    }

    /// <summary>
    /// Stops the current animation.
    /// </summary>
    public void Stop()
    {
        _isPlaying = false;
    }

    /// <summary>
    /// Flips the sprite horizontally.
    /// </summary>
    public void SetFlip(bool flipX)
    {
        if (_spriteRenderer != null)
        {
            _spriteRenderer.flipX = flipX;
        }
    }
}
