using UnityEngine;

public enum QuestObjectiveType
{
    Kill,
    Collect,
    ReachLocation
}

[System.Serializable]
public class QuestObjective
{
    public QuestObjectiveType type;
    public string targetID;
    public int requiredAmount;
    public int currentAmount;

    public bool IsComplete()
    {
        return currentAmount >= requiredAmount;
    }

    public void OnAreaReached(string areaID)
    {
        if (type == QuestObjectiveType.ReachLocation && areaID == targetID)
        {
            currentAmount++;
        }
    }
}