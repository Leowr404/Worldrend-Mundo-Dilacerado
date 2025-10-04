using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;
    private List<Quest> activeQuests = new List<Quest>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddQuest(Quest quest)
    {
        if (!activeQuests.Contains(quest))
        {
            activeQuests.Add(quest);
            Debug.Log($"Nova Quest: {quest.questName}");
        }
    }

    public bool HasQuest(Quest quest)
    {
        return activeQuests.Contains(quest);
    }

    public void UpdateProgress(string targetID, QuestObjectiveType type)
    {
        foreach (Quest quest in activeQuests)
        {
            if (quest.isCompleted) continue;

            foreach (QuestObjective obj in quest.objectives)
            {
                if (obj.type == type && obj.targetID == targetID && !obj.IsComplete())
                {
                    obj.currentAmount++;
                    Debug.Log($"{quest.questName}: {obj.currentAmount}/{obj.requiredAmount}");

                    if (IsQuestComplete(quest))
                    {
                        quest.isCompleted = true;
                        Debug.Log($"Quest Concluída: {quest.questName}");
                    }
                }
            }
        }
    }

    private bool IsQuestComplete(Quest quest)
    {
        foreach (QuestObjective obj in quest.objectives)
        {
            if (!obj.IsComplete()) return false;
        }
        return true;
    }
}
