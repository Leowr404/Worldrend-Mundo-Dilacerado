using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "RPG/Dialogue")]
public class DialogueData : ScriptableObject
{
    public string npcName;
    [TextArea(2, 5)] public List<string> lines;

    [Header("Falas da Quest")]
    [TextArea(2, 5)] public List<string> beforeQuest;
    [TextArea(2, 5)] public List<string> duringQuest;
    [TextArea(2, 5)] public List<string> afterQuest;

    [Header("Quest Opcional")]
    public Quest quest;

    [Header("Opções de Diálogo")]
    public List<DialogueOption> options;
}

[System.Serializable]
public class DialogueOption
{
    public string optionText;
    [TextArea(2, 5)] public List<string> response;
    public bool acceptQuest;
}