using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int Health = 5;
    public int MaxHealth = 5;
    public GameObject HurtVignette;
    public Action OnDeath;
    public Action OnHealthChanged;
    
    [Header("Feedback")]
    [SerializeField] private float damageShakeMagnitude = 1.5f;
    [SerializeField] private float hurtFadeInDuration = 0.1f;
    [SerializeField] private float hurtFadeOutDuration = 0.5f;
    [SerializeField] private float hurtFadeMaxAlpha = 0.8f;
    
    private SpriteAnimator _animator;
    private Movement _movement;
    private CameraTrack _cameraTrack;
    private Coroutine _hurtFlashRoutine;
    private Graphic _vignetteGraphic;

    void Start()
    {
        _animator = GetComponent<SpriteAnimator>();
        _movement = GetComponent<Movement>();
        _cameraTrack = FindFirstObjectByType<CameraTrack>();
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
    }

    public void TakeDamage(int damage)
    {
        if (Health <= 0) return;
        Health -= damage;
        OnHealthChanged?.Invoke();
        //_animator.Play("Hurt");
        SoundService.Instance?.Play("PlayerHurt");
        _cameraTrack?.Shake(damageShakeMagnitude);
        
        if (HurtVignette != null)
        {
            if (_hurtFlashRoutine != null) StopCoroutine(_hurtFlashRoutine);
            _hurtFlashRoutine = StartCoroutine(HurtFadeRoutine());
        }

        if (Health <= 0)
        {
            _animator.Play("Death");
            _movement.AllowMovement = false;
            
        }
    }

    private IEnumerator HurtFadeRoutine()
    {
        HurtVignette.SetActive(true);
        float elapsed = 0;

        if (_vignetteGraphic != null)
        {
            float startAlpha = _vignetteGraphic.color.a;
            
            // Fade In
            while (elapsed < hurtFadeInDuration)
            {
                elapsed += Time.deltaTime;
                float currentAlpha = Mathf.Lerp(startAlpha, hurtFadeMaxAlpha, elapsed / hurtFadeInDuration);
                SetVignetteAlpha(currentAlpha);
                yield return null;
            }
            SetVignetteAlpha(hurtFadeMaxAlpha);

            // Fade Out
            elapsed = 0;
            while (elapsed < hurtFadeOutDuration)
            {
                elapsed += Time.deltaTime;
                float currentAlpha = Mathf.Lerp(hurtFadeMaxAlpha, 0, elapsed / hurtFadeOutDuration);
                SetVignetteAlpha(currentAlpha);
                yield return null;
            }
            SetVignetteAlpha(0);
        }
        else
        {
            // Simple fallback if no Graphic is found
            yield return new WaitForSeconds(hurtFadeInDuration + hurtFadeOutDuration);
        }

        HurtVignette.SetActive(false);
        _hurtFlashRoutine = null;
    }

    private void SetVignetteAlpha(float alpha)
    {
        if (_vignetteGraphic != null)
        {
            Color c = _vignetteGraphic.color;
            c.a = alpha;
            _vignetteGraphic.color = c;
        }
    }

    public void TakeDamage(int damage, Vector2 knockback)
    {
        TakeDamage(damage);
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            if (Health > 0)
            {
                rb.linearVelocity = Vector2.zero;
                rb.AddForce(knockback, ForceMode2D.Impulse);
                StartCoroutine(KnockbackRoutine());
            }
        }
    }

    IEnumerator KnockbackRoutine()
    {
        if (_movement != null) _movement.isKnockedBack = true;
        yield return new WaitForSeconds(0.35f);
        if (_movement != null) _movement.isKnockedBack = false;
    }

    public void Heal(int healAmount)
    {
        Health += healAmount;
        OnHealthChanged?.Invoke();
        if (Health > MaxHealth)
        {
            Health = MaxHealth;
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(1);
        }
    }
}
