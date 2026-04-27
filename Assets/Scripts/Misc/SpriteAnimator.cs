using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

[Serializable]
public class AnimationStringEvent : UnityEvent<string> { }

[Serializable]
public class AnimationSpecificEvents
{
    public string AnimationName;
    public UnityEvent onAnimationStart;
    public UnityEvent onAnimationEnd;
}

public class SpriteAnimator : MonoBehaviour
{
    [SerializeField] private List<CustomSpriteAnimation> _animations;
    [SerializeField] private string _startingAnimation;

    private SpriteRenderer _spriteRenderer;
    private CustomSpriteAnimation _currentAnimation;
    private int _currentFrame;
    private float _timer;
    private bool _isPlaying;
    private int _direction = 1;
    private bool? _loopOverride;

    private Dictionary<string, CustomSpriteAnimation> _animationDict = new Dictionary<string, CustomSpriteAnimation>();

    public bool IsPlaying => _isPlaying;
    public string CurrentAnimationName => _currentAnimation?.AnimationName;
    public int CurrentFrame => _currentFrame;

    public event Action<string, int> OnFrameChanged;
    public event Action<string> OnAnimationStarted;
    public event Action<string> OnAnimationEnded;

    public AnimationStringEvent onAnimationStart;
    public AnimationStringEvent onAnimationEnd;

    [SerializeField] private List<AnimationSpecificEvents> _specificEvents = new List<AnimationSpecificEvents>();

    private void TryInvokeSpecificStartEvent(string animName)
    {
        if (string.IsNullOrEmpty(animName)) return;
        foreach (var specEvent in _specificEvents)
            if (specEvent.AnimationName == animName)
                specEvent.onAnimationStart?.Invoke();
    }

    private void TryInvokeSpecificEndEvent(string animName)
    {
        if (string.IsNullOrEmpty(animName)) return;
        foreach (var specEvent in _specificEvents)
            if (specEvent.AnimationName == animName)
                specEvent.onAnimationEnd?.Invoke();
    }

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        foreach (var anim in _animations)
            if (anim != null && !string.IsNullOrEmpty(anim.AnimationName) && !_animationDict.ContainsKey(anim.AnimationName))
                _animationDict.Add(anim.AnimationName, anim);
    }

    private void Start()
    {
        if (!string.IsNullOrEmpty(_startingAnimation))
            Play(_startingAnimation);
    }

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

    private void EndAnimation()
    {
        _isPlaying = false;
        string animName = _currentAnimation?.AnimationName;
        OnAnimationEnded?.Invoke(animName);
        onAnimationEnd?.Invoke(animName);
        TryInvokeSpecificEndEvent(animName);
    }

    private void UpdateSprite()
    {
        if (_currentAnimation.Frames.Length > _currentFrame)
        {
            _spriteRenderer.sprite = _currentAnimation.Frames[_currentFrame];
            OnFrameChanged?.Invoke(_currentAnimation.AnimationName, _currentFrame);
        }
    }

    public void Play(string animationName) => Play(animationName, null, false);
    
    public void Play(string animationName, bool? overrideLoop) => Play(animationName, overrideLoop, false);

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

    public void Stop()
    {
        if (_isPlaying) EndAnimation();
    }

    public void SetFlip(bool flipX)
    {
        if (_spriteRenderer != null) _spriteRenderer.flipX = flipX;
    }
}
