using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Represents a group of related sound clips with shared playback settings.
/// </summary>
/// <remarks>
/// Allows variation by randomly selecting clips and applying pitch variance.
/// Used by SoundService for flexible audio playback.
/// </remarks>
[System.Serializable]
public class SoundGroup
{
    /// <summary>Name used to reference this sound group.</summary>
    [Tooltip("Sound group")]
    public string GroupName;

    /// <summary>Collection of clips to randomly choose from.</summary>
    [Tooltip("One or more clips; a random one is chosen each play")]
    public AudioClip[] Clips;

    /// <summary>Base volume of the sound group.</summary>
    [Range(0.1f, 5f)]
    public float Volume = 1f;

    /// <summary>Base pitch of the sound group.</summary>
    [Range(0.1f, 2f)]
    public float Pitch = 1f;

    /// <summary>Random pitch variation applied each time the sound plays.</summary>
    [Range(0f, 0.5f)]
    [Tooltip("Randomly swing the pitch either negatively or positively every time the sound plays")]
    public float PitchVariance = 0f;

    /// <summary>Determines if the sound should loop.</summary>
    [Tooltip("Loop the clip?")]
    public bool Loop = false;
}

/// <summary>
/// Centralised audio manager responsible for playing sound effects.
/// </summary>
/// <remarks>
/// Features:
/// - Singleton pattern for global access
/// - Sound grouping with random clip selection
/// - Global and positional audio playback
/// - Pitch variation for natural sound diversity
/// - Automatic cleanup of non-looping sounds
///
/// Supports both 2D (UI/global) and 3D (world-positioned) audio.
/// </remarks>
public class SoundService : MonoBehaviour
{
    /// <summary>Singleton instance of the SoundService.</summary>
    public static SoundService Instance { get; private set; }

    /// <summary>List of all configured sound groups.</summary>
    [Header("Sound Groups")]
    [SerializeField] private List<SoundGroup> _soundGroups = new List<SoundGroup>();

    /// <summary>Global volume multiplier applied to all sounds.</summary>
    [Header("Settings")]
    [Tooltip("Global volume multiplier")]
    [SerializeField] private float _globalVolume = 1f;

    /// <summary>Dictionary for fast lookup of sound groups by name.</summary>
    private Dictionary<string, SoundGroup> _groupDict = new Dictionary<string, SoundGroup>();

    /// <summary>
    /// Initialises the singleton instance and builds the sound group lookup dictionary.
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        foreach (var group in _soundGroups)
        {
            if (group == null || string.IsNullOrEmpty(group.GroupName)) continue;

            if (!_groupDict.ContainsKey(group.GroupName))
                _groupDict.Add(group.GroupName, group);
        }
    }

    /// <summary>
    /// Plays a sound group globally (non-positional).
    /// </summary>
    /// <param name="groupName">Name of the sound group.</param>
    public void Play(string groupName)
    {
        if (!TryGetGroup(groupName, out SoundGroup group)) return;
        PlayGlobal(group);
    }

    /// <summary>
    /// Plays a global sound with an additional volume multiplier.
    /// </summary>
    /// <param name="groupName">Name of the sound group.</param>
    /// <param name="volumeMultiplier">Additional volume scaling.</param>
    public void PlayGlobal(string groupName, float volumeMultiplier = 1f)
    {
        if (!TryGetGroup(groupName, out SoundGroup group)) return;
        
        AudioSource src = CreateSource();
        ConfigureSource(src, group);
        src.volume *= volumeMultiplier * _globalVolume;
        src.transform.SetParent(transform, false);
        src.spatialBlend = 0f;
        src.Play();

        if (!group.Loop)
            Destroy(src.gameObject, src.clip.length / src.pitch + 0.1f);
    }

    /// <summary>
    /// Plays a positional sound at a given world position.
    /// </summary>
    /// <param name="groupName">Name of the sound group.</param>
    /// <param name="position">World position to play the sound.</param>
    public void Play(string groupName, Vector2 position)
    {
        Play(groupName, position, null);
    }

    /// <summary>
    /// Plays a positional sound, optionally attached to a GameObject.
    /// </summary>
    /// <param name="groupName">Name of the sound group.</param>
    /// <param name="position">Fallback world position.</param>
    /// <param name="attachTo">Optional GameObject to attach the sound to.</param>
    public void Play(string groupName, Vector2 position, GameObject attachTo)
    {
        if (!TryGetGroup(groupName, out SoundGroup group)) return;

        AudioSource src = CreateSource();
        ConfigureSource(src, group);
        src.volume *= _globalVolume;

        if (attachTo != null)
        {
            src.transform.SetParent(attachTo.transform, false);
            src.transform.localPosition = Vector3.zero;
        }
        else
        {
            src.transform.SetParent(transform, false);
            src.transform.position = new Vector3(position.x, position.y, 0f);
        }

        src.spatialBlend = 1f;
        src.Play();

        if (!group.Loop)
            Destroy(src.gameObject, src.clip.length / src.pitch + 0.1f);
    }

    /// <summary>
    /// Internal helper for playing non-positional audio.
    /// </summary>
    private void PlayGlobal(SoundGroup group)
    {
        AudioSource src = CreateSource();
        ConfigureSource(src, group);
        src.volume *= _globalVolume;
        src.transform.SetParent(transform, false);
        src.spatialBlend = 0f;
        src.Play();

        if (!group.Loop)
            Destroy(src.gameObject, src.clip.length / src.pitch + 0.1f);
    }

    /// <summary>
    /// Attempts to retrieve a sound group by name.
    /// </summary>
    /// <param name="groupName">Name of the sound group.</param>
    /// <param name="group">Output sound group.</param>
    /// <returns>True if found, otherwise false.</returns>
    private bool TryGetGroup(string groupName, out SoundGroup group)
    {
        if (_groupDict.TryGetValue(groupName, out group)) return true;

        return false;
    }

    /// <summary>
    /// Creates a temporary AudioSource for playback.
    /// </summary>
    /// <returns>Newly created AudioSource.</returns>
    private AudioSource CreateSource()
    {
        GameObject go = new GameObject("OneShotAudioSource");
        go.transform.SetParent(transform, false);
        AudioSource src = go.AddComponent<AudioSource>();
        src.playOnAwake = false;
        return src;
    }

    /// <summary>
    /// Configures an AudioSource using settings from a SoundGroup.
    /// </summary>
    /// <param name="src">AudioSource to configure.</param>
    /// <param name="group">SoundGroup containing settings.</param>
    private void ConfigureSource(AudioSource src, SoundGroup group)
    {
        if (group.Clips == null || group.Clips.Length == 0)
        {
            return;
        }

        src.clip = group.Clips[Random.Range(0, group.Clips.Length)];
        src.volume = group.Volume;
        
        float randomPitch = group.Pitch + Random.Range(-group.PitchVariance, group.PitchVariance);
        src.pitch = Mathf.Clamp(randomPitch, 0.1f, 3f);
        
        src.loop = group.Loop;
    }
}