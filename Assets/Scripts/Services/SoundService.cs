using UnityEngine;
using System.Collections;
using System.Collections.Generic;[System.Serializable]
public class SoundGroup
{
    [Tooltip("Key used in SoundManager.Play(\"...\")")]
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
    [Tooltip("Max simultaneous AudioSources in the pool")]
    [SerializeField] private int _poolSize = 16;

    private Dictionary<string, SoundGroup> _groupDict = new Dictionary<string, SoundGroup>();
    private Queue<AudioSource> _pool = new Queue<AudioSource>();

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
            else
                Debug.LogWarning($"[SoundService] Duplicate group name '{group.GroupName}' – skipping.");
        }

        for (int i = 0; i < _poolSize; i++)
            _pool.Enqueue(CreateSource());
    }

    public void Play(string groupName)
    {
        if (!TryGetGroup(groupName, out SoundGroup group)) return;
        PlayGlobal(group);
    }

    public void PlayGlobal(string groupName, float volumeMultiplier = 1f)
    {
        if (!TryGetGroup(groupName, out SoundGroup group)) return;
        
        AudioSource src = GetPooledSource();
        ConfigureSource(src, group);
        src.volume *= volumeMultiplier;
        src.transform.SetParent(transform, false);
        src.spatialBlend = 0f;
        src.Play();

        StartCoroutine(ReturnWhenDone(src, null));
    }

    public void Play(string groupName, Vector2 position)
    {
        Play(groupName, position, null);
    }

    public void Play(string groupName, Vector2 position, GameObject attachTo)
    {
        if (!TryGetGroup(groupName, out SoundGroup group)) return;

        AudioSource src = GetPooledSource();
        ConfigureSource(src, group);

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

        StartCoroutine(ReturnWhenDone(src, attachTo));
    }

    private void PlayGlobal(SoundGroup group)
    {
        AudioSource src = GetPooledSource();
        ConfigureSource(src, group);
        src.transform.SetParent(transform, false);
        src.spatialBlend = 0f;
        src.Play();

        StartCoroutine(ReturnWhenDone(src, null));
    }

    private bool TryGetGroup(string groupName, out SoundGroup group)
    {
        if (_groupDict.TryGetValue(groupName, out group)) return true;
        Debug.LogWarning($"[SoundService] Sound group '{groupName}' not found.");
        return false;
    }

    private AudioSource GetPooledSource()
    {
        if (_pool.Count > 0)
            return _pool.Dequeue();

        Debug.LogWarning("[SoundService] AudioSource pool exhausted – creating a temporary source.");
        return CreateSource();
    }

    private AudioSource CreateSource()
    {
        GameObject go = new GameObject("PooledAudioSource");
        go.transform.SetParent(transform, false);
        AudioSource src = go.AddComponent<AudioSource>();
        src.playOnAwake = false;
        return src;
    }

    private void ConfigureSource(AudioSource src, SoundGroup group)
    {
        if (group.Clips == null || group.Clips.Length == 0)
        {
            Debug.LogWarning($"[SoundService] Group '{group.GroupName}' has no clips assigned.");
            return;
        }

        src.clip   = group.Clips[Random.Range(0, group.Clips.Length)];
        src.volume = group.Volume;
        
        float randomPitch = group.Pitch + Random.Range(-group.PitchVariance, group.PitchVariance);
        src.pitch  = Mathf.Clamp(randomPitch, 0.1f, 3f);
        
        src.loop   = group.Loop;
    }

    private IEnumerator ReturnWhenDone(AudioSource src, GameObject attachedTo)
    {
        if (!src.loop)
            yield return new WaitWhile(() => src.isPlaying);

        if (!src.loop)
            yield return new WaitWhile(() => src.isPlaying);

        src.transform.SetParent(transform, false);
        src.transform.localPosition = Vector3.zero;
        src.clip = null;

        _pool.Enqueue(src);
    }
}
