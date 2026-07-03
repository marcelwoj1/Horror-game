using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

/// <summary>
/// Manages player health, damage handling, visual feedback,
/// and death behaviour.
/// </summary>
/// <remarks>
/// This system:
/// - Tracks current and maximum health
/// - Applies damage and healing
/// - Triggers camera shake and visual feedback on damage
/// - Applies knockback using physics
/// - Uses events to notify other systems of health changes
///
/// Visual feedback includes a vignette effect and animation responses.
/// </remarks>
public class PlayerHealth : MonoBehaviour
{
    [Header("Variables")]

    /// <summary>Current player health.</summary>
    public int Health = 5;

    /// <summary>Maximum player health.</summary>
    public int MaxHealth = 5;

    [Header("Components")]

    /// <summary>Handles animation playback.</summary>
    private SpriteAnimator _animator;

    /// <summary>Coroutine for hurt flash effect.</summary>
    private Coroutine _hurtFlashRoutine;

    /// <summary>UI graphic used for vignette effect.</summary>
    private Graphic _vignetteGraphic;

    /// <summary>Rigidbody used for knockback physics.</summary>
    private Rigidbody2D _rigidBody;

    /// <summary>GameObject containing the hurt vignette UI.</summary>
    public GameObject HurtVignette;

    [Header("Actions")]

    /// <summary>Invoked when the player dies.</summary>
    public Action OnDeath;

    /// <summary>Invoked when player health changes.</summary>
    public Action OnHealthChanged;

    [Header("Config")]

    /// <summary>Magnitude of camera shake when damaged.</summary>
    [SerializeField] private float damageShakeMagnitude = 1.5f;

    /// <summary>Duration of vignette fade-in effect.</summary>
    [SerializeField] private float hurtFadeInDuration = 0.1f;

    /// <summary>Duration of vignette fade-out effect.</summary>
    [SerializeField] private float hurtFadeOutDuration = 0.5f;

    /// <summary>Maximum alpha value for vignette effect.</summary>
    [SerializeField] private float hurtFadeMaxAlpha = 0.8f;

    [Header("Scripts")]

    /// <summary>Reference to movement system.</summary>
    private Movement _movement;

    /// <summary>Reference to camera system.</summary>
    private CameraTrack _cameraTrack;

    /// <summary>Reference to player manager.</summary>
    private PlayerManager _playerManager;

    [Header("Vignette")]
    public Material _vignetteMaterial;
    public float _fallOff = 0;
    [SerializeField] private float heartbeatStrength = 0.5f;
    [SerializeField] private float heartbeatSpeed = 1.2f;

    private float baseIntensity;

    /// <summary>
    /// Initialises component references and UI state.
    /// </summary>
    void Start()
    {
        _animator = GetComponent<SpriteAnimator>();
        _movement = GetComponent<Movement>();
        _playerManager = GetComponent<PlayerManager>();
        _cameraTrack = FindFirstObjectByType<CameraTrack>();
        _rigidBody = GetComponent<Rigidbody2D>();

        if (HurtVignette != null)
        {
            _vignetteGraphic = HurtVignette.GetComponentInChildren<Graphic>();
            HurtVignette.SetActive(false);

            if (_vignetteGraphic != null)
            {
                Color c = _vignetteGraphic.color;
                c.a = 0;
                _vignetteGraphic.color = c;
            }
        }
        _vignetteMaterial.SetFloat("_Falloff", _fallOff);
    }

    /// <summary>
    /// Applies damage to the player and triggers feedback effects.
    /// </summary>
    /// <param name="damage">Amount of damage taken.</param>
    /// <param name="knockback">Force applied to the player.</param>
    /// <remarks>
    /// Damage will not be applied if bug spray is active.
    /// Includes:
    /// - Health reduction
    /// - Camera shake
    /// - Visual feedback (vignette)
    /// - Knockback force
    /// </remarks>
    public void TakeDamage(int damage, Vector2 knockback)
    {
        if (_playerManager.IsBugSprayActive) return;

        Health -= damage;
        OnHealthChanged?.Invoke();
        _fallOff += 0.35f * damage;
        _vignetteMaterial.color = new Color(0.16f, 0f, 0.02f, 1f);
        _vignetteMaterial.SetFloat("_Falloff", _fallOff);

        SoundService.Instance?.Play("PlayerHurt");

        if (Health <= 0)
        {
            _animator.Play("Death");
            _playerManager.AllowMovement = false;

            OnDeath?.Invoke();
            return;
        }

        // Camera feedback
        _cameraTrack?.Shake(damageShakeMagnitude);

        // Visual feedback
        if (HurtVignette != null)
        {
            if (_hurtFlashRoutine != null) StopCoroutine(_hurtFlashRoutine);
            _hurtFlashRoutine = StartCoroutine(HurtFadeRoutine());
        }

        // Apply knockback
        _rigidBody.linearVelocity = Vector2.zero;
        _rigidBody.AddForce(knockback, ForceMode2D.Impulse);

        StartCoroutine(KnockbackRoutine());
    }

    /// <summary>
    /// Handles vignette fade-in and fade-out effect.
    /// </summary>
    /// <returns>Coroutine controlling visual feedback.</returns>
    private IEnumerator HurtFadeRoutine()
    {
        HurtVignette.SetActive(true);
        float elapsed = 0;

        if (_vignetteGraphic != null)
        {
            float startAlpha = _vignetteGraphic.color.a;

            // Fade in
            while (elapsed < hurtFadeInDuration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(startAlpha, hurtFadeMaxAlpha, elapsed / hurtFadeInDuration);
                SetVignetteAlpha(alpha);
                yield return null;
            }

            // Fade out
            elapsed = 0;
            while (elapsed < hurtFadeOutDuration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(hurtFadeMaxAlpha, 0, elapsed / hurtFadeOutDuration);
                SetVignetteAlpha(alpha);
                yield return null;
            }

            SetVignetteAlpha(0);
        }
        else
        {
            yield return new WaitForSeconds(hurtFadeInDuration + hurtFadeOutDuration);
        }

        HurtVignette.SetActive(false);
        _hurtFlashRoutine = null;
    }

    /// <summary>
    /// Updates vignette transparency.
    /// </summary>
    /// <param name="alpha">Alpha value to apply.</param>
    private void SetVignetteAlpha(float alpha)
    {
        if (_vignetteGraphic != null)
        {
            Color c = _vignetteGraphic.color;
            c.a = alpha;
            _vignetteGraphic.color = c;
        }
    }

    /// <summary>
    /// Temporarily disables player control during knockback.
    /// </summary>
    IEnumerator KnockbackRoutine()
    {
        if (_movement != null) _movement.isKnockedBack = true;

        yield return new WaitForSeconds(0.35f);

        if (_movement != null) _movement.isKnockedBack = false;
    }

    /// <summary>
    /// Restores player health.
    /// </summary>
    /// <param name="healAmount">Amount of health to restore.</param>
    public void Heal(int healAmount)
    {
        Health += healAmount;
        OnHealthChanged?.Invoke();

        _fallOff -= 0.35f * healAmount;
        _vignetteMaterial.color = new Color(0.16f, 0f, 0.02f, 1f);
        _vignetteMaterial.SetFloat("_Falloff", _fallOff);

        if (Health > MaxHealth)
        {
            Health = MaxHealth;
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(1, Vector2.zero);
        }
        if(Input.GetKeyDown(KeyCode.L))
        {
            Heal(1);
        }

        float baseIntensity = 0.94f;

        float pulse = 0f;

        /// <summary>
        /// Controls the heartbeat effect when player has 1 health
        /// </summary>
        if (Health == 1)
        {
            //timing for the heartbeat
            float t = Time.time * 2f;

            //using sin wave to create a heartbeat effect
            float beat1 = Mathf.Pow(Mathf.Max(0, Mathf.Sin(t * 6f)), 10f);
            float beat2 = Mathf.Pow(Mathf.Max(0, Mathf.Sin((t - 0.15f) * 6f)), 10f);

            //adding the two beat together to create a heartbeat effect
            pulse = (beat1 + beat2 * 0.7f) * 0.4f;

            _vignetteMaterial.SetFloat("_Falloff", baseIntensity + pulse);
            
        }
    }
}