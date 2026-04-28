using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages the display of player health using heart UI elements.
/// </summary>
/// <remarks>
/// This system:
/// - Listens for health changes via events
/// - Dynamically creates and updates heart UI elements
/// - Represents player health visually using full/empty hearts
///
/// The UI is rebuilt whenever health changes to ensure consistency.
/// </remarks>
public class HealthHeartManager : MonoBehaviour
{
    /// <summary>Prefab used to create heart UI elements.</summary>
    public GameObject HeartPrefab;

    /// <summary>Reference to the player's health system.</summary>
    public PlayerHealth playerHealth;

    /// <summary>List of currently active heart UI elements.</summary>
    private List<HealthHeart> hearts = new List<HealthHeart>();

    /// <summary>
    /// Subscribes to health change events when enabled.
    /// </summary>
    private void OnEnable()
    {
        playerHealth.OnHealthChanged += DrawHearts;
    }

    /// <summary>
    /// Unsubscribes from health change events when disabled.
    /// </summary>
    private void OnDisable()
    {
        playerHealth.OnHealthChanged -= DrawHearts;
    }

    /// <summary>
    /// Initialises heart UI on game start.
    /// </summary>
    public void Start()
    {
        DrawHearts();
    }

    /// <summary>
    /// Rebuilds and updates all heart UI elements.
    /// </summary>
    /// <remarks>
    /// This method:
    /// - Clears existing hearts
    /// - Creates new hearts based on maximum health
    /// - Updates each heart to reflect current health
    /// </remarks>
    public void DrawHearts()
    {
        // Clear existing hearts
        ClearHearts();

        // Create hearts based on max health
        int heartsToDraw = playerHealth.MaxHealth;

        for (int i = 0; i < heartsToDraw; i++)
        {
            CreateHearts();
        }

        // Update heart states based on current health
        for (int i = 0; i < hearts.Count; i++)
        {
            int heartState = Mathf.Clamp(playerHealth.Health - i, 0, 1);
            hearts[i].SetHeartState((HealthHeart.HeartState)heartState);
        }
    }

    /// <summary>
    /// Removes all existing heart UI elements.
    /// </summary>
    public void ClearHearts()
    {
        foreach (Transform t in transform)
        {
            Destroy(t.gameObject);
        }

        hearts = new List<HealthHeart>();
    }

    /// <summary>
    /// Creates and initialises a new heart UI element.
    /// </summary>
    public void CreateHearts()
    {
        GameObject newHeart = Instantiate(HeartPrefab);

        newHeart.transform.SetParent(transform);

        HealthHeart heartComponent = newHeart.GetComponent<HealthHeart>();

        // Default to full state
        heartComponent.SetHeartState(HealthHeart.HeartState.Full);

        hearts.Add(heartComponent);
    }
}