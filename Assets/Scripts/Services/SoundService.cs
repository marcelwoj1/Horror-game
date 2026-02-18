using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// ---------------------------------------------------------------------------
// SoundGroup  –  one named group of clips (e.g. "Footsteps", "Music")
// Shown as a list in the Inspector on the SoundService GameObject.
// ---------------------------------------------------------------------------
[System.Serializable]
public class SoundGroup
{
    [Tooltip("Key used in SoundManager.Play(\"...\")")]
    public string GroupName;

    [Tooltip("One or more clips; a random one is chosen each play")]
    public AudioClip[] Clips;

    [Range(0f, 1f)]
    public float Volume = 1f;

    [Range(0.5f, 2f)]
    public float Pitch = 1f;

    [Tooltip("Loop the clip?")]
    public bool Loop = false;

    [Tooltip("Global = 2-D (no position). Local = 3-D (requires a position).")]
    public bool IsGlobal = true;
}

// ---------------------------------------------------------------------------
// SoundService  –  singleton MonoBehaviour
// Attach to a persistent GameObject in your first scene.
// ---------------------------------------------------------------------------
public class SoundService : MonoBehaviour
{
    // ── Singleton ────────────────────────────────────────────────────────────
    public static SoundService Instance { get; private set; }

    // ── Inspector ────────────────────────────────────────────────────────────
    [Header("Sound Groups")]
    [SerializeField] private List<SoundGroup> _soundGroups = new List<SoundGroup>();

    [Header("Settings")]
    [Tooltip("Max simultaneous AudioSources in the pool")]
    [SerializeField] private int _poolSize = 16;

    // ── Private state ────────────────────────────────────────────────────────
    private Dictionary<string, SoundGroup> _groupDict = new Dictionary<string, SoundGroup>();
    private Queue<AudioSource> _pool = new Queue<AudioSource>();

    // ── Unity lifecycle ──────────────────────────────────────────────────────
    private void Awake()
    {
        // Singleton enforcement
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Build lookup dictionary
        foreach (var group in _soundGroups)
        {
            if (group == null || string.IsNullOrEmpty(group.GroupName)) continue;

            if (!_groupDict.ContainsKey(group.GroupName))
                _groupDict.Add(group.GroupName, group);
            else
                Debug.LogWarning($"[SoundService] Duplicate group name '{group.GroupName}' – skipping.");
        }

        // Pre-warm the AudioSource pool
        for (int i = 0; i < _poolSize; i++)
            _pool.Enqueue(CreateSource());
    }

    // ── Public API ───────────────────────────────────────────────────────────

    /// <summary>
    /// Play a global (2-D) sound.
    /// Example: SoundService.Instance.Play("Music");
    /// </summary>
    public void Play(string groupName)
    {
        if (!TryGetGroup(groupName, out SoundGroup group)) return;

        if (!group.IsGlobal)
            Debug.LogWarning($"[SoundService] '{groupName}' is marked as Local but was called without a position. Playing globally.");

        PlayGlobal(group);
    }

    /// <summary>
    /// Play a local (3-D / positional) sound at a world position.
    /// Example: SoundService.Instance.Play("Footsteps", transform.position);
    /// </summary>
    public void Play(string groupName, Vector2 position)
    {
        Play(groupName, position, null);
    }

    /// <summary>
    /// Play a local (3-D / positional) sound at a world position and optionally
    /// attach it to a GameObject so it follows that object until it finishes.
    /// Example: SoundService.Instance.Play("Footsteps", transform.position, gameObject);
    /// </summary>
    public void Play(string groupName, Vector2 position, GameObject attachTo)
    {
        if (!TryGetGroup(groupName, out SoundGroup group)) return;

        if (group.IsGlobal)
            Debug.LogWarning($"[SoundService] '{groupName}' is marked as Global but was called with a position. Playing globally.");

        AudioSource src = GetPooledSource();
        ConfigureSource(src, group);

        if (attachTo != null)
        {
            // Reparent the source so it follows the object
            src.transform.SetParent(attachTo.transform, false);
            src.transform.localPosition = Vector3.zero;
        }
        else
        {
            src.transform.SetParent(transform, false);
            src.transform.position = new Vector3(position.x, position.y, 0f);
        }

        src.spatialBlend = 1f; // full 3-D
        src.Play();

        StartCoroutine(ReturnWhenDone(src, attachTo));
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private void PlayGlobal(SoundGroup group)
    {
        AudioSource src = GetPooledSource();
        ConfigureSource(src, group);
        src.transform.SetParent(transform, false);
        src.spatialBlend = 0f; // full 2-D
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

        // Pool exhausted – create a temporary extra source
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
        // Pick a random clip from the group
        if (group.Clips == null || group.Clips.Length == 0)
        {
            Debug.LogWarning($"[SoundService] Group '{group.GroupName}' has no clips assigned.");
            return;
        }

        src.clip   = group.Clips[Random.Range(0, group.Clips.Length)];
        src.volume = group.Volume;
        src.pitch  = group.Pitch;
        src.loop   = group.Loop;
    }

    /// <summary>
    /// Waits until the source finishes playing, then detaches and returns it to the pool.
    /// </summary>
    private IEnumerator ReturnWhenDone(AudioSource src, GameObject attachedTo)
    {
        // Wait for the clip to finish (loop = never returns on its own)
        if (!src.loop)
            yield return new WaitWhile(() => src.isPlaying);

        // Detach from any parent object and re-parent to this manager
        src.transform.SetParent(transform, false);
        src.transform.localPosition = Vector3.zero;
        src.clip = null;

        _pool.Enqueue(src);
    }
}
