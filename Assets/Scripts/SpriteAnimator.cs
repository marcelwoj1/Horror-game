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

    private Dictionary<string, CustomSpriteAnimation> _animationDict = new Dictionary<string, CustomSpriteAnimation>();

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
            _currentFrame++;

            if (_currentFrame >= _currentAnimation.Frames.Length)
            {
                if (_currentAnimation.Loop)
                {
                    _currentFrame = 0;
                }
                else
                {
                    _currentFrame = _currentAnimation.Frames.Length - 1;
                    _isPlaying = false;
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
        }
    }

    /// <summary>
    /// Plays an animation by name.
    /// </summary>
    public void Play(string animationName)
    {
        // Don't restart if already playing the same thing
        if (_currentAnimation != null && _currentAnimation.AnimationName == animationName && _isPlaying)
            return;

        if (_animationDict.TryGetValue(animationName, out CustomSpriteAnimation anim))
        {
            _currentAnimation = anim;
            _currentFrame = 0;
            _timer = 0f;
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
