using UnityEngine;

/// <summary>
/// Handles key spawning and triggers janitor aggression when conditions are met.
/// </summary>
/// <remarks>
/// This script ensures:
/// - A key is only spawned once per enemy
/// - The janitor becomes aggressive when the player is not crouching
/// 
/// It is typically triggered as part of an interaction or event (e.g., enemy defeat).
/// </remarks>
public class JanitorKey : MonoBehaviour
{
    [Header("Components")]

    /// <summary>Provides access to player state (e.g., crouching).</summary>
    private PlayerManager _playerManager;

    /// <summary>Reference to the enemy component for state management.</summary>
    private Enemy _enemy;

    /// <summary>Reference to the janitor behaviour script.</summary>
    private Janitor _janitor;

    [Header("Prefabs")]

    /// <summary>Prefab of the key to be spawned.</summary>
    public GameObject KeyPrefab;

    /// <summary>
    /// Initialises component references.
    /// </summary>
    void Start()
    {
        _playerManager = GameObject.Find("Player").GetComponent<PlayerManager>();
        _enemy = GetComponent<Enemy>();
        _janitor = GetComponent<Janitor>();
    }

    /// <summary>
    /// Spawns a key and updates janitor behaviour.
    /// </summary>
    /// <remarks>
    /// - Prevents spawning multiple keys using a flag
    /// - If the player is not crouching, the janitor becomes aggressive
    ///   and begins chasing the player
    /// </remarks>
    public void Spawn()
    {
        // Spawn key only once
        if (_enemy.KeyDropped == false)
        {
            Instantiate(
                KeyPrefab,
                transform.position + new Vector3(0, -1.5f, 0),
                Quaternion.identity
            );

            _enemy.KeyDropped = true;
        }

        // Trigger aggression if player is not crouching
        if (!_playerManager.IsCrouching)
        {
            _enemy.isAggressive = true;
            _janitor.chasePlayer = true;
        }
    }
}