using UnityEngine;

public enum QuestObjectiveType
{
    Kill,
    Collect,
    ExploreArea,
    TalkToNPC
}

[System.Serializable]
public class QuestObjective
{
    public QuestObjectiveType type;
    public string targetID;       // ex: "goblin", "herb", "cave_entrance", "npc_elder"
    public string description;    // ex: "Matar 5 Goblins"
    public int requiredAmount;    // para Kill e Collect; para Explore/Talk = 1
    public int currentAmount;

    public bool IsComplete() => currentAmount >= requiredAmount;

    public void RegisterProgress(QuestObjectiveType progressType, string id, int amount = 1)
    {
        if (IsComplete()) return;
        if (type != progressType || targetID != id) return;
        currentAmount = Mathf.Min(currentAmount + amount, requiredAmount);
    }
}