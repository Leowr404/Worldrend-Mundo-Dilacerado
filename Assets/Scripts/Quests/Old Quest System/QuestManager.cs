using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;
    public List<Quest> activeQuests = new List<Quest>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddQuest(Quest quest)
    {
        if (quest == null) return;
        if (activeQuests.Contains(quest)) return;

        quest.ResetProgress(); // ✅ Garante que não inicie completa
        activeQuests.Add(quest);

        //Debug.Log($"📜 Nova quest adicionada: {quest.questName}");
    }

    public bool HasQuest(Quest quest)
    {
        return activeQuests.Contains(quest);
    }

    public void CompleteQuest(Quest quest)
    {
        if (quest == null) return;
        quest.isCompleted = true;
        //Debug.Log($"✅ Quest concluída: {quest.questName}");
        PlayerStats player = FindAnyObjectByType<PlayerStats>();
        if (player != null)
            quest.GiveRewards(player);
    }


}