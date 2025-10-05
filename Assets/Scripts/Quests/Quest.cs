using UnityEngine;

[CreateAssetMenu(fileName = "NewQuest", menuName = "RPG/Quest")]
public class Quest : ScriptableObject
{
    public string questName;
    [TextArea(2, 4)] public string description;
    public bool isCompleted;
    public int requiredItems = 3;
    public int collectedItems = 0;

    public void ResetProgress()
    {
        isCompleted = false;
        collectedItems = 0;
    }

    public void AddProgress(int amount = 1)
    {
        collectedItems += amount;
        if (collectedItems >= requiredItems)
        {
            collectedItems = requiredItems;
            isCompleted = true;
        }
    }
}