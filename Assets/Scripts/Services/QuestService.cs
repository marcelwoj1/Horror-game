using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class Quest
{
    public string identifier;
    public string description;
    public bool isCompleted;
}

public class QuestService : MonoBehaviour
{
    [Tooltip("The GameObject containing the UI Text element to display the current quest description.")]
    public GameObject textDisplay;

    [Tooltip("Chronological list of quests the player must complete.")]
    public List<Quest> quests = new List<Quest>();

    private int _currentQuestIndex = 0;
    private TextMeshProUGUI _questTextComponent;

    void Start()
    {
        // Get the TextMeshPro component from the textDisplay GameObject or its children
        if (textDisplay != null)
        {
            _questTextComponent = textDisplay.GetComponentInChildren<TextMeshProUGUI>();
        }

        UpdateQuestText();
    }

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
