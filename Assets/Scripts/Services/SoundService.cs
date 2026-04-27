using UnityEngine;
using System.Collections;
using System.Collections.Generic;[System.Serializable]
public class SoundGroup
{
    [Tooltip("Sound group")]
    public string GroupName;

    [Tooltip("One or more clips; a random one is chosen each play")]
    public AudioClip[] Clips;

    [Range(0.1f, 5f)]
    public float Volume = 1f;

    [Range(0.1f, 2f)]
    public float Pitch = 1f;

    [Range(0f, 0.5f)]
    [Tooltip("Randomly swing the pitch either negatively or positively every time the sound plays")]
    public float PitchVariance = 0f;

    [Tooltip("Loop the clip?")]
    public bool Loop = false;
}

public class SoundService : MonoBehaviour
{
    public static SoundService Instance { get; private set; }

    [Header("Sound Groups")]
    [SerializeField] private List<SoundGroup> _soundGroups = new List<SoundGroup>();

    [Header("Settings")]
    [Tooltip("Global volume multiplier")]
    [SerializeField] private float _globalVolume = 1f;

    private Dictionary<string, SoundGroup> _groupDict = new Dictionary<string, SoundGroup>();

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

    public void Play(string groupName)
    {
        if (!TryGetGroup(groupName, out SoundGroup group)) return;
        PlayGlobal(group);
    }

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

    public void Play(string groupName, Vector2 position)
    {
        Play(groupName, position, null);
    }

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

    private bool TryGetGroup(string groupName, out SoundGroup group)
    {
        if (_groupDict.TryGetValue(groupName, out group)) return true;

        return false;
    }

    private AudioSource CreateSource()
    {
        GameObject go = new GameObject("OneShotAudioSource");
        go.transform.SetParent(transform, false);
        AudioSource src = go.AddComponent<AudioSource>();
        src.playOnAwake = false;
        return src;
    }

    private void ConfigureSource(AudioSource src, SoundGroup group)
    {
        if (group.Clips == null || group.Clips.Length == 0)
        {
            return;
        }

        src.clip   = group.Clips[Random.Range(0, group.Clips.Length)];
        src.volume = group.Volume;
        
        float randomPitch = group.Pitch + Random.Range(-group.PitchVariance, group.PitchVariance);
        src.pitch  = Mathf.Clamp(randomPitch, 0.1f, 3f);
        
        src.loop   = group.Loop;
    }
}


