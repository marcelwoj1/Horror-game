using UnityEngine;
using System.Collections;

public class AnimatableObject : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private string _animationName;
    [SerializeField] private bool _playOnStart = false;
    
    [Header("Delay Settings")]
    [Tooltip("Minimum delay in seconds before playing the animation")]
    [SerializeField] private float _minDelay = 0f;
    [Tooltip("Maximum delay in seconds before playing the animation")]
    [SerializeField] private float _maxDelay = 0f;
    [Tooltip("If true, delay will be applied between each loop iteration")]
    [SerializeField] private bool _delayBetweenLoops = false;
    
    private SpriteAnimator _animator;
    private bool _hasPlayed = false;
    private bool _isLooping = false;
    private Coroutine _loopCoroutine = null;

    void Awake()
    {
        _animator = GetComponent<SpriteAnimator>();
        
        if (_animator == null)
        {
            Debug.LogError($"SpriteAnimator component not found on {gameObject.name}");
        }
    }

    void Start()
    {
        if (_playOnStart && !_hasPlayed)
        {
            PlayAnimationWithDelay();
        }
    }

    void Update()
    {
        // Check if animation has finished and we need to loop with delay
        if (_isLooping && _animator != null)
        {
            // Check if the animation has stopped playing (reached the end)
            if (!IsAnimationPlaying())
            {
                _isLooping = false;
                // Restart with delay
                PlayAnimationWithDelay();
            }
        }
    }

    private bool IsAnimationPlaying()
    {
        // Check if the animator is currently playing an animation
        return _animator != null && _animator.IsPlaying;
    }

    /// <summary>
    /// Plays the specified animation with a random delay between min and max delay.
    /// </summary>
    public void PlayAnimationWithDelay()
    {
        if (_loopCoroutine != null)
        {
            StopCoroutine(_loopCoroutine);
        }
        float delay = Random.Range(_minDelay, _maxDelay);
        _loopCoroutine = StartCoroutine(PlayAnimationAfterDelay(delay, _animationName));
    }

    /// <summary>
    /// Plays a specific animation by name with a random delay.
    /// </summary>
    public void PlayAnimationWithDelay(string animationName)
    {
        if (_loopCoroutine != null)
        {
            StopCoroutine(_loopCoroutine);
        }
        float delay = Random.Range(_minDelay, _maxDelay);
        _loopCoroutine = StartCoroutine(PlayAnimationAfterDelay(delay, animationName));
    }

    /// <summary>
    /// Plays the specified animation immediately. Works with both normal and ping-pong animations.
    /// </summary>
    public void PlayAnimation()
    {
        if (_animator != null && !string.IsNullOrEmpty(_animationName))
        {
            _animator.Play(_animationName);
            _hasPlayed = true;
        }
    }

    /// <summary>
    /// Plays a specific animation by name.
    /// </summary>
    public void PlayAnimation(string animationName)
    {
        if (_animator != null && !string.IsNullOrEmpty(animationName))
        {
            _animator.Play(animationName);
            _hasPlayed = true;
        }
    }

    /// <summary>
    /// Resets the animation state so it can be played again.
    /// </summary>
    public void ResetAnimation()
    {
        _hasPlayed = false;
    }

    /// <summary>
    /// Coroutine that waits for a delay before playing the animation.
    /// </summary>
    private IEnumerator PlayAnimationAfterDelay(float delay, string animationName)
    {
        yield return new WaitForSeconds(delay);
        
        if (_animator != null && !string.IsNullOrEmpty(animationName))
        {
            // If we want delay between loops, disable the animation's internal looping
            // and handle it ourselves
            if (_delayBetweenLoops)
            {
                // Play with loop disabled so we can control when it restarts
                _animator.Play(animationName, false);
                _hasPlayed = true;
                _isLooping = true;
            }
            else
            {
                // Normal play - let the animation handle its own looping
                _animator.Play(animationName);
                _hasPlayed = true;
            }
        }
        
        _loopCoroutine = null;
    }
}
