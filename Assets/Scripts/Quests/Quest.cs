using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewQuest", menuName = "RPG/Quest")]
public class Quest : ScriptableObject
{
    public string questName;
    [TextArea] public string description;

    public List<QuestObjective> objectives;
    public bool isCompleted;
}