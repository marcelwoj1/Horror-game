using UnityEngine;

/// <summary>
/// Represents a food object that can be detected and consumed by enemies.
/// </summary>
/// <remarks>
/// This script automatically registers and unregisters itself with the global
/// food tracking system in <see cref="EnemyPatrol"/>.
/// 
/// This allows enemies to dynamically detect available food targets without
/// needing direct references.
/// </remarks>
public class SpiderFood : MonoBehaviour
{
    /// <summary>
    /// Registers this food object when it becomes active.
    /// </summary>
    /// <remarks>
    /// Called automatically by Unity when the object is enabled.
    /// Adds this instance to the global food list used by enemies.
    /// </remarks>
    void OnEnable()
    {
        EnemyPatrol.RegisterFood(this);
    }

    /// <summary>
    /// Unregisters this food object when it is disabled or destroyed.
    /// </summary>
    /// <remarks>
    /// Ensures enemies do not target invalid or removed food objects.
    /// </remarks>
    void OnDisable()
    {
        EnemyPatrol.UnregisterFood(this);
    }
}