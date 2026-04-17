using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    [SerializeField] private PlayerStats player;

    public List<Quest> activeQuests = new List<Quest>();
    public List<Quest> completedQuests = new List<Quest>();

    public event Action<Quest> OnQuestAdded;
    public event Action<Quest> OnObjectiveCompleted;
    public event Action<Quest> OnQuestCompleted;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        if (player == null)
            player = FindAnyObjectByType<PlayerStats>();
    }

    public void AddQuest(Quest quest)
    {
        if (quest == null) return;
        if (activeQuests.Contains(quest) || completedQuests.Contains(quest)) return;

        quest.ResetProgress();
        activeQuests.Add(quest);

        UiManager.Notify($"Nova quest: {quest.questName}");
        OnQuestAdded?.Invoke(quest);
    }

    public bool HasQuest(Quest quest) => activeQuests.Contains(quest);
    public bool IsCompleted(Quest quest) => completedQuests.Contains(quest);

    // ── Reportar progresso ──────────────────────────────

    public void ReportKill(string enemyID) => ReportProgress(QuestObjectiveType.Kill, enemyID);
    public void ReportCollect(string itemID, int amount = 1) => ReportProgress(QuestObjectiveType.Collect, itemID, amount);
    public void ReportAreaReached(string areaID) => ReportProgress(QuestObjectiveType.ExploreArea, areaID);
    public void ReportTalkToNPC(string npcID) => ReportProgress(QuestObjectiveType.TalkToNPC, npcID);

    private void ReportProgress(QuestObjectiveType type, string id, int amount = 1)
    {
        foreach (var quest in activeQuests)
        {
            quest.objective.RegisterProgress(type, id, amount);

            if (quest.TryCompleteObjective())
            {
                UiManager.Notify($"Objetivo completo!\nVolte ao NPC para entregar.");
                OnObjectiveCompleted?.Invoke(quest);
            }
        }
    }

    // ── Entrega ao NPC ──────────────────────────────────

    public void TryDeliverQuest(string npcID)
    {
        for (int i = activeQuests.Count - 1; i >= 0; i--)
        {
            var quest = activeQuests[i];
            if (quest.deliveryNPCID != npcID) continue;
            if (!quest.isReadyToDeliver) continue;

            activeQuests.RemoveAt(i);
            completedQuests.Add(quest);
            quest.DeliverAndReward(player);

            UiManager.Notify($"Quest concluída: {quest.questName}!");
            OnQuestCompleted?.Invoke(quest);
        }
    }

    public bool HasQuestToDeliver(string npcID)
    {
        foreach (var q in activeQuests)
            if (q.deliveryNPCID == npcID && q.isReadyToDeliver)
                return true;
        return false;
    }
}