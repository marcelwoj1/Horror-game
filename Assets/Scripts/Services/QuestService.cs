using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Represents a single quest with an identifier, description, and completion state.
/// </summary>
[System.Serializable]
public class Quest
{
    /// <summary>Unique identifier used to match quest completion events.</summary>
    public string identifier;

    /// <summary>Description displayed to the player.</summary>
    public string description;

    /// <summary>Indicates whether the quest has been completed.</summary>
    public bool isCompleted;
}

/// <summary>
/// Manages quest progression and updates UI feedback.
/// </summary>
/// <remarks>
/// This system:
/// - Tracks a list of quests in chronological order
/// - Advances quests when conditions are satisfied
/// - Updates on-screen quest text
/// - Plays feedback sounds upon completion
///
/// Designed as a linear quest system where only the current quest
/// can be completed at a given time.
/// </remarks>
public class QuestService : MonoBehaviour
{
    /// <summary>UI object displaying the current quest text.</summary>
    [Tooltip("Text OBJ")]
    public GameObject textDisplay;

    /// <summary>List of all quests in order.</summary>
    [Tooltip("List of quests")]
    public List<Quest> quests = new List<Quest>();

    /// <summary>Index of the current active quest.</summary>
    private int _currentQuestIndex = 0;

    /// <summary>Reference to the TextMeshPro UI component.</summary>
    private TextMeshProUGUI _questTextComponent;

    /// <summary>
    /// Initialises UI references and displays the first quest.
    /// </summary>
    void Start()
    {
        if (textDisplay != null)
        {
            _questTextComponent = textDisplay.GetComponentInChildren<TextMeshProUGUI>();
        }

        UpdateQuestText();
    }

    /// <summary>
    /// Marks the current quest as completed if the identifier matches.
    /// </summary>
    /// <param name="identifier">Identifier of the completed objective.</param>
    /// <remarks>
    /// Only the current quest in sequence can be completed.
    /// Advances to the next quest upon success.
    /// </remarks>
    public void SatisfyQuest(string identifier)
    {
        // Check if all quests are finished
        if (_currentQuestIndex >= quests.Count)
        {
            return; 
        }

        Quest currentQuest = quests[_currentQuestIndex];

        // Ensure the identifier matches the current chronological quest
        if (currentQuest.identifier == identifier && !currentQuest.isCompleted)
        {
            currentQuest.isCompleted = true;
            _currentQuestIndex++;
            UpdateQuestText();

            SoundService.Instance?.Play("QuestDone");
        }
        else
        {
        }
    }

    /// <summary>
    /// Updates the quest text displayed on the UI.
    /// </summary>
    /// <remarks>
    /// Displays the current quest description, or a completion message
    /// if all quests are finished.
    /// </remarks>
    private void UpdateQuestText()
    {
        if (_questTextComponent != null)
        {
            if (_currentQuestIndex < quests.Count)
            {
                _questTextComponent.text = quests[_currentQuestIndex].description;
            }
            else
            {
                _questTextComponent.text = "All objectives completed.";
            }
        }
    }
}